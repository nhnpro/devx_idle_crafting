using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Length Calculator")]
	public class LengthCalculator : SplineUser
	{
		[Serializable]
		public class LengthEvent
		{
			public enum Type
			{
				Growing,
				Shrinking,
				Both
			}

			public bool enabled = true;

			public float targetLength;

			public SplineAction action = new SplineAction();

			public Type type = Type.Both;

			public LengthEvent()
			{
			}

			public LengthEvent(Type t)
			{
				type = t;
			}

			public LengthEvent(Type t, SplineAction a)
			{
				type = t;
				action = a;
			}

			public void Check(float fromLength, float toLength)
			{
				if (enabled)
				{
					bool flag = false;
					switch (type)
					{
					case Type.Growing:
						flag = (toLength >= targetLength && fromLength < targetLength);
						break;
					case Type.Shrinking:
						flag = (toLength <= targetLength && fromLength > targetLength);
						break;
					case Type.Both:
						flag = ((toLength >= targetLength && fromLength < targetLength) || (toLength <= targetLength && fromLength > targetLength));
						break;
					}
					if (flag)
					{
						action.Invoke();
					}
				}
			}
		}

		[HideInInspector]
		public LengthEvent[] lengthEvents = new LengthEvent[0];

		[HideInInspector]
		public float idealLength = 1f;

		private float _length;

		private float lastLength;

		public float length => _length;

		protected override void Awake()
		{
			base.Awake();
			_length = _address.CalculateLength(base.clipFrom, base.clipTo);
			lastLength = _length;
			for (int i = 0; i < lengthEvents.Length; i++)
			{
				if (lengthEvents[i].targetLength == _length)
				{
					lengthEvents[i].action.Invoke();
				}
			}
		}

		protected override void Build()
		{
			base.Build();
			_length = CalculateLength(base.clipFrom, base.clipTo);
			if (lastLength != _length)
			{
				for (int i = 0; i < lengthEvents.Length; i++)
				{
					lengthEvents[i].Check(lastLength, _length);
				}
				lastLength = _length;
			}
		}

		private void AddEvent(LengthEvent lengthEvent)
		{
			LengthEvent[] array = new LengthEvent[lengthEvents.Length + 1];
			lengthEvents.CopyTo(array, 0);
			array[array.Length - 1] = lengthEvent;
			lengthEvents = array;
		}

		public void AddEvent(LengthEvent.Type t, UnityAction call, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddEvent(LengthEvent.Type t, UnityAction<int> call, int value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddEvent(LengthEvent.Type t, UnityAction<float> call, float value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddEvent(LengthEvent.Type t, UnityAction<double> call, double value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddTrigger(LengthEvent.Type t, UnityAction<string> call, string value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddEvent(LengthEvent.Type t, UnityAction<bool> call, bool value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddEvent(LengthEvent.Type t, UnityAction<GameObject> call, GameObject value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}

		public void AddEvent(LengthEvent.Type t, UnityAction<Transform> call, Transform value, float targetLength = 0f, LengthEvent.Type type = LengthEvent.Type.Both)
		{
			LengthEvent lengthEvent = new LengthEvent(t, new SplineAction(call, value));
			lengthEvent.targetLength = targetLength;
			lengthEvent.type = type;
			AddEvent(lengthEvent);
		}
	}
}
