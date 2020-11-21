using System;

namespace UniRx.Operators
{
	internal class TimeIntervalObservable<T> : OperatorObservableBase<TimeInterval<T>>
	{
		private class TimeInterval : OperatorObserverBase<T, TimeInterval<T>>
		{
			private readonly TimeIntervalObservable<T> parent;

			private DateTimeOffset lastTime;

			public TimeInterval(TimeIntervalObservable<T> parent, IObserver<TimeInterval<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				lastTime = parent.scheduler.Now;
			}

			public override void OnNext(T value)
			{
				DateTimeOffset now = parent.scheduler.Now;
				TimeSpan interval = now.Subtract(lastTime);
				lastTime = now;
				observer.OnNext(new TimeInterval<T>(value, interval));
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

		private readonly UniRx.IObservable<T> source;

		private readonly IScheduler scheduler;

		public TimeIntervalObservable(UniRx.IObservable<T> source, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TimeInterval<T>> observer, IDisposable cancel)
		{
			return source.Subscribe(new TimeInterval(this, observer, cancel));
		}
	}
}
