using System;

namespace UniRx.Operators
{
	internal class TimerObservable : OperatorObservableBase<long>
	{
		private class Timer : OperatorObserverBase<long, long>
		{
			private long index;

			public Timer(UniRx.IObserver<long> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public void OnNext()
			{
				try
				{
					observer.OnNext(index++);
				}
				catch
				{
					Dispose();
					throw;
				}
			}

			public override void OnNext(long value)
			{
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

		private readonly DateTimeOffset? dueTimeA;

		private readonly TimeSpan? dueTimeB;

		private readonly TimeSpan? period;

		private readonly IScheduler scheduler;

		public TimerObservable(DateTimeOffset dueTime, TimeSpan? period, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			dueTimeA = dueTime;
			this.period = period;
			this.scheduler = scheduler;
		}

		public TimerObservable(TimeSpan dueTime, TimeSpan? period, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			dueTimeB = dueTime;
			this.period = period;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<long> observer, IDisposable cancel)
		{
			Timer timerObserver = new Timer(observer, cancel);
			TimeSpan timeSpan = (!dueTimeA.HasValue) ? dueTimeB.Value : (dueTimeA.Value - scheduler.Now);
			if (!period.HasValue)
			{
				return scheduler.Schedule(Scheduler.Normalize(timeSpan), delegate
				{
					timerObserver.OnNext();
					timerObserver.OnCompleted();
				});
			}
			ISchedulerPeriodic periodicScheduler = scheduler as ISchedulerPeriodic;
			if (periodicScheduler != null)
			{
				if (timeSpan == period.Value)
				{
					return periodicScheduler.SchedulePeriodic(Scheduler.Normalize(timeSpan), timerObserver.OnNext);
				}
				SerialDisposable disposable = new SerialDisposable();
				disposable.Disposable = scheduler.Schedule(Scheduler.Normalize(timeSpan), delegate
				{
					timerObserver.OnNext();
					TimeSpan timeSpan2 = Scheduler.Normalize(period.Value);
					disposable.Disposable = periodicScheduler.SchedulePeriodic(timeSpan2, timerObserver.OnNext);
				});
				return disposable;
			}
			TimeSpan timeP = Scheduler.Normalize(period.Value);
			return scheduler.Schedule(Scheduler.Normalize(timeSpan), delegate(Action<TimeSpan> self)
			{
				timerObserver.OnNext();
				self(timeP);
			});
		}
	}
}
