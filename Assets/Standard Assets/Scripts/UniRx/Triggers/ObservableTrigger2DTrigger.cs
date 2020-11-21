using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableTrigger2DTrigger : ObservableTriggerBase
	{
		private Subject<Collider2D> onTriggerEnter2D;

		private Subject<Collider2D> onTriggerExit2D;

		private Subject<Collider2D> onTriggerStay2D;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (onTriggerEnter2D != null)
			{
				onTriggerEnter2D.OnNext(other);
			}
		}

		public UniRx.IObservable<Collider2D> OnTriggerEnter2DAsObservable()
		{
			return onTriggerEnter2D ?? (onTriggerEnter2D = new Subject<Collider2D>());
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (onTriggerExit2D != null)
			{
				onTriggerExit2D.OnNext(other);
			}
		}

		public UniRx.IObservable<Collider2D> OnTriggerExit2DAsObservable()
		{
			return onTriggerExit2D ?? (onTriggerExit2D = new Subject<Collider2D>());
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (onTriggerStay2D != null)
			{
				onTriggerStay2D.OnNext(other);
			}
		}

		public UniRx.IObservable<Collider2D> OnTriggerStay2DAsObservable()
		{
			return onTriggerStay2D ?? (onTriggerStay2D = new Subject<Collider2D>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onTriggerEnter2D != null)
			{
				onTriggerEnter2D.OnCompleted();
			}
			if (onTriggerExit2D != null)
			{
				onTriggerExit2D.OnCompleted();
			}
			if (onTriggerStay2D != null)
			{
				onTriggerStay2D.OnCompleted();
			}
		}
	}
}
