using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Spline Projector")]
	public class SplineProjector : SplineTracer
	{
		public enum Mode
		{
			Accurate,
			Cached
		}

		[SerializeField]
		[HideInInspector]
		private Mode _mode;

		[SerializeField]
		[HideInInspector]
		private bool _autoProject = true;

		[SerializeField]
		[HideInInspector]
		private int _subdivide = 3;

		[SerializeField]
		[HideInInspector]
		private Transform _projectTarget;

		[SerializeField]
		[HideInInspector]
		private Transform applyTarget;

		[SerializeField]
		[HideInInspector]
		private GameObject _targetObject;

		[SerializeField]
		[HideInInspector]
		private TS_Transform finalTarget;

		private double traceFromA = -1.0;

		private double traceToA = -1.0;

		private double traceFromB = -1.0;

		[SerializeField]
		[HideInInspector]
		public Vector2 _offset;

		[SerializeField]
		[HideInInspector]
		public Vector3 _rotationOffset = Vector3.zero;

		public Mode mode
		{
			get
			{
				return _mode;
			}
			set
			{
				if (value != _mode)
				{
					_mode = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool autoProject
		{
			get
			{
				return _autoProject;
			}
			set
			{
				if (value != _autoProject)
				{
					_autoProject = value;
					if (_autoProject)
					{
						Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public int subdivide
		{
			get
			{
				return _subdivide;
			}
			set
			{
				if (value != _subdivide)
				{
					_subdivide = value;
					if (_mode == Mode.Accurate)
					{
						Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public Transform projectTarget
		{
			get
			{
				if (_projectTarget == null)
				{
					return base.transform;
				}
				return _projectTarget;
			}
			set
			{
				if (value != _projectTarget)
				{
					_projectTarget = value;
					finalTarget = new TS_Transform(_projectTarget);
					Rebuild(sampleComputer: false);
				}
			}
		}

		[Obsolete("Deprecated in 1.0.8. Use targetObject instead")]
		public Transform target
		{
			get
			{
				return targetObject.transform;
			}
			set
			{
				if (value != applyTarget)
				{
					applyTarget = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public GameObject targetObject
		{
			get
			{
				if (_targetObject == null && applyTarget != null)
				{
					_targetObject = applyTarget.gameObject;
					applyTarget = null;
					return _targetObject;
				}
				return _targetObject;
			}
			set
			{
				if (value != _targetObject)
				{
					_targetObject = value;
					RefreshTargets();
					Rebuild(sampleComputer: false);
				}
			}
		}

		[Obsolete("Deprecated in 1.0.8. Use result instead.")]
		public SplineResult projectResult => base.result;

		public event SplineReachHandler onEndReached;

		public event SplineReachHandler onBeginningReached;

		protected override void Awake()
		{
			base.Awake();
			GetProjectTarget();
		}

		protected override Transform GetTransform()
		{
			if (targetObject == null)
			{
				return null;
			}
			return targetObject.transform;
		}

		protected override Rigidbody GetRigidbody()
		{
			if (targetObject == null)
			{
				return null;
			}
			return targetObject.GetComponent<Rigidbody>();
		}

		protected override Rigidbody2D GetRigidbody2D()
		{
			if (targetObject == null)
			{
				return null;
			}
			return targetObject.GetComponent<Rigidbody2D>();
		}

		private void GetProjectTarget()
		{
			if (_projectTarget != null)
			{
				finalTarget = new TS_Transform(_projectTarget);
			}
			else
			{
				finalTarget = new TS_Transform(base.transform);
			}
		}

		protected override void LateRun()
		{
			base.LateRun();
			if (autoProject)
			{
				if (finalTarget == null)
				{
					GetProjectTarget();
				}
				else if (finalTarget.transform == null)
				{
					GetProjectTarget();
				}
				if (finalTarget.HasPositionChange())
				{
					finalTarget.Update();
					RebuildImmediate(sampleComputer: false);
				}
			}
		}

		protected override void PostBuild()
		{
			base.PostBuild();
			InternalCalculateProjection();
			if (targetObject != null)
			{
				ApplyMotion();
			}
			CheckTriggers();
			InvokeTriggers();
		}

		private void CheckTriggers()
		{
			if (traceFromA >= 0.0)
			{
				if (base.clipTo - traceFromA > traceFromB)
				{
					traceToA = base.clipTo;
					traceFromB = base.clipFrom;
				}
				else
				{
					traceToA = base.clipFrom;
					traceFromB = base.clipTo;
				}
				if (Math.Abs(traceToA - traceFromA) + Math.Abs(base.result.percent - traceFromB) < Math.Abs(base.result.percent - traceFromA))
				{
					CheckTriggers(traceFromA, traceToA);
					CheckTriggers(traceFromB, base.result.percent);
				}
				else
				{
					CheckTriggers(traceFromA, base.result.percent);
				}
			}
		}

		public void CalculateProjection()
		{
			finalTarget.Update();
			Rebuild(sampleComputer: false);
		}

		private void InternalCalculateProjection()
		{
			if (base.computer == null || base.samples.Length == 0)
			{
				_result = new SplineResult();
				return;
			}
			traceFromA = -1.0;
			traceToA = -1.0;
			traceFromB = -1.0;
			double percent = base.result.percent;
			if (_mode == Mode.Accurate)
			{
				double percent2 = _address.Project(finalTarget.position, subdivide, base.clipFrom, base.clipTo);
				if (base.result != null)
				{
					traceFromA = base.result.percent;
				}
				_result = _address.Evaluate(percent2);
			}
			else
			{
				_result = Project(finalTarget.position);
			}
			if (this.onBeginningReached != null && base.result.percent <= base.clipFrom)
			{
				if (!Mathf.Approximately((float)percent, (float)base.result.percent))
				{
					this.onBeginningReached();
				}
			}
			else if (this.onEndReached != null && base.result.percent >= base.clipTo && !Mathf.Approximately((float)percent, (float)base.result.percent))
			{
				this.onEndReached();
			}
		}
	}
}
