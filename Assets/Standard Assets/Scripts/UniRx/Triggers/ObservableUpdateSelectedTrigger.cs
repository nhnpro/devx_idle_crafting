using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableUpdateSelectedTrigger : ObservableTriggerBase, IEventSystemHandler, IUpdateSelectedHandler
	{
		private Subject<BaseEventData> onUpdateSelected;

		void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
		{
			if (onUpdateSelected != null)
			{
				onUpdateSelected.OnNext(eventData);
			}
		}

		public UniRx.IObservable<BaseEventData> OnUpdateSelectedAsObservable()
		{
			return onUpdateSelected ?? (onUpdateSelected = new Subject<BaseEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onUpdateSelected != null)
			{
				onUpdateSelected.OnCompleted();
			}
		}
	}
}
