using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[Serializable]
	public class CustomRotationModule
	{
		[Serializable]
		public class Key
		{
			[SerializeField]
			private Vector3 _rotation = Vector3.zero;

			[SerializeField]
			private double _from;

			[SerializeField]
			private double _to;

			[SerializeField]
			private double _center;

			[SerializeField]
			private bool _loop;

			public AnimationCurve interpolation;

			public Vector3 rotation
			{
				get
				{
					return _rotation;
				}
				set
				{
					_rotation = value;
				}
			}

			public double center
			{
				get
				{
					return _center;
				}
				set
				{
					_center = DMath.Clamp01(value);
				}
			}

			public double from
			{
				get
				{
					return _from;
				}
				set
				{
					_from = DMath.Clamp01(value);
				}
			}

			public double to
			{
				get
				{
					return _to;
				}
				set
				{
					_to = DMath.Clamp01(value);
				}
			}

			public bool loop
			{
				get
				{
					return _loop;
				}
				set
				{
					_loop = value;
				}
			}

			public double position
			{
				get
				{
					if (from > to)
					{
						double num = DMath.Lerp(_from, _to, _center);
						double num2 = 1.0 - _from;
						double num3 = _center * (num2 + _to);
						num = _from + num3;
						if (num > 1.0)
						{
							num -= 1.0;
						}
						return num;
					}
					return DMath.Lerp(_from, _to, _center);
				}
				set
				{
					double num = value - position;
					from += num;
					to += num;
					center = DMath.InverseLerp(from, to, value);
				}
			}

			public Key(Vector3 r, double f, double t, double c)
			{
				_rotation = r;
				from = f;
				to = t;
				center = c;
				interpolation = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			}

			public float Evaluate(float t)
			{
				return interpolation.Evaluate(t);
			}
		}

		public List<Key> keys = new List<Key>();

		[SerializeField]
		private float _blend = 1f;

		public float blend
		{
			get
			{
				return _blend;
			}
			set
			{
				_blend = Mathf.Clamp01(value);
			}
		}

		public CustomRotationModule()
		{
			keys = new List<Key>();
		}

		public void AddKey(Vector3 rotation, double f, double t, double c)
		{
			keys.Add(new Key(rotation, f, t, c));
		}

		public Quaternion Evaluate(Quaternion baseRotation, double time)
		{
			if (keys.Count == 0)
			{
				return baseRotation;
			}
			for (int i = 0; i < keys.Count; i++)
			{
				double position = keys[i].position;
				float num = 0f;
				num = ((!(keys[i].from > keys[i].to)) ? ((!(time < position)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].to, position, time))) * _blend) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].from, position, time))) * _blend)) : ((!(position >= keys[i].from)) ? ((!(time > keys[i].from)) ? ((!(time <= position)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].to, position, time))) * _blend) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(0.0 - (1.0 - keys[i].from), position, time))) * _blend)) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].from, 1.0 + position, time))) * _blend)) : ((!(time > keys[i].from)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].to, 0.0 - (1.0 - position), time))) * _blend) : ((!(time <= position)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(1.0 + keys[i].to, position, time))) * _blend) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].from, position, time))) * _blend)))));
				Vector3 rotation = keys[i].rotation;
				float x = rotation.x;
				Vector3 rotation2 = keys[i].rotation;
				float y = rotation2.y;
				Vector3 rotation3 = keys[i].rotation;
				Quaternion rhs = Quaternion.Euler(x, y, rotation3.z);
				baseRotation = Quaternion.Slerp(baseRotation, baseRotation * rhs, num);
			}
			return baseRotation;
		}
	}
}
