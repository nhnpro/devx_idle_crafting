using System;

namespace UniRx.Operators
{
	internal class AsSingleUnitObservableObservable<T> : OperatorObservableBase<Unit>
	{
		private class AsSingleUnitObservable : OperatorObserverBase<T, Unit>
		{
			public AsSingleUnitObservable(UniRx.IObserver<Unit> observer, IDisposable cancel)
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
				observer.OnNext(Unit.Default);
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

		public AsSingleUnitObservableObservable(UniRx.IObservable<T> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<Unit> observer, IDisposable cancel)
		{
			return source.Subscribe(new AsSingleUnitObservable(observer, cancel));
		}
	}
}
