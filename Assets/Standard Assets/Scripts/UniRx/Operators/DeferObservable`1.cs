using System;

namespace UniRx.Operators
{
	internal class DeferObservable<T> : OperatorObservableBase<T>
	{
		private class Defer : OperatorObserverBase<T, T>
		{
			public Defer(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				try
				{
					observer.OnNext(value);
				}
				catch
				{
					Dispose();
					throw;
				}
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

		private readonly Func<UniRx.IObservable<T>> observableFactory;

		public DeferObservable(Func<UniRx.IObservable<T>> observableFactory)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.observableFactory = observableFactory;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new Defer(observer, cancel);
			UniRx.IObservable<T> observable;
			try
			{
				observable = observableFactory();
			}
			catch (Exception error)
			{
				observable = Observable.Throw<T>(error);
			}
			return observable.Subscribe(observer);
		}
	}
}
