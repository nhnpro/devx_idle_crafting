using System;

namespace UniRx.Operators
{
	internal class DoObserverObservable<T> : OperatorObservableBase<T>
	{
		private class Do : OperatorObserverBase<T, T>
		{
			private readonly DoObserverObservable<T> parent;

			public Do(DoObserverObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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
					parent.observer.OnNext(value);
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
					parent.observer.OnError(error);
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
					parent.observer.OnCompleted();
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

		private readonly IObserver<T> observer;

		public DoObserverObservable(UniRx.IObservable<T> source, IObserver<T> observer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.observer = observer;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Do(this, observer, cancel).Run();
		}
	}
}
