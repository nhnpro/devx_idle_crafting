using System;

namespace UniRx.Operators
{
	internal class AsObservableObservable<T> : OperatorObservableBase<T>
	{
		private class AsObservable : OperatorObserverBase<T, T>
		{
			public AsObservable(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
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

		private readonly UniRx.IObservable<T> source;

		public AsObservableObservable(UniRx.IObservable<T> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new AsObservable(observer, cancel));
		}
	}
}
