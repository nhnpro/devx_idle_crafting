using System;

namespace UniRx.Operators
{
	internal class TakeUntilObservable<T, TOther> : OperatorObservableBase<T>
	{
		private class TakeUntil : OperatorObserverBase<T, T>
		{
			private class TakeUntilOther : IObserver<TOther>
			{
				private readonly TakeUntil sourceObserver;

				private readonly IDisposable subscription;

				public TakeUntilOther(TakeUntil sourceObserver, IDisposable subscription)
				{
					this.sourceObserver = sourceObserver;
					this.subscription = subscription;
				}

				public void OnNext(TOther value)
				{
					lock (sourceObserver.gate)
					{
						try
						{
							sourceObserver.observer.OnCompleted();
						}
						finally
						{
							sourceObserver.Dispose();
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (sourceObserver.gate)
					{
						try
						{
							sourceObserver.observer.OnError(error);
						}
						finally
						{
							sourceObserver.Dispose();
						}
					}
				}

				public void OnCompleted()
				{
					lock (sourceObserver.gate)
					{
						sourceObserver.open = true;
						subscription.Dispose();
					}
				}
			}

			private readonly TakeUntilObservable<T, TOther> parent;

			private object gate = new object();

			private bool open;

			public TakeUntil(TakeUntilObservable<T, TOther> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				TakeUntilOther observer = new TakeUntilOther(this, singleAssignmentDisposable);
				singleAssignmentDisposable.Disposable = parent.other.Subscribe(observer);
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(singleAssignmentDisposable, disposable);
			}

			public override void OnNext(T value)
			{
				if (open)
				{
					observer.OnNext(value);
				}
				else
				{
					lock (gate)
					{
						observer.OnNext(value);
					}
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

		private readonly UniRx.IObservable<TOther> other;

		public TakeUntilObservable(UniRx.IObservable<T> source, UniRx.IObservable<TOther> other)
			: base(source.IsRequiredSubscribeOnCurrentThread() || other.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.other = other;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new TakeUntil(this, observer, cancel).Run();
		}
	}
}
