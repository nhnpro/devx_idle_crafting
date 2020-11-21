using UnityEngine;

namespace Dreamteck.Splines
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Waveform Generator")]
	public class WaveformGenerator : MeshGenerator
	{
		public enum Axis
		{
			X,
			Y,
			Z
		}

		public enum UVWrapMode
		{
			Clamp,
			UniformX,
			UniformY,
			Uniform
		}

		[SerializeField]
		[HideInInspector]
		private Axis _axis = Axis.Y;

		[SerializeField]
		[HideInInspector]
		private bool _symmetry;

		[SerializeField]
		[HideInInspector]
		private UVWrapMode _uvWrapMode;

		[SerializeField]
		[HideInInspector]
		private int _slices = 1;

		public Axis axis
		{
			get
			{
				return _axis;
			}
			set
			{
				if (value != _axis)
				{
					_axis = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool symmetry
		{
			get
			{
				return _symmetry;
			}
			set
			{
				if (value != _symmetry)
				{
					_symmetry = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public UVWrapMode uvWrapMode
		{
			get
			{
				return _uvWrapMode;
			}
			set
			{
				if (value != _uvWrapMode)
				{
					_uvWrapMode = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public int slices
		{
			get
			{
				return _slices;
			}
			set
			{
				if (value != _slices)
				{
					if (value < 1)
					{
						value = 1;
					}
					_slices = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			mesh.name = "waveform";
		}

		protected override void BuildMesh()
		{
			base.BuildMesh();
			if (_symmetry)
			{
				GenerateSymmetrical();
			}
			else
			{
				GenerateDefault();
			}
		}

		protected override void Build()
		{
			base.Build();
		}

		protected override void LateRun()
		{
			base.LateRun();
		}

		private void GenerateDefault()
		{
			int vertexCount = base.clippedSamples.Length * (_slices + 1);
			AllocateMesh(vertexCount, _slices * (base.clippedSamples.Length - 1) * 6);
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < base.clippedSamples.Length; i++)
			{
				Vector3 position = base.clippedSamples[i].position;
				Vector3 a = position;
				Vector3 vector = Vector3.right;
				float num5 = 1f;
				if ((_uvWrapMode == UVWrapMode.UniformX || _uvWrapMode == UVWrapMode.Uniform) && i > 0)
				{
					num4 += Vector3.Distance(base.clippedSamples[i].position, base.clippedSamples[i - 1].position);
				}
				switch (_axis)
				{
				case Axis.X:
				{
					Vector3 position4 = base.computer.position;
					num3 = (a.x = position4.x);
					Vector2 uvScale3 = base.uvScale;
					num5 = uvScale3.y * Mathf.Abs(position.x - a.x);
					num2 += position.x;
					break;
				}
				case Axis.Y:
				{
					Vector3 position3 = base.computer.position;
					num3 = (a.y = position3.y);
					Vector2 uvScale2 = base.uvScale;
					num5 = uvScale2.y * Mathf.Abs(position.y - a.y);
					vector = Vector3.up;
					num2 += position.y;
					break;
				}
				case Axis.Z:
				{
					Vector3 position2 = base.computer.position;
					num3 = (a.z = position2.z);
					Vector2 uvScale = base.uvScale;
					num5 = uvScale.y * Mathf.Abs(position.z - a.z);
					vector = Vector3.forward;
					num2 += position.z;
					break;
				}
				}
				Vector3 normalized = Vector3.Cross(vector, base.clippedSamples[i].direction).normalized;
				Vector3 vector2 = Vector3.Cross(base.clippedSamples[i].normal, base.clippedSamples[i].direction);
				for (int j = 0; j < _slices + 1; j++)
				{
					float num6 = (float)j / (float)_slices;
					ref Vector3 reference = ref tsMesh.vertices[num];
					Vector3 a2 = Vector3.Lerp(a, position, num6);
					Vector3 a3 = vector;
					Vector3 offset = base.offset;
					Vector3 a4 = a2 + a3 * offset.y;
					Vector3 a5 = vector2;
					Vector3 offset2 = base.offset;
					reference = a4 + a5 * offset2.x;
					tsMesh.normals[num] = normalized;
					switch (_uvWrapMode)
					{
					case UVWrapMode.Clamp:
					{
						ref Vector2 reference5 = ref tsMesh.uv[num];
						float num19 = (float)base.clippedSamples[i].percent;
						Vector2 uvScale10 = base.uvScale;
						float num20 = num19 * uvScale10.x;
						Vector2 uvOffset7 = base.uvOffset;
						float x4 = num20 + uvOffset7.x;
						float num21 = num6;
						Vector2 uvScale11 = base.uvScale;
						float num22 = num21 * uvScale11.y;
						Vector2 uvOffset8 = base.uvOffset;
						reference5 = new Vector2(x4, num22 + uvOffset8.y);
						break;
					}
					case UVWrapMode.UniformX:
					{
						ref Vector2 reference4 = ref tsMesh.uv[num];
						float num15 = num4;
						Vector2 uvScale8 = base.uvScale;
						float num16 = num15 * uvScale8.x;
						Vector2 uvOffset5 = base.uvOffset;
						float x3 = num16 + uvOffset5.x;
						float num17 = num6;
						Vector2 uvScale9 = base.uvScale;
						float num18 = num17 * uvScale9.y;
						Vector2 uvOffset6 = base.uvOffset;
						reference4 = new Vector2(x3, num18 + uvOffset6.y);
						break;
					}
					case UVWrapMode.UniformY:
					{
						ref Vector2 reference3 = ref tsMesh.uv[num];
						float num11 = (float)base.clippedSamples[i].percent;
						Vector2 uvScale6 = base.uvScale;
						float num12 = num11 * uvScale6.x;
						Vector2 uvOffset3 = base.uvOffset;
						float x2 = num12 + uvOffset3.x;
						float num13 = num5 * num6;
						Vector2 uvScale7 = base.uvScale;
						float num14 = num13 * uvScale7.y;
						Vector2 uvOffset4 = base.uvOffset;
						reference3 = new Vector2(x2, num14 + uvOffset4.y);
						break;
					}
					case UVWrapMode.Uniform:
					{
						ref Vector2 reference2 = ref tsMesh.uv[num];
						float num7 = num4;
						Vector2 uvScale4 = base.uvScale;
						float num8 = num7 * uvScale4.x;
						Vector2 uvOffset = base.uvOffset;
						float x = num8 + uvOffset.x;
						float num9 = num5 * num6;
						Vector2 uvScale5 = base.uvScale;
						float num10 = num9 * uvScale5.y;
						Vector2 uvOffset2 = base.uvOffset;
						reference2 = new Vector2(x, num10 + uvOffset2.y);
						break;
					}
					}
					tsMesh.colors[num] = base.clippedSamples[i].color * base.color;
					num++;
				}
			}
			if (base.clippedSamples.Length > 0)
			{
				num2 /= (float)base.clippedSamples.Length;
			}
			MeshUtility.GeneratePlaneTriangles(ref tsMesh.triangles, _slices, base.clippedSamples.Length, num2 < num3);
		}

		private void GenerateSymmetrical()
		{
			AllocateMesh(base.clippedSamples.Length * (_slices + 1) * 2, _slices * (base.clippedSamples.Length - 1) * 6);
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < base.clippedSamples.Length; i++)
			{
				Vector3 position = base.clippedSamples[i].position;
				Vector3 a = position;
				Vector3 vector = Vector3.right;
				float num4 = 1f;
				switch (_axis)
				{
				case Axis.X:
				{
					Vector3 position8 = base.computer.position;
					float x = position8.x;
					Vector3 position9 = base.computer.position;
					a.x = x + (position9.x - position.x);
					Vector2 uvScale3 = base.uvScale;
					num4 = uvScale3.y * Mathf.Abs(position.x - a.x);
					num2 += position.x;
					Vector3 position10 = base.computer.position;
					num3 = position10.x;
					break;
				}
				case Axis.Y:
				{
					Vector3 position5 = base.computer.position;
					float y = position5.y;
					Vector3 position6 = base.computer.position;
					a.y = y + (position6.y - position.y);
					Vector2 uvScale2 = base.uvScale;
					num4 = uvScale2.y * Mathf.Abs(position.y - a.y);
					vector = Vector3.up;
					num2 += position.y;
					Vector3 position7 = base.computer.position;
					num3 = position7.y;
					break;
				}
				case Axis.Z:
				{
					Vector3 position2 = base.computer.position;
					float z = position2.z;
					Vector3 position3 = base.computer.position;
					a.z = z + (position3.z - position.z);
					Vector2 uvScale = base.uvScale;
					num4 = uvScale.y * Mathf.Abs(position.z - a.z);
					vector = Vector3.forward;
					num2 += position.z;
					Vector3 position4 = base.computer.position;
					num3 = position4.z;
					break;
				}
				}
				Vector3 normalized = Vector3.Cross(vector, base.clippedSamples[i].direction).normalized;
				Vector3 vector2 = Vector3.Cross(base.clippedSamples[i].normal, base.clippedSamples[i].direction);
				for (int j = 0; j < _slices * 2; j++)
				{
					float num5 = (float)j / (float)_slices;
					ref Vector3 reference = ref tsMesh.vertices[num];
					Vector3 a2 = Vector3.Lerp(a, position, num5);
					Vector3 a3 = vector;
					Vector3 offset = base.offset;
					Vector3 a4 = a2 + a3 * offset.y;
					Vector3 a5 = vector2;
					Vector3 offset2 = base.offset;
					reference = a4 + a5 * offset2.x;
					tsMesh.normals[num] = normalized;
					if (_uvWrapMode == UVWrapMode.Clamp)
					{
						ref Vector2 reference2 = ref tsMesh.uv[num];
						float num6 = (float)base.clippedSamples[i].percent;
						Vector2 uvScale4 = base.uvScale;
						float num7 = num6 * uvScale4.x;
						Vector2 uvOffset = base.uvOffset;
						float x2 = num7 + uvOffset.x;
						float num8 = num5;
						Vector2 uvScale5 = base.uvScale;
						float num9 = num8 * uvScale5.y;
						Vector2 uvOffset2 = base.uvOffset;
						reference2 = new Vector2(x2, num9 + uvOffset2.y);
					}
					else
					{
						ref Vector2 reference3 = ref tsMesh.uv[num];
						float num10 = (float)base.clippedSamples[i].percent;
						Vector2 uvScale6 = base.uvScale;
						float num11 = num10 * uvScale6.x;
						Vector2 uvOffset3 = base.uvOffset;
						float x3 = num11 + uvOffset3.x;
						float num12 = 0.5f - 0.5f * num4 + num4 * num5;
						Vector2 uvScale7 = base.uvScale;
						float num13 = num12 * uvScale7.y;
						Vector2 uvOffset4 = base.uvOffset;
						reference3 = new Vector2(x3, num13 + uvOffset4.y);
					}
					tsMesh.colors[num] = base.clippedSamples[i].color * base.color;
					num++;
				}
			}
			if (base.clippedSamples.Length > 0)
			{
				num2 /= (float)base.clippedSamples.Length;
			}
			MeshUtility.GeneratePlaneTriangles(ref tsMesh.triangles, _slices * 2 - 1, base.clippedSamples.Length, num2 * 2f < num3);
		}
	}
}
