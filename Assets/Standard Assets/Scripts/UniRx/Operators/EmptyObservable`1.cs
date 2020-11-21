using System;

namespace UniRx.Operators
{
	internal class EmptyObservable<T> : OperatorObservableBase<T>
	{
		private class Empty : OperatorObserverBase<T, T>
		{
			public Empty(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				try
				{
					observer.OnNext(value);
				}
				catch
				{
					Dispose();
					throw;
				}
			}

			public override void OnError(Exception error)
			{
				try
				{
					observer.OnError(error);
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnCompleted()
			{
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly IScheduler scheduler;

		public EmptyObservable(IScheduler scheduler)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new Empty(observer, cancel);
			if (scheduler == Scheduler.Immediate)
			{
				observer.OnCompleted();
				return Disposable.Empty;
			}
			IScheduler obj = scheduler;
			UniRx.IObserver<T> observer2 = observer;
			return obj.Schedule(observer2.OnCompleted);
		}
	}
}
