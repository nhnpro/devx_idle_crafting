using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableInitializePotentialDragTrigger : ObservableTriggerBase, IEventSystemHandler, IInitializePotentialDragHandler
	{
		private Subject<PointerEventData> onInitializePotentialDrag;

		void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (onInitializePotentialDrag != null)
			{
				onInitializePotentialDrag.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnInitializePotentialDragAsObservable()
		{
			return onInitializePotentialDrag ?? (onInitializePotentialDrag = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onInitializePotentialDrag != null)
			{
				onInitializePotentialDrag.OnCompleted();
			}
		}
	}
}
