using System;

namespace UniRx.Operators
{
	internal class FinallyObservable<T> : OperatorObservableBase<T>
	{
		private class Finally : OperatorObserverBase<T, T>
		{
			private readonly FinallyObservable<T> parent;

			public Finally(FinallyObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable disposable;
				try
				{
					disposable = parent.source.Subscribe(this);
				}
				catch
				{
					parent.finallyAction();
					throw;
				}
				return StableCompositeDisposable.Create(disposable, Disposable.Create(delegate
				{
					parent.finallyAction();
				}));
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

		private readonly Action finallyAction;

		public FinallyObservable(UniRx.IObservable<T> source, Action finallyAction)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.finallyAction = finallyAction;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Finally(this, observer, cancel).Run();
		}
	}
}
