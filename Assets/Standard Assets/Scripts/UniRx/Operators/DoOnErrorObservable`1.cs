using System;

namespace UniRx.Operators
{
	internal class DoOnErrorObservable<T> : OperatorObservableBase<T>
	{
		private class DoOnError : OperatorObserverBase<T, T>
		{
			private readonly DoOnErrorObservable<T> parent;

			public DoOnError(DoOnErrorObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly Action<Exception> onError;

		public DoOnErrorObservable(UniRx.IObservable<T> source, Action<Exception> onError)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onError = onError;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnError(this, observer, cancel).Run();
		}
	}
}
