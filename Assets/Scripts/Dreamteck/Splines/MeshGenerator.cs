using UnityEngine;

namespace Dreamteck.Splines
{
	public class MeshGenerator : SplineUser
	{
		public enum UVMode
		{
			Clip,
			UniformClip,
			Clamp,
			UniformClamp
		}

		[SerializeField]
		[HideInInspector]
		private bool _baked;

		[SerializeField]
		[HideInInspector]
		private float _size = 1f;

		[SerializeField]
		[HideInInspector]
		private Color _color = Color.white;

		[SerializeField]
		[HideInInspector]
		private Vector3 _offset = Vector3.zero;

		[SerializeField]
		[HideInInspector]
		private int _normalMethod = 1;

		[SerializeField]
		[HideInInspector]
		private bool _tangents = true;

		[SerializeField]
		[HideInInspector]
		private float _rotation;

		[SerializeField]
		[HideInInspector]
		private bool _flipFaces;

		[SerializeField]
		[HideInInspector]
		private bool _doubleSided;

		[SerializeField]
		[HideInInspector]
		private UVMode _uvMode;

		[SerializeField]
		[HideInInspector]
		private Vector2 _uvScale = Vector2.one;

		[SerializeField]
		[HideInInspector]
		private Vector2 _uvOffset = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private float _uvRotation;

		[SerializeField]
		[HideInInspector]
		protected MeshCollider meshCollider;

		[SerializeField]
		[HideInInspector]
		protected MeshFilter filter;

		[SerializeField]
		[HideInInspector]
		protected MeshRenderer meshRenderer;

		[SerializeField]
		[HideInInspector]
		protected TS_Mesh tsMesh = new TS_Mesh();

		[SerializeField]
		[HideInInspector]
		protected Mesh mesh;

		[HideInInspector]
		public float colliderUpdateRate = 0.2f;

		protected bool updateCollider;

		protected float lastUpdateTime;

		private float vDist;

		protected static Vector2 uvs = Vector2.zero;

		public float size
		{
			get
			{
				return _size;
			}
			set
			{
				if (value != _size)
				{
					_size = value;
					Rebuild(sampleComputer: false);
				}
				else
				{
					_size = value;
				}
			}
		}

		public Color color
		{
			get
			{
				return _color;
			}
			set
			{
				if (value != _color)
				{
					_color = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public Vector3 offset
		{
			get
			{
				return _offset;
			}
			set
			{
				if (value != _offset)
				{
					_offset = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public int normalMethod
		{
			get
			{
				return _normalMethod;
			}
			set
			{
				if (value != _normalMethod)
				{
					_normalMethod = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool calculateTangents
		{
			get
			{
				return _tangents;
			}
			set
			{
				if (value != _tangents)
				{
					_tangents = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public float rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				if (value != _rotation)
				{
					_rotation = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool flipFaces
		{
			get
			{
				return _flipFaces;
			}
			set
			{
				if (value != _flipFaces)
				{
					_flipFaces = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool doubleSided
		{
			get
			{
				return _doubleSided;
			}
			set
			{
				if (value != _doubleSided)
				{
					_doubleSided = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public UVMode uvMode
		{
			get
			{
				return _uvMode;
			}
			set
			{
				if (value != _uvMode)
				{
					_uvMode = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public Vector2 uvScale
		{
			get
			{
				return _uvScale;
			}
			set
			{
				if (value != _uvScale)
				{
					_uvScale = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public Vector2 uvOffset
		{
			get
			{
				return _uvOffset;
			}
			set
			{
				if (value != _uvOffset)
				{
					_uvOffset = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public float uvRotation
		{
			get
			{
				return _uvRotation;
			}
			set
			{
				if (value != _uvRotation)
				{
					_uvRotation = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool baked => _baked;

		protected override void Awake()
		{
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			base.Awake();
			filter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			meshCollider = GetComponent<MeshCollider>();
		}

		protected override void Reset()
		{
			base.Reset();
			MeshFilter component = GetComponent<MeshFilter>();
			if (component != null)
			{
				component.hideFlags = HideFlags.HideInInspector;
			}
			MeshRenderer component2 = GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				component2.hideFlags = HideFlags.None;
			}
			bool flag = false;
			int num = 0;
			while (true)
			{
				if (num < component2.sharedMaterials.Length)
				{
					if (component2.sharedMaterials[num] != null)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			flag = true;
		}

		public void CloneMesh()
		{
			if (tsMesh != null)
			{
				tsMesh = TS_Mesh.Copy(tsMesh);
			}
			else
			{
				tsMesh = new TS_Mesh();
			}
			if (mesh != null)
			{
				mesh = Object.Instantiate(mesh);
			}
			else
			{
				mesh = new Mesh();
			}
		}

		public override void Rebuild(bool sampleComputer)
		{
			if (!_baked)
			{
				base.Rebuild(sampleComputer);
			}
		}

		public override void RebuildImmediate(bool sampleComputer)
		{
			if (!_baked)
			{
				base.RebuildImmediate(sampleComputer);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			MeshFilter component = GetComponent<MeshFilter>();
			MeshRenderer component2 = GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.hideFlags = HideFlags.None;
			}
			if (component2 != null)
			{
				component2.hideFlags = HideFlags.None;
			}
		}

		public void UpdateCollider()
		{
			meshCollider = GetComponent<MeshCollider>();
			if (meshCollider == null)
			{
				meshCollider = base.gameObject.AddComponent<MeshCollider>();
			}
			meshCollider.sharedMesh = filter.sharedMesh;
		}

		protected override void LateRun()
		{
			if (!_baked)
			{
				base.LateRun();
				if (updateCollider && meshCollider != null && Time.time - lastUpdateTime >= colliderUpdateRate)
				{
					lastUpdateTime = Time.time;
					updateCollider = false;
					meshCollider.sharedMesh = filter.sharedMesh;
				}
			}
		}

		protected override void Build()
		{
			base.Build();
			if (base.samples.Length > 0)
			{
				BuildMesh();
			}
		}

		protected override void PostBuild()
		{
			base.PostBuild();
			WriteMesh();
		}

		protected virtual void BuildMesh()
		{
		}

		protected virtual void WriteMesh()
		{
			MeshUtility.InverseTransformMesh(tsMesh, base.transform);
			if (_doubleSided)
			{
				MeshUtility.MakeDoublesidedHalf(tsMesh);
			}
			else if (_flipFaces)
			{
				MeshUtility.FlipFaces(tsMesh);
			}
			if (_tangents)
			{
				MeshUtility.CalculateTangents(tsMesh);
			}
			tsMesh.WriteMesh(ref mesh);
			if (_normalMethod == 0)
			{
				mesh.RecalculateNormals();
			}
			if (filter != null)
			{
				filter.sharedMesh = mesh;
			}
			updateCollider = true;
		}

		protected virtual void AllocateMesh(int vertexCount, int trisCount)
		{
			if (_doubleSided)
			{
				vertexCount *= 2;
				trisCount *= 2;
			}
			if (tsMesh.vertexCount != vertexCount)
			{
				tsMesh.vertices = new Vector3[vertexCount];
				tsMesh.normals = new Vector3[vertexCount];
				tsMesh.colors = new Color[vertexCount];
				tsMesh.uv = new Vector2[vertexCount];
			}
			if (tsMesh.triangles.Length != trisCount)
			{
				tsMesh.triangles = new int[trisCount];
			}
		}

		protected void ResetUVDistance()
		{
			vDist = 0f;
			if (uvMode == UVMode.UniformClip)
			{
				vDist = CalculateLength(0.0, base.clippedSamples[0].percent);
			}
		}

		protected void AddUVDistance(int sampleIndex)
		{
			if (sampleIndex != 0)
			{
				vDist += Vector3.Distance(base.clippedSamples[sampleIndex].position, base.clippedSamples[sampleIndex - 1].position);
			}
		}

		protected void CalculateUVs(double percent, float u)
		{
			uvs.x = u * _uvScale.x - _uvOffset.x;
			switch (uvMode)
			{
			case UVMode.Clip:
				uvs.y = (float)percent * _uvScale.y - _uvOffset.y;
				break;
			case UVMode.Clamp:
				uvs.y = (float)DMath.InverseLerp(base.clipFrom, base.clipTo, percent) * _uvScale.y - _uvOffset.y;
				break;
			case UVMode.UniformClamp:
				uvs.y = vDist * _uvScale.y / (float)base.span - _uvOffset.y;
				break;
			default:
				uvs.y = vDist * _uvScale.y - _uvOffset.y;
				break;
			}
		}
	}
}
