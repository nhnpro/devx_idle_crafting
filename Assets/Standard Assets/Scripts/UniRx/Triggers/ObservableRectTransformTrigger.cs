using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableRectTransformTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onRectTransformDimensionsChange;

		private Subject<Unit> onRectTransformRemoved;

		public void OnRectTransformDimensionsChange()
		{
			if (onRectTransformDimensionsChange != null)
			{
				onRectTransformDimensionsChange.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnRectTransformDimensionsChangeAsObservable()
		{
			return onRectTransformDimensionsChange ?? (onRectTransformDimensionsChange = new Subject<Unit>());
		}

		public void OnRectTransformRemoved()
		{
			if (onRectTransformRemoved != null)
			{
				onRectTransformRemoved.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnRectTransformRemovedAsObservable()
		{
			return onRectTransformRemoved ?? (onRectTransformRemoved = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onRectTransformDimensionsChange != null)
			{
				onRectTransformDimensionsChange.OnCompleted();
			}
			if (onRectTransformRemoved != null)
			{
				onRectTransformRemoved.OnCompleted();
			}
		}
	}
}
