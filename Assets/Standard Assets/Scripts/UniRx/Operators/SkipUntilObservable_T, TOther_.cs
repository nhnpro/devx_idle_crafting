using System;
using UniRx.InternalUtil;

namespace UniRx.Operators
{
	internal class SkipUntilObservable<T, TOther> : OperatorObservableBase<T>
	{
		private class SkipUntilOuterObserver : OperatorObserverBase<T, T>
		{
			private class SkipUntil : IObserver<T>
			{
				public volatile IObserver<T> observer;

				private readonly SkipUntilOuterObserver parent;

				private readonly IDisposable subscription;

				public SkipUntil(SkipUntilOuterObserver parent, IDisposable subscription)
				{
					this.parent = parent;
					observer = EmptyObserver<T>.Instance;
					this.subscription = subscription;
				}

				public void OnNext(T value)
				{
					observer.OnNext(value);
				}

				public void OnError(Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						parent.Dispose();
					}
				}

				public void OnCompleted()
				{
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						subscription.Dispose();
					}
				}
			}

			private class SkipUntilOther : IObserver<TOther>
			{
				private readonly SkipUntilOuterObserver parent;

				private readonly SkipUntil sourceObserver;

				private readonly IDisposable subscription;

				public SkipUntilOther(SkipUntilOuterObserver parent, SkipUntil sourceObserver, IDisposable subscription)
				{
					this.parent = parent;
					this.sourceObserver = sourceObserver;
					this.subscription = subscription;
				}

				public void OnNext(TOther value)
				{
					sourceObserver.observer = parent.observer;
					subscription.Dispose();
				}

				public void OnError(Exception error)
				{
					try
					{
						parent.observer.OnError(error);
					}
					finally
					{
						parent.Dispose();
					}
				}

				public void OnCompleted()
				{
					subscription.Dispose();
				}
			}

			private readonly SkipUntilObservable<T, TOther> parent;

			public SkipUntilOuterObserver(SkipUntilObservable<T, TOther> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				SkipUntil skipUntil = new SkipUntil(this, singleAssignmentDisposable);
				SingleAssignmentDisposable singleAssignmentDisposable2 = new SingleAssignmentDisposable();
				SkipUntilOther observer = new SkipUntilOther(this, skipUntil, singleAssignmentDisposable2);
				singleAssignmentDisposable.Disposable = parent.source.Subscribe(skipUntil);
				singleAssignmentDisposable2.Disposable = parent.other.Subscribe(observer);
				return StableCompositeDisposable.Create(singleAssignmentDisposable2, singleAssignmentDisposable);
			}

			public override void OnNext(T value)
			{
			}

			public override void OnError(Exception error)
			{
			}

			public override void OnCompleted()
			{
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly UniRx.IObservable<TOther> other;

		public SkipUntilObservable(UniRx.IObservable<T> source, UniRx.IObservable<TOther> other)
			: base(source.IsRequiredSubscribeOnCurrentThread() || other.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.other = other;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new SkipUntilOuterObserver(this, observer, cancel).Run();
		}
	}
}
