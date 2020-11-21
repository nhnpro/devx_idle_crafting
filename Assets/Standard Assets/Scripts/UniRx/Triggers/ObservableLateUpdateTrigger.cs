using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableLateUpdateTrigger : ObservableTriggerBase
	{
		private Subject<Unit> lateUpdate;

		private void LateUpdate()
		{
			if (lateUpdate != null)
			{
				lateUpdate.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> LateUpdateAsObservable()
		{
			return lateUpdate ?? (lateUpdate = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (lateUpdate != null)
			{
				lateUpdate.OnCompleted();
			}
		}
	}
}
