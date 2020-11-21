using System;

namespace UniRx.Operators
{
	internal class SampleObservable<T> : OperatorObservableBase<T>
	{
		private class Sample : OperatorObserverBase<T, T>
		{
			private readonly SampleObservable<T> parent;

			private readonly object gate = new object();

			private T latestValue = default(T);

			private bool isUpdated;

			private bool isCompleted;

			private SingleAssignmentDisposable sourceSubscription;

			public Sample(SampleObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				sourceSubscription.Disposable = parent.source.Subscribe(this);
				ISchedulerPeriodic schedulerPeriodic = parent.scheduler as ISchedulerPeriodic;
				return StableCompositeDisposable.Create(disposable2: (schedulerPeriodic == null) ? parent.scheduler.Schedule(parent.interval, OnNextRecursive) : schedulerPeriodic.SchedulePeriodic(parent.interval, OnNextTick), disposable1: sourceSubscription);
			}

			private void OnNextTick()
			{
				lock (gate)
				{
					if (isUpdated)
					{
						T value = latestValue;
						isUpdated = false;
						observer.OnNext(value);
					}
					if (isCompleted)
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
			}

			private void OnNextRecursive(Action<TimeSpan> self)
			{
				lock (gate)
				{
					if (isUpdated)
					{
						T value = latestValue;
						isUpdated = false;
						observer.OnNext(value);
					}
					if (isCompleted)
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
				self(parent.interval);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					latestValue = value;
					isUpdated = true;
				}
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				lock (gate)
				{
					isCompleted = true;
					sourceSubscription.Dispose();
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly TimeSpan interval;

		private readonly IScheduler scheduler;

		public SampleObservable(UniRx.IObservable<T> source, TimeSpan interval, IScheduler scheduler)
			: base(source.IsRequiredSubscribeOnCurrentThread() || scheduler == Scheduler.CurrentThread)
		{
			this.source = source;
			this.interval = interval;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Sample(this, observer, cancel).Run();
		}
	}
	internal class SampleObservable<T, T2> : OperatorObservableBase<T>
	{
		private class Sample : OperatorObserverBase<T, T>
		{
			private class SampleTick : IObserver<T2>
			{
				private readonly Sample parent;

				public SampleTick(Sample parent)
				{
					this.parent = parent;
				}

				public void OnCompleted()
				{
				}

				public void OnError(Exception error)
				{
				}

				public void OnNext(T2 _)
				{
					lock (parent.gate)
					{
						if (parent.isUpdated)
						{
							T latestValue = parent.latestValue;
							parent.isUpdated = false;
							parent.observer.OnNext(latestValue);
						}
						if (parent.isCompleted)
						{
							try
							{
								parent.observer.OnCompleted();
							}
							finally
							{
								parent.Dispose();
							}
						}
					}
				}
			}

			private readonly SampleObservable<T, T2> parent;

			private readonly object gate = new object();

			private T latestValue = default(T);

			private bool isUpdated;

			private bool isCompleted;

			private SingleAssignmentDisposable sourceSubscription;

			public Sample(SampleObservable<T, T2> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				sourceSubscription.Disposable = parent.source.Subscribe(this);
				IDisposable disposable = parent.intervalSource.Subscribe(new SampleTick(this));
				return StableCompositeDisposable.Create(sourceSubscription, disposable);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					latestValue = value;
					isUpdated = true;
				}
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				lock (gate)
				{
					isCompleted = true;
					sourceSubscription.Dispose();
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly UniRx.IObservable<T2> intervalSource;

		public SampleObservable(UniRx.IObservable<T> source, UniRx.IObservable<T2> intervalSource)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.intervalSource = intervalSource;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Sample(this, observer, cancel).Run();
		}
	}
}
