using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableVisibleTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onBecameInvisible;

		private Subject<Unit> onBecameVisible;

		private void OnBecameInvisible()
		{
			if (onBecameInvisible != null)
			{
				onBecameInvisible.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnBecameInvisibleAsObservable()
		{
			return onBecameInvisible ?? (onBecameInvisible = new Subject<Unit>());
		}

		private void OnBecameVisible()
		{
			if (onBecameVisible != null)
			{
				onBecameVisible.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnBecameVisibleAsObservable()
		{
			return onBecameVisible ?? (onBecameVisible = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onBecameInvisible != null)
			{
				onBecameInvisible.OnCompleted();
			}
			if (onBecameVisible != null)
			{
				onBecameVisible.OnCompleted();
			}
		}
	}
}
