using System;
using UnityEngine;

namespace Dreamteck
{
	[Serializable]
	public class TS_Transform
	{
		private bool setPosition;

		private bool setRotation;

		private bool setScale;

		private bool setLocalPosition;

		private bool setLocalRotation;

		[SerializeField]
		[HideInInspector]
		private Transform _transform;

		[SerializeField]
		[HideInInspector]
		private volatile float posX;

		[SerializeField]
		[HideInInspector]
		private volatile float posY;

		[SerializeField]
		[HideInInspector]
		private volatile float posZ;

		[SerializeField]
		[HideInInspector]
		private volatile float scaleX = 1f;

		[SerializeField]
		[HideInInspector]
		private volatile float scaleY = 1f;

		[SerializeField]
		[HideInInspector]
		private volatile float scaleZ = 1f;

		[SerializeField]
		[HideInInspector]
		private volatile float lossyScaleX = 1f;

		[SerializeField]
		[HideInInspector]
		private volatile float lossyScaleY = 1f;

		[SerializeField]
		[HideInInspector]
		private volatile float lossyScaleZ = 1f;

		[SerializeField]
		[HideInInspector]
		private volatile float rotX;

		[SerializeField]
		[HideInInspector]
		private volatile float rotY;

		[SerializeField]
		[HideInInspector]
		private volatile float rotZ;

		[SerializeField]
		[HideInInspector]
		private volatile float rotW;

		[SerializeField]
		[HideInInspector]
		private volatile float lposX;

		[SerializeField]
		[HideInInspector]
		private volatile float lposY;

		[SerializeField]
		[HideInInspector]
		private volatile float lposZ;

		[SerializeField]
		[HideInInspector]
		private volatile float lrotX;

		[SerializeField]
		[HideInInspector]
		private volatile float lrotY;

		[SerializeField]
		[HideInInspector]
		private volatile float lrotZ;

		[SerializeField]
		[HideInInspector]
		private volatile float lrotW;

		public Vector3 position
		{
			get
			{
				return new Vector3(posX, posY, posZ);
			}
			set
			{
				setPosition = true;
				setLocalPosition = false;
				posX = value.x;
				posY = value.y;
				posZ = value.z;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return new Quaternion(rotX, rotY, rotZ, rotW);
			}
			set
			{
				setRotation = true;
				setLocalRotation = false;
				rotX = value.x;
				rotY = value.y;
				rotZ = value.z;
				rotW = value.w;
			}
		}

		public Vector3 scale
		{
			get
			{
				return new Vector3(scaleX, scaleY, scaleZ);
			}
			set
			{
				setScale = true;
				scaleX = value.x;
				scaleY = value.y;
				scaleZ = value.z;
			}
		}

		public Vector3 lossyScale
		{
			get
			{
				return new Vector3(lossyScaleX, lossyScaleY, lossyScaleZ);
			}
			set
			{
				setScale = true;
				lossyScaleX = value.x;
				lossyScaleY = value.y;
				lossyScaleZ = value.z;
			}
		}

		public Vector3 localPosition
		{
			get
			{
				return new Vector3(lposX, lposY, lposZ);
			}
			set
			{
				setLocalPosition = true;
				setPosition = false;
				lposX = value.x;
				lposY = value.y;
				lposZ = value.z;
			}
		}

		public Quaternion localRotation
		{
			get
			{
				return new Quaternion(lrotX, lrotY, lrotZ, lrotW);
			}
			set
			{
				setLocalRotation = true;
				setRotation = false;
				lrotX = value.x;
				lrotY = value.y;
				lrotZ = value.z;
				lrotW = value.w;
			}
		}

		public Transform transform => _transform;

		public TS_Transform(Transform input)
		{
			SetTransform(input);
		}

		public void Update()
		{
			if (!(transform == null))
			{
				if (setPosition)
				{
					_transform.position = position;
				}
				else if (setLocalPosition)
				{
					_transform.localPosition = localPosition;
				}
				else
				{
					position = _transform.position;
					localPosition = _transform.localPosition;
				}
				if (setScale)
				{
					_transform.localScale = scale;
				}
				else
				{
					scale = _transform.localScale;
				}
				lossyScale = _transform.lossyScale;
				if (setRotation)
				{
					_transform.rotation = rotation;
				}
				else if (setLocalRotation)
				{
					_transform.localRotation = localRotation;
				}
				else
				{
					rotation = _transform.rotation;
					localRotation = _transform.localRotation;
				}
				setPosition = (setLocalPosition = (setRotation = (setLocalRotation = (setScale = false))));
			}
		}

		public void SetTransform(Transform input)
		{
			_transform = input;
			setPosition = (setLocalPosition = (setRotation = (setLocalRotation = (setScale = false))));
			Update();
		}

		public bool HasChange()
		{
			return HasPositionChange() || HasRotationChange() || HasScaleChange();
		}

		public bool HasPositionChange()
		{
			float num = posX;
			Vector3 position = _transform.position;
			int result;
			if (num == position.x)
			{
				float num2 = posY;
				Vector3 position2 = _transform.position;
				if (num2 == position2.y)
				{
					float num3 = posZ;
					Vector3 position3 = _transform.position;
					result = ((num3 != position3.z) ? 1 : 0);
					goto IL_0063;
				}
			}
			result = 1;
			goto IL_0063;
			IL_0063:
			return (byte)result != 0;
		}

		public bool HasRotationChange()
		{
			float num = rotX;
			Quaternion rotation = _transform.rotation;
			int result;
			if (num == rotation.x)
			{
				float num2 = rotY;
				Quaternion rotation2 = _transform.rotation;
				if (num2 == rotation2.y)
				{
					float num3 = rotZ;
					Quaternion rotation3 = _transform.rotation;
					if (num3 == rotation3.z)
					{
						float num4 = rotW;
						Quaternion rotation4 = _transform.rotation;
						result = ((num4 != rotation4.w) ? 1 : 0);
						goto IL_0083;
					}
				}
			}
			result = 1;
			goto IL_0083;
			IL_0083:
			return (byte)result != 0;
		}

		public bool HasScaleChange()
		{
			float num = lossyScaleX;
			Vector3 lossyScale = _transform.lossyScale;
			int result;
			if (num == lossyScale.x)
			{
				float num2 = lossyScaleY;
				Vector3 lossyScale2 = _transform.lossyScale;
				if (num2 == lossyScale2.y)
				{
					float num3 = lossyScaleZ;
					Vector3 lossyScale3 = _transform.lossyScale;
					result = ((num3 != lossyScale3.z) ? 1 : 0);
					goto IL_0063;
				}
			}
			result = 1;
			goto IL_0063;
			IL_0063:
			return (byte)result != 0;
		}

		public Vector3 TransformPoint(Vector3 point)
		{
			Vector3 point2 = new Vector3(point.x * lossyScaleX, point.y * lossyScaleY, point.z * lossyScaleZ);
			Vector3 b = rotation * point2;
			return position + b;
		}

		public Vector3 TransformDirection(Vector3 direction)
		{
			return TransformPoint(direction) - position;
		}

		public Vector3 InverseTransformPoint(Vector3 point)
		{
			return InverseTransformDirection(point - position);
		}

		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			Vector3 vector = Quaternion.Inverse(rotation) * direction;
			return new Vector3(vector.x / lossyScaleX, vector.y / lossyScaleY, vector.z / lossyScaleZ);
		}

		public T GetComponent<T>()
		{
			return _transform.GetComponent<T>();
		}
	}
}
