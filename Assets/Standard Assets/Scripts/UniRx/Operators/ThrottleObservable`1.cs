using System;

namespace UniRx.Operators
{
	internal class ThrottleObservable<T> : OperatorObservableBase<T>
	{
		private class Throttle : OperatorObserverBase<T, T>
		{
			private readonly ThrottleObservable<T> parent;

			private readonly object gate = new object();

			private T latestValue = default(T);

			private bool hasValue;

			private SerialDisposable cancelable;

			private ulong id;

			public Throttle(ThrottleObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				cancelable = new SerialDisposable();
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(cancelable, disposable);
			}

			private void OnNext(ulong currentid)
			{
				lock (gate)
				{
					if (hasValue && id == currentid)
					{
						observer.OnNext(latestValue);
					}
					hasValue = false;
				}
			}

			public override void OnNext(T value)
			{
				ulong currentid;
				lock (gate)
				{
					hasValue = true;
					latestValue = value;
					id++;
					currentid = id;
				}
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				cancelable.Disposable = singleAssignmentDisposable;
				singleAssignmentDisposable.Disposable = parent.scheduler.Schedule(parent.dueTime, delegate
				{
					OnNext(currentid);
				});
			}

			public override void OnError(Exception error)
			{
				cancelable.Dispose();
				lock (gate)
				{
					hasValue = false;
					id++;
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
				cancelable.Dispose();
				lock (gate)
				{
					if (hasValue)
					{
						observer.OnNext(latestValue);
					}
					hasValue = false;
					id++;
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

		private readonly TimeSpan dueTime;

		private readonly IScheduler scheduler;

		public ThrottleObservable(UniRx.IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.dueTime = dueTime;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Throttle(this, observer, cancel).Run();
		}
	}
}
