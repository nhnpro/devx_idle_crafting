using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Tube Generator")]
	public class TubeGenerator : MeshGenerator
	{
		public enum CapMethod
		{
			None,
			Flat,
			Round
		}

		[SerializeField]
		[HideInInspector]
		private int _sides = 12;

		[SerializeField]
		[HideInInspector]
		private int _roundCapLatitude = 6;

		[SerializeField]
		[HideInInspector]
		private bool _cap;

		[SerializeField]
		[HideInInspector]
		private CapMethod _capMode;

		[SerializeField]
		[HideInInspector]
		private float _integrity = 360f;

		[SerializeField]
		[HideInInspector]
		private float _capUVScale = 1f;

		private int bodyVertexCount;

		private int bodyTrisCount;

		private int capVertexCount;

		private int capTrisCount;

		public int sides
		{
			get
			{
				return _sides;
			}
			set
			{
				if (value != _sides)
				{
					if (value < 3)
					{
						value = 3;
					}
					_sides = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		[Obsolete("Deprecated in 1.0.7. Use capMode instead")]
		public bool cap
		{
			get
			{
				return _cap;
			}
			set
			{
				if (value != _cap)
				{
					_cap = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public CapMethod capMode
		{
			get
			{
				return _capMode;
			}
			set
			{
				if (value != _capMode)
				{
					_capMode = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public int roundCapLatitude
		{
			get
			{
				return _roundCapLatitude;
			}
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				if (value != _roundCapLatitude)
				{
					_roundCapLatitude = value;
					if (_capMode == CapMethod.Round)
					{
						Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public float integrity
		{
			get
			{
				return _integrity;
			}
			set
			{
				if (value != _integrity)
				{
					_integrity = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public float capUVScale
		{
			get
			{
				return _capUVScale;
			}
			set
			{
				if (value != _capUVScale)
				{
					_capUVScale = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		private bool useCap
		{
			get
			{
				bool flag = _capMode != CapMethod.None;
				if (base.computer != null)
				{
					return flag && (!base.computer.isClosed || base.span < 1.0);
				}
				if (sampleUser)
				{
					SplineUser rootUser = base.rootUser;
					if (rootUser == null)
					{
						return flag;
					}
					if (rootUser.computer != null)
					{
						return flag && (!rootUser.computer.isClosed || rootUser.span < 1.0);
					}
				}
				return _cap;
			}
		}

		protected override void Reset()
		{
			base.Reset();
		}

		protected override void Awake()
		{
			base.Awake();
			if (_cap)
			{
				_capMode = CapMethod.Flat;
				_cap = false;
			}
			mesh.name = "tube";
		}

		protected override void BuildMesh()
		{
			if (_sides > 2)
			{
				base.BuildMesh();
				bodyVertexCount = (_sides + 1) * base.clippedSamples.Length;
				CapMethod capMethod = _capMode;
				if (!useCap)
				{
					capMethod = CapMethod.None;
				}
				switch (capMethod)
				{
				case CapMethod.Flat:
					capVertexCount = _sides + 1;
					break;
				case CapMethod.Round:
					capVertexCount = _roundCapLatitude * (sides + 1);
					break;
				default:
					capVertexCount = 0;
					break;
				}
				int vertexCount = bodyVertexCount + capVertexCount * 2;
				bodyTrisCount = _sides * (base.clippedSamples.Length - 1) * 2 * 3;
				switch (capMethod)
				{
				case CapMethod.Flat:
					capTrisCount = (_sides - 1) * 3 * 2;
					break;
				case CapMethod.Round:
					capTrisCount = _sides * _roundCapLatitude * 6;
					break;
				default:
					capTrisCount = 0;
					break;
				}
				AllocateMesh(vertexCount, bodyTrisCount + capTrisCount * 2);
				Generate();
				switch (capMethod)
				{
				case CapMethod.Flat:
					GenerateFlatCaps();
					break;
				case CapMethod.Round:
					GenerateRoundCaps();
					break;
				}
			}
		}

		private void Generate()
		{
			int num = 0;
			ResetUVDistance();
			for (int i = 0; i < base.clippedSamples.Length; i++)
			{
				Vector3 vector = base.clippedSamples[i].position;
				Vector3 right = base.clippedSamples[i].right;
				if (base.offset != Vector3.zero)
				{
					Vector3 a = vector;
					Vector3 offset = base.offset;
					Vector3 a2 = offset.x * right;
					Vector3 offset2 = base.offset;
					Vector3 a3 = a2 + offset2.y * base.clippedSamples[i].normal;
					Vector3 offset3 = base.offset;
					vector = a + (a3 + offset3.z * base.clippedSamples[i].direction);
				}
				if (base.uvMode == UVMode.UniformClamp || base.uvMode == UVMode.UniformClip)
				{
					AddUVDistance(i);
				}
				for (int j = 0; j < _sides + 1; j++)
				{
					float num2 = (float)j / (float)_sides;
					Quaternion rotation = Quaternion.AngleAxis(_integrity * num2 + base.rotation + 180f, base.clippedSamples[i].direction);
					tsMesh.vertices[num] = vector + rotation * right * base.size * base.clippedSamples[i].size * 0.5f;
					CalculateUVs(base.clippedSamples[i].percent, num2);
					tsMesh.uv[num] = Vector2.one * 0.5f + (Vector2)(Quaternion.AngleAxis(base.uvRotation, Vector3.forward) * (Vector2.one * 0.5f - MeshGenerator.uvs));
					tsMesh.normals[num] = Vector3.Normalize(tsMesh.vertices[num] - vector);
					tsMesh.colors[num] = base.clippedSamples[i].color * base.color;
					num++;
				}
			}
			MeshUtility.GeneratePlaneTriangles(ref tsMesh.triangles, _sides, base.clippedSamples.Length, flip: false);
		}

		private void GenerateFlatCaps()
		{
			for (int i = 0; i < _sides + 1; i++)
			{
				int num = bodyVertexCount + i;
				tsMesh.vertices[num] = tsMesh.vertices[i];
				tsMesh.normals[num] = -base.clippedSamples[0].direction;
				tsMesh.colors[num] = tsMesh.colors[i];
				tsMesh.uv[num] = Quaternion.AngleAxis(_integrity * ((float)i / (float)(_sides - 1)), Vector3.forward) * Vector2.right * 0.5f * capUVScale + Vector3.right * 0.5f + Vector3.up * 0.5f;
			}
			for (int j = 0; j < _sides + 1; j++)
			{
				int num2 = bodyVertexCount + (_sides + 1) + j;
				int num3 = bodyVertexCount - (_sides + 1) + j;
				tsMesh.vertices[num2] = tsMesh.vertices[num3];
				tsMesh.normals[num2] = base.clippedSamples[base.clippedSamples.Length - 1].direction;
				tsMesh.colors[num2] = tsMesh.colors[num3];
				tsMesh.uv[num2] = Quaternion.AngleAxis(_integrity * ((float)num3 / (float)(_sides - 1)), Vector3.forward) * Vector2.right * 0.5f * capUVScale + Vector3.right * 0.5f + Vector3.up * 0.5f;
			}
			int num4 = bodyTrisCount;
			int num5 = (_integrity != 360f) ? _sides : (_sides - 1);
			for (int k = 0; k < num5 - 1; k++)
			{
				tsMesh.triangles[num4++] = k + bodyVertexCount + 2;
				tsMesh.triangles[num4++] = k + bodyVertexCount + 1;
				tsMesh.triangles[num4++] = bodyVertexCount;
			}
			for (int l = 0; l < num5 - 1; l++)
			{
				tsMesh.triangles[num4++] = bodyVertexCount + (_sides + 1);
				tsMesh.triangles[num4++] = l + 1 + bodyVertexCount + (_sides + 1);
				tsMesh.triangles[num4++] = l + 2 + bodyVertexCount + (_sides + 1);
			}
		}

		private void GenerateRoundCaps()
		{
			Vector3 position = base.clippedSamples[0].position;
			Quaternion lhs = Quaternion.LookRotation(-base.clippedSamples[0].direction, base.clippedSamples[0].normal);
			float num = 0f;
			float num2 = 0f;
			switch (base.uvMode)
			{
			case UVMode.Clip:
				num = (float)base.clippedSamples[0].percent;
				num2 = base.size * 0.5f / CalculateLength();
				break;
			case UVMode.UniformClip:
				num = CalculateLength(0.0, base.clippedSamples[0].percent);
				num2 = base.size * 0.5f;
				break;
			case UVMode.UniformClamp:
				num = 0f;
				num2 = base.size * 0.5f / (float)base.span;
				break;
			case UVMode.Clamp:
				num2 = base.size * 0.5f / CalculateLength(base.clipFrom, base.clipTo);
				break;
			}
			for (int i = 1; i < _roundCapLatitude + 1; i++)
			{
				float num3 = (float)i / (float)_roundCapLatitude;
				float angle = 90f * num3;
				for (int j = 0; j <= sides; j++)
				{
					float num4 = (float)j / (float)sides;
					int num5 = bodyVertexCount + j + (i - 1) * (sides + 1);
					Quaternion rhs = Quaternion.AngleAxis(_integrity * num4 + base.rotation + 180f, -Vector3.forward) * Quaternion.AngleAxis(angle, Vector3.up);
					tsMesh.vertices[num5] = position + lhs * rhs * -Vector3.right * base.size * 0.5f * base.clippedSamples[0].size;
					tsMesh.colors[num5] = base.clippedSamples[0].color * base.color;
					tsMesh.normals[num5] = (tsMesh.vertices[num5] - position).normalized;
					ref Vector2 reference = ref tsMesh.uv[num5];
					float num6 = num4;
					Vector2 uvScale = base.uvScale;
					float x = num6 * uvScale.x;
					float num7 = num - num2 * num3;
					Vector2 uvScale2 = base.uvScale;
					reference = new Vector2(x, num7 * uvScale2.y) - base.uvOffset;
				}
			}
			int num8 = bodyTrisCount;
			for (int k = -1; k < _roundCapLatitude - 1; k++)
			{
				for (int l = 0; l < sides; l++)
				{
					int num9 = bodyVertexCount + l + k * (sides + 1);
					int num10 = num9 + (sides + 1);
					if (k == -1)
					{
						num9 = l;
						num10 = bodyVertexCount + l;
					}
					tsMesh.triangles[num8++] = num10 + 1;
					tsMesh.triangles[num8++] = num9 + 1;
					tsMesh.triangles[num8++] = num9;
					tsMesh.triangles[num8++] = num10;
					tsMesh.triangles[num8++] = num10 + 1;
					tsMesh.triangles[num8++] = num9;
				}
			}
			position = base.clippedSamples[base.clippedSamples.Length - 1].position;
			lhs = Quaternion.LookRotation(base.clippedSamples[base.clippedSamples.Length - 1].direction, base.clippedSamples[base.clippedSamples.Length - 1].normal);
			switch (base.uvMode)
			{
			case UVMode.Clip:
				num = (float)base.clippedSamples[base.clippedSamples.Length - 1].percent;
				break;
			case UVMode.UniformClip:
				num = CalculateLength(0.0, base.clippedSamples[base.clippedSamples.Length - 1].percent);
				break;
			case UVMode.Clamp:
				num = 1f;
				break;
			case UVMode.UniformClamp:
				num = CalculateLength();
				break;
			}
			for (int m = 1; m < _roundCapLatitude + 1; m++)
			{
				float num17 = (float)m / (float)_roundCapLatitude;
				float angle2 = 90f * num17;
				for (int n = 0; n <= sides; n++)
				{
					float num18 = (float)n / (float)sides;
					int num19 = bodyVertexCount + capVertexCount + n + (m - 1) * (sides + 1);
					Quaternion rhs2 = Quaternion.AngleAxis(_integrity * num18 + base.rotation + 180f, Vector3.forward) * Quaternion.AngleAxis(angle2, -Vector3.up);
					tsMesh.vertices[num19] = position + lhs * rhs2 * Vector3.right * base.size * 0.5f * base.clippedSamples[base.clippedSamples.Length - 1].size;
					tsMesh.normals[num19] = (tsMesh.vertices[num19] - position).normalized;
					tsMesh.colors[num19] = base.clippedSamples[base.clippedSamples.Length - 1].color * base.color;
					ref Vector2 reference2 = ref tsMesh.uv[num19];
					float num20 = num18;
					Vector2 uvScale3 = base.uvScale;
					float x2 = num20 * uvScale3.x;
					float num21 = num + num2 * num17;
					Vector2 uvScale4 = base.uvScale;
					reference2 = new Vector2(x2, num21 * uvScale4.y) - base.uvOffset;
				}
			}
			for (int num22 = -1; num22 < _roundCapLatitude - 1; num22++)
			{
				for (int num23 = 0; num23 < sides; num23++)
				{
					int num24 = bodyVertexCount + capVertexCount + num23 + num22 * (sides + 1);
					int num25 = num24 + (sides + 1);
					if (num22 == -1)
					{
						num24 = bodyVertexCount - (_sides + 1) + num23;
						num25 = bodyVertexCount + capVertexCount + num23;
					}
					tsMesh.triangles[num8++] = num24 + 1;
					tsMesh.triangles[num8++] = num25 + 1;
					tsMesh.triangles[num8++] = num25;
					tsMesh.triangles[num8++] = num25;
					tsMesh.triangles[num8++] = num24;
					tsMesh.triangles[num8++] = num24 + 1;
				}
			}
		}
	}
}
