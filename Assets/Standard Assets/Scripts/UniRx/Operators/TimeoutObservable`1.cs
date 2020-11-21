using System;

namespace UniRx.Operators
{
	internal class TimeoutObservable<T> : OperatorObservableBase<T>
	{
		private class Timeout : OperatorObserverBase<T, T>
		{
			private readonly TimeoutObservable<T> parent;

			private readonly object gate = new object();

			private ulong objectId;

			private bool isTimeout;

			private SingleAssignmentDisposable sourceSubscription;

			private SerialDisposable timerSubscription;

			public Timeout(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				timerSubscription = new SerialDisposable();
				timerSubscription.Disposable = RunTimer(objectId);
				sourceSubscription.Disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
			}

			private IDisposable RunTimer(ulong timerId)
			{
				return parent.scheduler.Schedule(parent.dueTime.Value, delegate
				{
					lock (gate)
					{
						if (objectId == timerId)
						{
							isTimeout = true;
						}
					}
					if (isTimeout)
					{
						try
						{
							observer.OnError(new TimeoutException());
						}
						finally
						{
							Dispose();
						}
					}
				});
			}

			public override void OnNext(T value)
			{
				bool flag;
				ulong timerId;
				lock (gate)
				{
					flag = isTimeout;
					objectId++;
					timerId = objectId;
				}
				if (!flag)
				{
					timerSubscription.Disposable = Disposable.Empty;
					observer.OnNext(value);
					timerSubscription.Disposable = RunTimer(timerId);
				}
			}

			public override void OnError(Exception error)
			{
				bool flag;
				lock (gate)
				{
					flag = isTimeout;
					objectId++;
				}
				if (!flag)
				{
					timerSubscription.Dispose();
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
				bool flag;
				lock (gate)
				{
					flag = isTimeout;
					objectId++;
				}
				if (!flag)
				{
					timerSubscription.Dispose();
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

		private class Timeout_ : OperatorObserverBase<T, T>
		{
			private readonly TimeoutObservable<T> parent;

			private readonly object gate = new object();

			private bool isFinished;

			private SingleAssignmentDisposable sourceSubscription;

			private IDisposable timerSubscription;

			public Timeout_(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				timerSubscription = parent.scheduler.Schedule(parent.dueTimeDT.Value, OnNext);
				sourceSubscription.Disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
			}

			private void OnNext()
			{
				lock (gate)
				{
					if (isFinished)
					{
						return;
					}
					isFinished = true;
				}
				sourceSubscription.Dispose();
				try
				{
					observer.OnError(new TimeoutException());
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					if (!isFinished)
					{
						observer.OnNext(value);
					}
				}
			}

			public override void OnError(Exception error)
			{
				lock (gate)
				{
					if (isFinished)
					{
						return;
					}
					isFinished = true;
					timerSubscription.Dispose();
				}
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
				lock (gate)
				{
					if (!isFinished)
					{
						isFinished = true;
						timerSubscription.Dispose();
					}
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

		private readonly UniRx.IObservable<T> source;

		private readonly TimeSpan? dueTime;

		private readonly DateTimeOffset? dueTimeDT;

		private readonly IScheduler scheduler;

		public TimeoutObservable(UniRx.IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.dueTime = dueTime;
			this.scheduler = scheduler;
		}

		public TimeoutObservable(UniRx.IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			dueTimeDT = dueTime;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (dueTime.HasValue)
			{
				return new Timeout(this, observer, cancel).Run();
			}
			return new Timeout_(this, observer, cancel).Run();
		}
	}
}
