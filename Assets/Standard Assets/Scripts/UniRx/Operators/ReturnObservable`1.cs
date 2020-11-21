using System;

namespace UniRx.Operators
{
	internal class ReturnObservable<T> : OperatorObservableBase<T>
	{
		private class Return : OperatorObserverBase<T, T>
		{
			public Return(UniRx.IObserver<T> observer, IDisposable cancel)
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

		private readonly T value;

		private readonly IScheduler scheduler;

		public ReturnObservable(T value, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			this.value = value;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new Return(observer, cancel);
			if (scheduler == Scheduler.Immediate)
			{
				observer.OnNext(value);
				observer.OnCompleted();
				return Disposable.Empty;
			}
			return scheduler.Schedule(delegate
			{
				observer.OnNext(value);
				observer.OnCompleted();
			});
		}
	}
}
