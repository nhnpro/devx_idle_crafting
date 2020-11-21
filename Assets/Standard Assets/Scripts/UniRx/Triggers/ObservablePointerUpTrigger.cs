using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservablePointerUpTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerUpHandler
	{
		private Subject<PointerEventData> onPointerUp;

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (onPointerUp != null)
			{
				onPointerUp.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnPointerUpAsObservable()
		{
			return onPointerUp ?? (onPointerUp = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onPointerUp != null)
			{
				onPointerUp.OnCompleted();
			}
		}
	}
}
