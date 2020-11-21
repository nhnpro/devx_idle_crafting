using System;

namespace UniRx.Operators
{
	internal class DoOnCancelObservable<T> : OperatorObservableBase<T>
	{
		private class DoOnCancel : OperatorObserverBase<T, T>
		{
			private readonly DoOnCancelObservable<T> parent;

			private bool isCompletedCall;

			public DoOnCancel(DoOnCancelObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return StableCompositeDisposable.Create(parent.source.Subscribe(this), Disposable.Create(delegate
				{
					if (!isCompletedCall)
					{
						parent.onCancel();
					}
				}));
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				isCompletedCall = true;
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
				isCompletedCall = true;
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

		private readonly Action onCancel;

		public DoOnCancelObservable(UniRx.IObservable<T> source, Action onCancel)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onCancel = onCancel;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnCancel(this, observer, cancel).Run();
		}
	}
}
