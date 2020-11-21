using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[Serializable]
	public class CustomOffsetModule
	{
		[Serializable]
		public class Key
		{
			[SerializeField]
			private Vector2 _offset = Vector2.zero;

			[SerializeField]
			private double _from;

			[SerializeField]
			private double _to;

			[SerializeField]
			private double _center;

			[SerializeField]
			private bool _loop;

			public AnimationCurve interpolation;

			public Vector2 offset
			{
				get
				{
					return _offset;
				}
				set
				{
					_offset = value;
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

			public Key(Vector2 o, double f, double t, double c)
			{
				_offset = o;
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

		public CustomOffsetModule()
		{
			keys = new List<Key>();
		}

		public void AddKey(Vector2 offset, double f, double t, double c)
		{
			keys.Add(new Key(offset, f, t, c));
		}

		public Vector2 Evaluate(double time)
		{
			if (keys.Count == 0)
			{
				return Vector2.zero;
			}
			Vector2 vector = Vector2.zero;
			for (int i = 0; i < keys.Count; i++)
			{
				double position = keys[i].position;
				float num = 0f;
				num = ((!(keys[i].from > keys[i].to)) ? ((!(time < position)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].to, position, time))) * _blend) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].from, position, time))) * _blend)) : ((!(position >= keys[i].from)) ? ((!(time > keys[i].from)) ? ((!(time <= position)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].to, position, time))) * _blend) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(0.0 - (1.0 - keys[i].from), position, time))) * _blend)) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].from, 1.0 + position, time))) * _blend)) : ((!(time > keys[i].from)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].to, 0.0 - (1.0 - position), time))) * _blend) : ((!(time <= position)) ? (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(1.0 + keys[i].to, position, time))) * _blend) : (Mathf.Clamp01(keys[i].Evaluate((float)DMath.InverseLerp(keys[i].from, position, time))) * _blend)))));
				vector += keys[i].offset * num;
			}
			return vector;
		}
	}
}
