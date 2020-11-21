using System;

namespace UniRx.Operators
{
	internal class AsUnitObservableObservable<T> : OperatorObservableBase<Unit>
	{
		private class AsUnitObservable : OperatorObserverBase<T, Unit>
		{
			public AsUnitObservable(UniRx.IObserver<Unit> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				observer.OnNext(Unit.Default);
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

		public AsUnitObservableObservable(UniRx.IObservable<T> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<Unit> observer, IDisposable cancel)
		{
			return source.Subscribe(new AsUnitObservable(observer, cancel));
		}
	}
}
