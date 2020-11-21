using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableDropTrigger : ObservableTriggerBase, IEventSystemHandler, IDropHandler
	{
		private Subject<PointerEventData> onDrop;

		void IDropHandler.OnDrop(PointerEventData eventData)
		{
			if (onDrop != null)
			{
				onDrop.OnNext(eventData);
			}
		}

		public UniRx.IObservable<PointerEventData> OnDropAsObservable()
		{
			return onDrop ?? (onDrop = new Subject<PointerEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onDrop != null)
			{
				onDrop.OnCompleted();
			}
		}
	}
}
