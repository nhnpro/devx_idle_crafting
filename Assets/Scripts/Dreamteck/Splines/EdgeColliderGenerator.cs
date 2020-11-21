using UnityEngine;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Edge Collider Generator")]
	[RequireComponent(typeof(EdgeCollider2D))]
	public class EdgeColliderGenerator : SplineUser
	{
		[SerializeField]
		[HideInInspector]
		private float _offset;

		[SerializeField]
		[HideInInspector]
		protected EdgeCollider2D edgeCollider;

		[SerializeField]
		[HideInInspector]
		protected Vector2[] vertices = new Vector2[0];

		[HideInInspector]
		public float updateRate = 0.1f;

		protected float lastUpdateTime;

		private bool updateCollider;

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
			edgeCollider = GetComponent<EdgeCollider2D>();
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
			if (updateCollider && edgeCollider != null && Time.time - lastUpdateTime >= updateRate)
			{
				lastUpdateTime = Time.time;
				updateCollider = false;
				edgeCollider.points = vertices;
			}
		}

		protected override void Build()
		{
			base.Build();
			if (base.clippedSamples.Length == 0)
			{
				return;
			}
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

		protected override void PostBuild()
		{
			base.PostBuild();
			if (!(edgeCollider == null))
			{
				for (int i = 0; i < vertices.Length; i++)
				{
					vertices[i] = base.transform.InverseTransformPoint(vertices[i]);
				}
				if (updateRate == 0f)
				{
					edgeCollider.points = vertices;
				}
				else
				{
					updateCollider = true;
				}
			}
		}
	}
}
