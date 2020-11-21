using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableCollisionTrigger : ObservableTriggerBase
	{
		private Subject<Collision> onCollisionEnter;

		private Subject<Collision> onCollisionExit;

		private Subject<Collision> onCollisionStay;

		private void OnCollisionEnter(Collision collision)
		{
			if (onCollisionEnter != null)
			{
				onCollisionEnter.OnNext(collision);
			}
		}

		public UniRx.IObservable<Collision> OnCollisionEnterAsObservable()
		{
			return onCollisionEnter ?? (onCollisionEnter = new Subject<Collision>());
		}

		private void OnCollisionExit(Collision collisionInfo)
		{
			if (onCollisionExit != null)
			{
				onCollisionExit.OnNext(collisionInfo);
			}
		}

		public UniRx.IObservable<Collision> OnCollisionExitAsObservable()
		{
			return onCollisionExit ?? (onCollisionExit = new Subject<Collision>());
		}

		private void OnCollisionStay(Collision collisionInfo)
		{
			if (onCollisionStay != null)
			{
				onCollisionStay.OnNext(collisionInfo);
			}
		}

		public UniRx.IObservable<Collision> OnCollisionStayAsObservable()
		{
			return onCollisionStay ?? (onCollisionStay = new Subject<Collision>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onCollisionEnter != null)
			{
				onCollisionEnter.OnCompleted();
			}
			if (onCollisionExit != null)
			{
				onCollisionExit.OnCompleted();
			}
			if (onCollisionStay != null)
			{
				onCollisionStay.OnCompleted();
			}
		}
	}
}
