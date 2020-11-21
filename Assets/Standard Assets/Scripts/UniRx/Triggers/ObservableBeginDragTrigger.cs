using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableBeginDragTrigger : ObservableTriggerBase, IEventSystemHandler, IBeginDragHandler
	{
		private Subject<PointerEventData> onBeginDrag;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDrag != null)
			{
				onBeginDrag.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnBeginDragAsObservable()
		{
			return onBeginDrag ?? (onBeginDrag = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onBeginDrag != null)
			{
				onBeginDrag.OnCompleted();
			}
		}
	}
}
