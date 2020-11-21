using System;

namespace UniRx.Operators
{
	internal class RangeObservable : OperatorObservableBase<int>
	{
		private class Range : OperatorObserverBase<int, int>
		{
			public Range(UniRx.IObserver<int> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(int value)
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

		private readonly int start;

		private readonly int count;

		private readonly IScheduler scheduler;

		public RangeObservable(int start, int count, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count < 0");
			}
			this.start = start;
			this.count = count;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<int> observer, IDisposable cancel)
		{
			observer = new Range(observer, cancel);
			if (scheduler == Scheduler.Immediate)
			{
				for (int j = 0; j < count; j++)
				{
					int value = start + j;
					observer.OnNext(value);
				}
				observer.OnCompleted();
				return Disposable.Empty;
			}
			int i = 0;
			return scheduler.Schedule(delegate(Action self)
			{
				if (i < count)
				{
					int value2 = start + i;
					observer.OnNext(value2);
					i++;
					self();
				}
				else
				{
					observer.OnCompleted();
				}
			});
		}
	}
}
