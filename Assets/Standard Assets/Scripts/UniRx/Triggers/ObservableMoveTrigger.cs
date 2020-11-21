using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableMoveTrigger : ObservableTriggerBase, IEventSystemHandler, IMoveHandler
	{
		private Subject<AxisEventData> onMove;

		void IMoveHandler.OnMove(AxisEventData eventData)
		{
			if (onMove != null)
			{
				onMove.OnNext(eventData);
			}
		}

		public UniRx.IObservable<AxisEventData> OnMoveAsObservable()
		{
			return onMove ?? (onMove = new Subject<AxisEventData>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onMove != null)
			{
				onMove.OnCompleted();
			}
		}
	}
}
