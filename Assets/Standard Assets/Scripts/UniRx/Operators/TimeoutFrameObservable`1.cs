using System;

namespace UniRx.Operators
{
	internal class TimeoutFrameObservable<T> : OperatorObservableBase<T>
	{
		private class TimeoutFrame : OperatorObserverBase<T, T>
		{
			private class TimeoutFrameTick : IObserver<long>
			{
				private readonly TimeoutFrame parent;

				private readonly ulong timerId;

				public TimeoutFrameTick(TimeoutFrame parent, ulong timerId)
				{
					this.parent = parent;
					this.timerId = timerId;
				}

				public void OnCompleted()
				{
				}

				public void OnError(Exception error)
				{
				}

				public void OnNext(long _)
				{
					lock (parent.gate)
					{
						if (parent.objectId == timerId)
						{
							parent.isTimeout = true;
						}
					}
					if (parent.isTimeout)
					{
						try
						{
							parent.observer.OnError(new TimeoutException());
						}
						finally
						{
							parent.Dispose();
						}
					}
				}
			}

			private readonly TimeoutFrameObservable<T> parent;

			private readonly object gate = new object();

			private ulong objectId;

			private bool isTimeout;

			private SingleAssignmentDisposable sourceSubscription;

			private SerialDisposable timerSubscription;

			public TimeoutFrame(TimeoutFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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
				return Observable.TimerFrame(parent.frameCount, parent.frameCountType).Subscribe(new TimeoutFrameTick(this, timerId));
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

		private readonly UniRx.IObservable<T> source;

		private readonly int frameCount;

		private readonly FrameCountType frameCountType;

		public TimeoutFrameObservable(UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new TimeoutFrame(this, observer, cancel).Run();
		}
	}
}
