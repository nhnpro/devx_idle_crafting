using System;

namespace UniRx.Operators
{
	internal class SwitchObservable<T> : OperatorObservableBase<T>
	{
		private class SwitchObserver : OperatorObserverBase<UniRx.IObservable<T>, T>
		{
			private class Switch : IObserver<T>
			{
				private readonly SwitchObserver parent;

				private readonly ulong id;

				public Switch(SwitchObserver observer, ulong id)
				{
					parent = observer;
					this.id = id;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (parent.latest == id)
						{
							parent.observer.OnNext(value);
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (parent.latest == id)
						{
							parent.observer.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (parent.latest == id)
						{
							parent.hasLatest = false;
							if (parent.isStopped)
							{
								parent.observer.OnCompleted();
							}
						}
					}
				}
			}

			private readonly SwitchObservable<T> parent;

			private readonly object gate = new object();

			private readonly SerialDisposable innerSubscription = new SerialDisposable();

			private bool isStopped;

			private ulong latest;

			private bool hasLatest;

			public SwitchObserver(SwitchObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.sources.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, innerSubscription);
			}

			public override void OnNext(UniRx.IObservable<T> value)
			{
				ulong id = 0uL;
				lock (gate)
				{
					id = ++latest;
					hasLatest = true;
				}
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				innerSubscription.Disposable = singleAssignmentDisposable;
				singleAssignmentDisposable.Disposable = value.Subscribe(new Switch(this, id));
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
					isStopped = true;
					if (!hasLatest)
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
		}

		private readonly UniRx.IObservable<UniRx.IObservable<T>> sources;

		public SwitchObservable(UniRx.IObservable<UniRx.IObservable<T>> sources)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new SwitchObserver(this, observer, cancel).Run();
		}
	}
}
