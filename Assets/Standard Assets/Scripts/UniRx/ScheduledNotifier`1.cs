using System;

namespace UniRx
{
	public class ScheduledNotifier<T> : UniRx.IObservable<T>, IProgress<T>
	{
		private readonly IScheduler scheduler;

		private readonly Subject<T> trigger = new Subject<T>();

		public ScheduledNotifier()
		{
			scheduler = Scheduler.DefaultSchedulers.ConstantTimeOperations;
		}

		public ScheduledNotifier(IScheduler scheduler)
		{
			if (scheduler == null)
			{
				throw new ArgumentNullException("scheduler");
			}
			this.scheduler = scheduler;
		}

		public void Report(T value)
		{
			scheduler.Schedule(delegate
			{
				trigger.OnNext(value);
			});
		}

		public IDisposable Report(T value, TimeSpan dueTime)
		{
			return scheduler.Schedule(dueTime, delegate
			{
				trigger.OnNext(value);
			});
		}

		public IDisposable Report(T value, DateTimeOffset dueTime)
		{
			return scheduler.Schedule(dueTime, (Action)delegate
			{
				trigger.OnNext(value);
			});
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}
			return trigger.Subscribe(observer);
		}
	}
}
