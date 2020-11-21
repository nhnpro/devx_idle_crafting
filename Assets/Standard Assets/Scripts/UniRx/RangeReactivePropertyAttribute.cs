using System;
using UnityEngine;

namespace UniRx
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class RangeReactivePropertyAttribute : PropertyAttribute
	{
		public float Min
		{
			get;
			private set;
		}

		public float Max
		{
			get;
			private set;
		}

		public RangeReactivePropertyAttribute(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}
