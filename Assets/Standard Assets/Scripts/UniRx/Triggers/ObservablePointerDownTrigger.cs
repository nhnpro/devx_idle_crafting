using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservablePointerDownTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerDownHandler
	{
		private Subject<PointerEventData> onPointerDown;

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (onPointerDown != null)
			{
				onPointerDown.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnPointerDownAsObservable()
		{
			return onPointerDown ?? (onPointerDown = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onPointerDown != null)
			{
				onPointerDown.OnCompleted();
			}
		}
	}
}
