using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
	[Serializable]
	public class Vector3ReactiveProperty : ReactiveProperty<Vector3>
	{
		protected override IEqualityComparer<Vector3> EqualityComparer => UnityEqualityComparer.Vector3;

		public Vector3ReactiveProperty()
		{
		}

		public Vector3ReactiveProperty(Vector3 initialValue)
			: base(initialValue)
		{
		}
	}
}
