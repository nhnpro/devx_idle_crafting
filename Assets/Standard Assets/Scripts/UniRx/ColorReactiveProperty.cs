using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
	[Serializable]
	public class ColorReactiveProperty : ReactiveProperty<Color>
	{
		protected override IEqualityComparer<Color> EqualityComparer => UnityEqualityComparer.Color;

		public ColorReactiveProperty()
		{
		}

		public ColorReactiveProperty(Color initialValue)
			: base(initialValue)
		{
		}
	}
}
