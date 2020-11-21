using System;

namespace UniRx.Operators
{
	internal class DoOnCompletedObservable<T> : OperatorObservableBase<T>
	{
		private class DoOnCompleted : OperatorObserverBase<T, T>
		{
			private readonly DoOnCompletedObservable<T> parent;

			public DoOnCompleted(DoOnCompletedObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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

		private readonly Action onCompleted;

		public DoOnCompletedObservable(UniRx.IObservable<T> source, Action onCompleted)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onCompleted = onCompleted;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnCompleted(this, observer, cancel).Run();
		}
	}
}
