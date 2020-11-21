using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ToArrayObservable<TSource> : OperatorObservableBase<TSource[]>
	{
		private class ToArray : OperatorObserverBase<TSource, TSource[]>
		{
			private readonly List<TSource> list = new List<TSource>();

			public ToArray(UniRx.IObserver<TSource[]> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(TSource value)
			{
				try
				{
					list.Add(value);
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
				TSource[] value;
				try
				{
					value = list.ToArray();
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

		public ToArrayObservable(UniRx.IObservable<TSource> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TSource[]> observer, IDisposable cancel)
		{
			return source.Subscribe(new ToArray(observer, cancel));
		}
	}
}
