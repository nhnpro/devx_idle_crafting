using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	[Serializable]
	public class TransformModule
	{
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("offset")]
		private Vector2 _offset;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("rotationOffset")]
		private Vector3 _rotationOffset = Vector3.zero;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("baseScale")]
		private Vector3 _baseScale = Vector3.one;

		private SplineResult _splineResult;

		public CustomRotationModule customRotation;

		public CustomOffsetModule customOffset;

		public bool applyPositionX = true;

		public bool applyPositionY = true;

		public bool applyPositionZ = true;

		public Spline.Direction direction = Spline.Direction.Forward;

		public bool applyRotationX = true;

		public bool applyRotationY = true;

		public bool applyRotationZ = true;

		public bool applyScaleX;

		public bool applyScaleY;

		public bool applyScaleZ;

		[HideInInspector]
		public SplineUser targetUser;

		private static Vector3 position = Vector3.zero;

		private static Quaternion rotation = Quaternion.identity;

		public Vector2 offset
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
					if (targetUser != null)
					{
						targetUser.Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public Vector3 rotationOffset
		{
			get
			{
				return _rotationOffset;
			}
			set
			{
				if (value != _rotationOffset)
				{
					_rotationOffset = value;
					if (targetUser != null)
					{
						targetUser.Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public Vector3 baseScale
		{
			get
			{
				return _baseScale;
			}
			set
			{
				if (value != _baseScale)
				{
					_baseScale = value;
					if (targetUser != null)
					{
						targetUser.Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public SplineResult splineResult
		{
			get
			{
				if (_splineResult == null)
				{
					_splineResult = new SplineResult();
				}
				return _splineResult;
			}
			set
			{
				if (_splineResult == null)
				{
					_splineResult = new SplineResult(value);
				}
				else
				{
					_splineResult.CopyFrom(value);
				}
			}
		}

		public bool applyPosition
		{
			get
			{
				return applyPositionX || applyPositionY || applyPositionZ;
			}
			set
			{
				applyPositionX = (applyPositionY = (applyPositionZ = value));
			}
		}

		public bool applyRotation
		{
			get
			{
				return applyRotationX || applyRotationY || applyRotationZ;
			}
			set
			{
				applyRotationX = (applyRotationY = (applyRotationZ = value));
			}
		}

		public bool applyScale
		{
			get
			{
				return applyScaleX || applyScaleY || applyScaleZ;
			}
			set
			{
				applyScaleX = (applyScaleY = (applyScaleZ = value));
			}
		}

		public void ApplyTransform(Transform input)
		{
			input.position = GetPosition(input.position);
			input.rotation = GetRotation(input.rotation);
			input.localScale = GetScale(input.localScale);
		}

		public void ApplyRigidbody(Rigidbody input)
		{
			input.transform.localScale = GetScale(input.transform.localScale);
			input.MovePosition(GetPosition(input.position));
			Vector3 vector = input.velocity;
			if (applyPositionX)
			{
				vector.x = 0f;
			}
			if (applyPositionY)
			{
				vector.y = 0f;
			}
			if (applyPositionZ)
			{
				vector.z = 0f;
			}
			input.velocity = vector;
			input.MoveRotation(GetRotation(input.rotation));
			vector = input.angularVelocity;
			if (applyRotationX)
			{
				vector.x = 0f;
			}
			if (applyRotationY)
			{
				vector.y = 0f;
			}
			if (applyRotationZ)
			{
				vector.z = 0f;
			}
			input.angularVelocity = vector;
		}

		public void ApplyRigidbody2D(Rigidbody2D input)
		{
			input.transform.localScale = GetScale(input.transform.localScale);
			input.position = GetPosition(input.position);
			Vector2 velocity = input.velocity;
			if (applyPositionX)
			{
				velocity.x = 0f;
			}
			if (applyPositionY)
			{
				velocity.y = 0f;
			}
			input.velocity = velocity;
			Vector3 eulerAngles = GetRotation(Quaternion.Euler(0f, 0f, input.rotation)).eulerAngles;
			input.rotation = 0f - eulerAngles.z;
			if (applyRotationX)
			{
				input.angularVelocity = 0f;
			}
		}

		private Vector3 GetPosition(Vector3 inputPosition)
		{
			position = _splineResult.position;
			Vector2 vector = _offset;
			if (customOffset != null)
			{
				vector += customOffset.Evaluate(_splineResult.percent);
			}
			if (vector != Vector2.zero)
			{
				position += _splineResult.right * vector.x * _splineResult.size + _splineResult.normal * vector.y * _splineResult.size;
			}
			if (applyPositionX)
			{
				inputPosition.x = position.x;
			}
			if (applyPositionY)
			{
				inputPosition.y = position.y;
			}
			if (applyPositionZ)
			{
				inputPosition.z = position.z;
			}
			return inputPosition;
		}

		private Quaternion GetRotation(Quaternion inputRotation)
		{
			Vector3 a = _splineResult.direction;
			if (direction == Spline.Direction.Backward && !targetUser.averageResultVectors)
			{
				double num = targetUser.ClipPercent(_splineResult.percent);
				int num2 = targetUser.GetClippedSampleIndex(num);
				for (int num3 = DMath.CeilInt(num * (double)(targetUser.clippedSamples.Length - 1)); num3 >= 0; num3--)
				{
					num2 = num3;
					if (targetUser.ClipPercent(targetUser.clippedSamples[num3].percent) < num)
					{
						break;
					}
				}
				int num4 = num2;
				SplineResult splineResult = targetUser.clippedSamples[num4];
				num4--;
				if (num4 < 0)
				{
					num4 = ((targetUser.span == 1.0 && targetUser.rootUser.computer.isClosed) ? Mathf.Max(targetUser.clippedSamples.Length - 2, 0) : 0);
				}
				SplineResult splineResult2 = targetUser.clippedSamples[num4];
				a = Vector3.Slerp(splineResult.direction, splineResult2.direction, (float)DMath.InverseLerp(targetUser.ClipPercent(targetUser.clippedSamples[num2 + 1].percent), targetUser.ClipPercent(targetUser.clippedSamples[num2].percent), targetUser.ClipPercent(_splineResult.percent)));
			}
			rotation = Quaternion.LookRotation(a * ((direction != Spline.Direction.Forward) ? (-1f) : 1f), _splineResult.normal);
			if (_rotationOffset != Vector3.zero)
			{
				rotation *= Quaternion.Euler(_rotationOffset);
			}
			if (customRotation != null)
			{
				rotation = customRotation.Evaluate(rotation, _splineResult.percent);
			}
			if (!applyRotationX || !applyRotationY)
			{
				Vector3 eulerAngles = rotation.eulerAngles;
				if (!applyRotationX)
				{
					Vector3 eulerAngles2 = inputRotation.eulerAngles;
					eulerAngles.x = eulerAngles2.x;
				}
				if (!applyRotationY)
				{
					Vector3 eulerAngles3 = inputRotation.eulerAngles;
					eulerAngles.y = eulerAngles3.y;
				}
				if (!applyRotationZ)
				{
					Vector3 eulerAngles4 = inputRotation.eulerAngles;
					eulerAngles.z = eulerAngles4.z;
				}
				inputRotation.eulerAngles = eulerAngles;
			}
			else
			{
				inputRotation = rotation;
			}
			return inputRotation;
		}

		private Vector3 GetScale(Vector3 inputScale)
		{
			if (applyScaleX)
			{
				inputScale.x = _baseScale.x * _splineResult.size;
			}
			if (applyScaleY)
			{
				inputScale.y = _baseScale.y * _splineResult.size;
			}
			if (applyScaleZ)
			{
				inputScale.z = _baseScale.z * _splineResult.size;
			}
			return inputScale;
		}
	}
}
