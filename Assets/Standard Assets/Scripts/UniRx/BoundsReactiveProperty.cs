using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
	[Serializable]
	public class BoundsReactiveProperty : ReactiveProperty<Bounds>
	{
		protected override IEqualityComparer<Bounds> EqualityComparer => UnityEqualityComparer.Bounds;

		public BoundsReactiveProperty()
		{
		}

		public BoundsReactiveProperty(Bounds initialValue)
			: base(initialValue)
		{
		}
	}
}
