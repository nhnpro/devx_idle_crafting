using System;

namespace UniRx.Operators
{
	internal class CreateSafeObservable<T> : OperatorObservableBase<T>
	{
		private class CreateSafe : OperatorObserverBase<T, T>
		{
			public CreateSafe(UniRx.IObserver<T> observer, IDisposable cancel)
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

		private readonly Func<IObserver<T>, IDisposable> subscribe;

		public CreateSafeObservable(Func<IObserver<T>, IDisposable> subscribe)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.subscribe = subscribe;
		}

		public CreateSafeObservable(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
			: base(isRequiredSubscribeOnCurrentThread)
		{
			this.subscribe = subscribe;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new CreateSafe(observer, cancel);
			return subscribe(observer) ?? Disposable.Empty;
		}
	}
}
