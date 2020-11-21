using System;

namespace UniRx.Operators
{
	internal class CastObservable<TSource, TResult> : OperatorObservableBase<TResult>
	{
		private class Cast : OperatorObserverBase<TSource, TResult>
		{
			public Cast(UniRx.IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(TSource value)
			{
				TResult val = default(TResult);
				try
				{
					val = (TResult)(object)value;
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
				observer.OnNext(val);
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

		public CastObservable(UniRx.IObservable<TSource> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return source.Subscribe(new Cast(observer, cancel));
		}
	}
}
