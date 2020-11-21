using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Spline Positioner")]
	public class SplinePositioner : SplineTracer
	{
		public enum Mode
		{
			Percent,
			Distance
		}

		[SerializeField]
		[HideInInspector]
		private Transform _applyTransform;

		[SerializeField]
		[HideInInspector]
		private GameObject _targetObject;

		[SerializeField]
		[HideInInspector]
		private double _position;

		[SerializeField]
		[HideInInspector]
		private float animPosition;

		[SerializeField]
		[HideInInspector]
		private Mode _mode;

		[Obsolete("Deprecated in 1.0.8. Use targetObject instead")]
		public Transform applyTransform
		{
			get
			{
				return targetObject.transform;
			}
			set
			{
				if (value != null)
				{
					targetObject = value.gameObject;
				}
				else
				{
					targetObject = null;
				}
			}
		}

		public GameObject targetObject
		{
			get
			{
				if (_targetObject == null)
				{
					if (_applyTransform != null)
					{
						_targetObject = _applyTransform.gameObject;
						_applyTransform = null;
						return _targetObject;
					}
					return base.gameObject;
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

		public double position
		{
			get
			{
				return _position;
			}
			set
			{
				if (value != _position)
				{
					animPosition = (float)value;
					_position = value;
					if (mode == Mode.Distance)
					{
						SetDistance((float)_position, checkTriggers: true);
					}
					else
					{
						SetPercent(_position, checkTriggers: true);
					}
				}
			}
		}

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

		[Obsolete("Deprecated in 1.0.8. Use result instead")]
		public SplineResult positionResult => _result;

		[Obsolete("Deprecated in 1.0.8. Use offsettedResult instead")]
		public SplineResult offsettedPositionResult => base.offsettedResult;

		protected override void OnDidApplyAnimationProperties()
		{
			if ((double)animPosition != _position)
			{
				position = animPosition;
			}
			base.OnDidApplyAnimationProperties();
		}

		protected override Transform GetTransform()
		{
			return targetObject.transform;
		}

		protected override Rigidbody GetRigidbody()
		{
			return targetObject.GetComponent<Rigidbody>();
		}

		protected override Rigidbody2D GetRigidbody2D()
		{
			return targetObject.GetComponent<Rigidbody2D>();
		}

		protected override void PostBuild()
		{
			base.PostBuild();
			if (mode == Mode.Distance)
			{
				SetDistance((float)_position, checkTriggers: true);
			}
			else
			{
				SetPercent(_position, checkTriggers: true);
			}
		}

		public override void SetPercent(double percent, bool checkTriggers = false)
		{
			base.SetPercent(percent, checkTriggers);
			_position = percent;
		}

		public override void SetDistance(float distance, bool checkTriggers = false)
		{
			base.SetDistance(distance, checkTriggers);
			_position = distance;
		}
	}
}
