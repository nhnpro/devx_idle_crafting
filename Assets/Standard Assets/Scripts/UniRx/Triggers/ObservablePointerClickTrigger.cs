using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservablePointerClickTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerClickHandler
	{
		private Subject<PointerEventData> onPointerClick;

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (onPointerClick != null)
			{
				onPointerClick.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnPointerClickAsObservable()
		{
			return onPointerClick ?? (onPointerClick = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onPointerClick != null)
			{
				onPointerClick.OnCompleted();
			}
		}
	}
}
