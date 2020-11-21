using System;

namespace UniRx.Operators
{
	internal class DoOnSubscribeObservable<T> : OperatorObservableBase<T>
	{
		private class DoOnSubscribe : OperatorObserverBase<T, T>
		{
			private readonly DoOnSubscribeObservable<T> parent;

			public DoOnSubscribe(DoOnSubscribeObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				try
				{
					parent.onSubscribe();
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
					return Disposable.Empty;
				}
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
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly Action onSubscribe;

		public DoOnSubscribeObservable(UniRx.IObservable<T> source, Action onSubscribe)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onSubscribe = onSubscribe;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnSubscribe(this, observer, cancel).Run();
		}
	}
}
