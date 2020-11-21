using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ToListObservable<TSource> : OperatorObservableBase<IList<TSource>>
	{
		private class ToList : OperatorObserverBase<TSource, IList<TSource>>
		{
			private readonly List<TSource> list = new List<TSource>();

			public ToList(UniRx.IObserver<IList<TSource>> observer, IDisposable cancel)
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
				observer.OnNext(list);
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

		public ToListObservable(UniRx.IObservable<TSource> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<TSource>> observer, IDisposable cancel)
		{
			return source.Subscribe(new ToList(observer, cancel));
		}
	}
}
