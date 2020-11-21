using System;

namespace UniRx.Operators
{
	internal class RepeatObservable<T> : OperatorObservableBase<T>
	{
		private class Repeat : OperatorObserverBase<T, T>
		{
			public Repeat(UniRx.IObserver<T> observer, IDisposable cancel)
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

		private readonly int? repeatCount;

		private readonly IScheduler scheduler;

		public RepeatObservable(T value, int? repeatCount, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			this.value = value;
			this.repeatCount = repeatCount;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new Repeat(observer, cancel);
			if (!repeatCount.HasValue)
			{
				return scheduler.Schedule(delegate(Action self)
				{
					observer.OnNext(value);
					self();
				});
			}
			if (scheduler == Scheduler.Immediate)
			{
				int num = repeatCount.Value;
				for (int i = 0; i < num; i++)
				{
					observer.OnNext(value);
				}
				observer.OnCompleted();
				return Disposable.Empty;
			}
			int currentCount = repeatCount.Value;
			return scheduler.Schedule(delegate(Action self)
			{
				if (currentCount > 0)
				{
					observer.OnNext(value);
					currentCount--;
				}
				if (currentCount == 0)
				{
					observer.OnCompleted();
				}
				else
				{
					self();
				}
			});
		}
	}
}
