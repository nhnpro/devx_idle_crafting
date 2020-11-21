using UnityEngine;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Polygon Collider Generator")]
	[RequireComponent(typeof(PolygonCollider2D))]
	public class PolygonColliderGenerator : SplineUser
	{
		public enum Type
		{
			Path,
			Shape
		}

		[SerializeField]
		[HideInInspector]
		private Type _type;

		[SerializeField]
		[HideInInspector]
		private float _size = 1f;

		[SerializeField]
		[HideInInspector]
		private float _offset;

		[SerializeField]
		[HideInInspector]
		protected PolygonCollider2D polygonCollider;

		[SerializeField]
		[HideInInspector]
		protected Vector2[] vertices = new Vector2[0];

		[HideInInspector]
		public float updateRate = 0.1f;

		protected float lastUpdateTime;

		private bool updateCollider;

		public Type type
		{
			get
			{
				return _type;
			}
			set
			{
				if (value != _type)
				{
					_type = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

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
			}
		}

		public float offset
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

		protected override void Awake()
		{
			base.Awake();
			polygonCollider = GetComponent<PolygonCollider2D>();
		}

		protected override void Reset()
		{
			base.Reset();
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
		}

		protected override void LateRun()
		{
			base.LateRun();
			if (updateCollider && polygonCollider != null && Time.time - lastUpdateTime >= updateRate)
			{
				lastUpdateTime = Time.time;
				updateCollider = false;
				polygonCollider.SetPath(0, vertices);
			}
		}

		protected override void Build()
		{
			base.Build();
			if (base.clippedSamples.Length != 0)
			{
				switch (type)
				{
				case Type.Path:
					GeneratePath();
					break;
				case Type.Shape:
					GenerateShape();
					break;
				}
			}
		}

		protected override void PostBuild()
		{
			base.PostBuild();
			if (!(polygonCollider == null))
			{
				for (int i = 0; i < vertices.Length; i++)
				{
					vertices[i] = base.transform.InverseTransformPoint(vertices[i]);
				}
				if (updateRate == 0f)
				{
					polygonCollider.SetPath(0, vertices);
				}
				else
				{
					updateCollider = true;
				}
			}
		}

		private void GeneratePath()
		{
			int num = base.clippedSamples.Length * 2;
			if (vertices.Length != num)
			{
				vertices = new Vector2[num];
			}
			for (int i = 0; i < base.clippedSamples.Length; i++)
			{
				Vector2 a = new Vector2(0f - base.clippedSamples[i].direction.y, base.clippedSamples[i].direction.x).normalized * base.clippedSamples[i].size;
				vertices[i] = new Vector2(base.clippedSamples[i].position.x, base.clippedSamples[i].position.y) + a * size * 0.5f + a * offset;
				vertices[base.clippedSamples.Length + (base.clippedSamples.Length - 1) - i] = new Vector2(base.clippedSamples[i].position.x, base.clippedSamples[i].position.y) - a * size * 0.5f + a * offset;
			}
		}

		private void GenerateShape()
		{
			if (vertices.Length != base.clippedSamples.Length)
			{
				vertices = new Vector2[base.clippedSamples.Length];
			}
			for (int i = 0; i < base.clippedSamples.Length; i++)
			{
				vertices[i] = base.clippedSamples[i].position;
				if (offset != 0f)
				{
					Vector2 a = new Vector2(0f - base.clippedSamples[i].direction.y, base.clippedSamples[i].direction.x).normalized * base.clippedSamples[i].size;
					vertices[i] += a * offset;
				}
			}
		}
	}
}
