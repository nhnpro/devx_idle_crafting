using System;

namespace UniRx.Operators
{
	internal class OfTypeObservable<TSource, TResult> : OperatorObservableBase<TResult>
	{
		private class OfType : OperatorObserverBase<TSource, TResult>
		{
			public OfType(UniRx.IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(TSource value)
			{
				if (value is TResult)
				{
					TResult value2 = (TResult)(object)value;
					observer.OnNext(value2);
				}
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

		private readonly UniRx.IObservable<TSource> source;

		public OfTypeObservable(UniRx.IObservable<TSource> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return source.Subscribe(new OfType(observer, cancel));
		}
	}
}
