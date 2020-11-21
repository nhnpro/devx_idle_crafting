using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableScrollTrigger : ObservableTriggerBase, IEventSystemHandler, IScrollHandler
	{
		private Subject<PointerEventData> onScroll;

		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			if (onScroll != null)
			{
				onScroll.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnScrollAsObservable()
		{
			return onScroll ?? (onScroll = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onScroll != null)
			{
				onScroll.OnCompleted();
			}
		}
	}
}
