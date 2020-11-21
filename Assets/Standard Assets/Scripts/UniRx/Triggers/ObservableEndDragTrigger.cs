using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableEndDragTrigger : ObservableTriggerBase, IEventSystemHandler, IEndDragHandler
	{
		private Subject<PointerEventData> onEndDrag;

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (onEndDrag != null)
			{
				onEndDrag.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnEndDragAsObservable()
		{
			return onEndDrag ?? (onEndDrag = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onEndDrag != null)
			{
				onEndDrag.OnCompleted();
			}
		}
	}
}
