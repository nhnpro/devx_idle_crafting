using System;

namespace UniRx.Operators
{
	internal class DoObservable<T> : OperatorObservableBase<T>
	{
		private class Do : OperatorObserverBase<T, T>
		{
			private readonly DoObservable<T> parent;

			public Do(DoObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				try
				{
					parent.onNext(value);
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					parent.onError(error);
				}
				catch (Exception error2)
				{
					try
					{
						observer.OnError(error2);
					}
					finally
					{
						Dispose();
					}
					return;
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
				try
				{
					parent.onCompleted();
				}
				catch (Exception error)
				{
					observer.OnError(error);
					Dispose();
					return;
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

		private readonly UniRx.IObservable<T> source;

		private readonly Action<T> onNext;

		private readonly Action<Exception> onError;

		private readonly Action onCompleted;

		public DoObservable(UniRx.IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onNext = onNext;
			this.onError = onError;
			this.onCompleted = onCompleted;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Do(this, observer, cancel).Run();
		}
	}
}
