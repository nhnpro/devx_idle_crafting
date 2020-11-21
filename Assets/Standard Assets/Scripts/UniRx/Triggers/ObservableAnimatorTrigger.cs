using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableAnimatorTrigger : ObservableTriggerBase
	{
		private Subject<int> onAnimatorIK;

		private Subject<Unit> onAnimatorMove;

		private void OnAnimatorIK(int layerIndex)
		{
			if (onAnimatorIK != null)
			{
				onAnimatorIK.OnNext(layerIndex);
			}
		}

		public UniRx.IObservable<int> OnAnimatorIKAsObservable()
		{
			return onAnimatorIK ?? (onAnimatorIK = new Subject<int>());
		}

		private void OnAnimatorMove()
		{
			if (onAnimatorMove != null)
			{
				onAnimatorMove.OnNext(Unit.Default);
			}
		}

		public UniRx.IObservable<Unit> OnAnimatorMoveAsObservable()
		{
			return onAnimatorMove ?? (onAnimatorMove = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onAnimatorIK != null)
			{
				onAnimatorIK.OnCompleted();
			}
			if (onAnimatorMove != null)
			{
				onAnimatorMove.OnCompleted();
			}
		}
	}
}
