using UnityEngine;

namespace Dreamteck.Splines
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Spline Renderer")]
	[ExecuteInEditMode]
	public class SplineRenderer : MeshGenerator
	{
		[HideInInspector]
		public bool autoOrient = true;

		[HideInInspector]
		public int updateFrameInterval;

		private int currentFrame;

		[SerializeField]
		[HideInInspector]
		private int _slices = 1;

		[SerializeField]
		[HideInInspector]
		private Vector3 vertexDirection = Vector3.up;

		private bool orthographic;

		private bool init;

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
			mesh.name = "spline";
		}

		private void Start()
		{
			if (Camera.current != null)
			{
				orthographic = Camera.current.orthographic;
			}
		}

		protected override void LateRun()
		{
			if (updateFrameInterval > 0)
			{
				currentFrame++;
				if (currentFrame > updateFrameInterval)
				{
					currentFrame = 0;
				}
			}
		}

		protected override void BuildMesh()
		{
			base.BuildMesh();
			GenerateVertices(vertexDirection, orthographic);
			MeshUtility.GeneratePlaneTriangles(ref tsMesh.triangles, _slices, base.clippedSamples.Length, flip: false);
		}

		public void RenderWithCamera(Camera cam)
		{
			if (base.samples.Length == 0)
			{
				return;
			}
			if (cam != null)
			{
				if (cam.orthographic)
				{
					vertexDirection = -cam.transform.forward;
				}
				else
				{
					vertexDirection = cam.transform.position;
				}
			}
			orthographic = cam.orthographic;
			BuildMesh();
			WriteMesh();
		}

		private void OnWillRenderObject()
		{
			if (autoOrient && (updateFrameInterval <= 0 || currentFrame == 0))
			{
				if (!Application.isPlaying && !init)
				{
					Awake();
					init = true;
				}
				RenderWithCamera(Camera.current);
			}
		}

		public void GenerateVertices(Vector3 vertexDirection, bool orthoGraphic)
		{
			AllocateMesh((_slices + 1) * base.clippedSamples.Length, _slices * (base.clippedSamples.Length - 1) * 6);
			int num = 0;
			ResetUVDistance();
			for (int i = 0; i < base.clippedSamples.Length; i++)
			{
				Vector3 vector = base.clippedSamples[i].position;
				if (base.offset != Vector3.zero)
				{
					Vector3 a = vector;
					Vector3 offset = base.offset;
					Vector3 a2 = offset.x * -Vector3.Cross(base.clippedSamples[i].direction, base.clippedSamples[i].normal);
					Vector3 offset2 = base.offset;
					Vector3 a3 = a2 + offset2.y * base.clippedSamples[i].normal;
					Vector3 offset3 = base.offset;
					vector = a + (a3 + offset3.z * base.clippedSamples[i].direction);
				}
				Vector3 vector2 = (!orthoGraphic) ? (vertexDirection - vector).normalized : vertexDirection;
				Vector3 normalized = Vector3.Cross(base.clippedSamples[i].direction, vector2).normalized;
				if (base.uvMode == UVMode.UniformClamp || base.uvMode == UVMode.UniformClip)
				{
					AddUVDistance(i);
				}
				for (int j = 0; j < _slices + 1; j++)
				{
					float num2 = (float)j / (float)_slices;
					tsMesh.vertices[num] = vector - normalized * base.clippedSamples[i].size * 0.5f * base.size + normalized * base.clippedSamples[i].size * num2 * base.size;
					CalculateUVs(base.clippedSamples[i].percent, num2);
					tsMesh.uv[num] = Vector2.one * 0.5f + (Vector2)(Quaternion.AngleAxis(base.uvRotation, Vector3.forward) * (Vector2.one * 0.5f - MeshGenerator.uvs));
					tsMesh.normals[num] = vector2;
					tsMesh.colors[num] = base.clippedSamples[i].color * base.color;
					num++;
				}
			}
		}
	}
}
