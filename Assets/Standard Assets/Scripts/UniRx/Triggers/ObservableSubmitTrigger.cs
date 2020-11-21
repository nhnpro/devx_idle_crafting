using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableSubmitTrigger : ObservableTriggerBase, IEventSystemHandler, ISubmitHandler
	{
		private Subject<BaseEventData> onSubmit;

		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			if (onSubmit != null)
			{
				onSubmit.OnNext(eventData);
			}
		}

		public UniRx.IObservable<BaseEventData> OnSubmitAsObservable()
		{
			return onSubmit ?? (onSubmit = new Subject<BaseEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onSubmit != null)
			{
				onSubmit.OnCompleted();
			}
		}
	}
}
