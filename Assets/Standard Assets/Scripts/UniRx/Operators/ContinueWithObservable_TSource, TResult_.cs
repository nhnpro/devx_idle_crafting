using System;

namespace UniRx.Operators
{
	internal class ContinueWithObservable<TSource, TResult> : OperatorObservableBase<TResult>
	{
		private class ContinueWith : OperatorObserverBase<TSource, TResult>
		{
			private readonly ContinueWithObservable<TSource, TResult> parent;

			private readonly SerialDisposable serialDisposable = new SerialDisposable();

			private bool seenValue;

			private TSource lastValue;

			public ContinueWith(ContinueWithObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				serialDisposable.Disposable = singleAssignmentDisposable;
				singleAssignmentDisposable.Disposable = parent.source.Subscribe(this);
				return serialDisposable;
			}

			public override void OnNext(TSource value)
			{
				seenValue = true;
				lastValue = value;
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
				if (seenValue)
				{
					UniRx.IObservable<TResult> observable = parent.selector(lastValue);
					serialDisposable.Disposable = observable.Subscribe(observer);
				}
				else
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
		}

		private readonly UniRx.IObservable<TSource> source;

		private readonly Func<TSource, UniRx.IObservable<TResult>> selector;

		public ContinueWithObservable(UniRx.IObservable<TSource> source, Func<TSource, UniRx.IObservable<TResult>> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.selector = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return new ContinueWith(this, observer, cancel).Run();
		}
	}
}
