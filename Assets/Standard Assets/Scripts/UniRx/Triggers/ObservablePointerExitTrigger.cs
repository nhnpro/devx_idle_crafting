using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservablePointerExitTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerExitHandler
	{
		private Subject<PointerEventData> onPointerExit;

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null)
			{
				onPointerExit.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnPointerExitAsObservable()
		{
			return onPointerExit ?? (onPointerExit = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onPointerExit != null)
			{
				onPointerExit.OnCompleted();
			}
		}
	}
}
