using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableTransformChangedTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onBeforeTransformParentChanged;

		private Subject<Unit> onTransformParentChanged;

		private Subject<Unit> onTransformChildrenChanged;

		private void OnBeforeTransformParentChanged()
		{
			if (onBeforeTransformParentChanged != null)
			{
				onBeforeTransformParentChanged.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnBeforeTransformParentChangedAsObservable()
		{
			return onBeforeTransformParentChanged ?? (onBeforeTransformParentChanged = new Subject<Unit>());
		}

		private void OnTransformParentChanged()
		{
			if (onTransformParentChanged != null)
			{
				onTransformParentChanged.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnTransformParentChangedAsObservable()
		{
			return onTransformParentChanged ?? (onTransformParentChanged = new Subject<Unit>());
		}

		private void OnTransformChildrenChanged()
		{
			if (onTransformChildrenChanged != null)
			{
				onTransformChildrenChanged.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnTransformChildrenChangedAsObservable()
		{
			return onTransformChildrenChanged ?? (onTransformChildrenChanged = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onBeforeTransformParentChanged != null)
			{
				onBeforeTransformParentChanged.OnCompleted();
			}
			if (onTransformParentChanged != null)
			{
				onTransformParentChanged.OnCompleted();
			}
			if (onTransformChildrenChanged != null)
			{
				onTransformChildrenChanged.OnCompleted();
			}
		}
	}
}
