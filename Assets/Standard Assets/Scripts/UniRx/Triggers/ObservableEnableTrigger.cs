using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableEnableTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onEnable;

		private Subject<Unit> onDisable;

		private void OnEnable()
		{
			if (onEnable != null)
			{
				onEnable.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnEnableAsObservable()
		{
			return onEnable ?? (onEnable = new Subject<Unit>());
		}

		private void OnDisable()
		{
			if (onDisable != null)
			{
				onDisable.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnDisableAsObservable()
		{
			return onDisable ?? (onDisable = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onEnable != null)
			{
				onEnable.OnCompleted();
			}
			if (onDisable != null)
			{
				onDisable.OnCompleted();
			}
		}
	}
}
