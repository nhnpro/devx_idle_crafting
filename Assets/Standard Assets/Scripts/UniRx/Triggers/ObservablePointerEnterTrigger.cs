using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservablePointerEnterTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerEnterHandler
	{
		private Subject<PointerEventData> onPointerEnter;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null)
			{
				onPointerEnter.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnPointerEnterAsObservable()
		{
			return onPointerEnter ?? (onPointerEnter = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onPointerEnter != null)
			{
				onPointerEnter.OnCompleted();
			}
		}
	}
}
