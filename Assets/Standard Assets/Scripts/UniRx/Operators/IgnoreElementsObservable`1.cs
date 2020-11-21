using System;

namespace UniRx.Operators
{
	internal class IgnoreElementsObservable<T> : OperatorObservableBase<T>
	{
		private class IgnoreElements : OperatorObserverBase<T, T>
		{
			public IgnoreElements(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
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

		public IgnoreElementsObservable(UniRx.IObservable<T> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new IgnoreElements(observer, cancel));
		}
	}
}
