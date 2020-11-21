using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableCancelTrigger : ObservableTriggerBase, IEventSystemHandler, ICancelHandler
	{
		private Subject<BaseEventData> onCancel;

		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			if (onCancel != null)
			{
				onCancel.OnNext(eventData);
			}
		}

		public UniRx.IObservable<BaseEventData> OnCancelAsObservable()
		{
			return onCancel ?? (onCancel = new Subject<BaseEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onCancel != null)
			{
				onCancel.OnCompleted();
			}
		}
	}
}
