using System;

namespace UniRx.Operators
{
	internal class SelectWhereObservable<T, TR> : OperatorObservableBase<TR>
	{
		private class SelectWhere : OperatorObserverBase<T, TR>
		{
			private readonly SelectWhereObservable<T, TR> parent;

			public SelectWhere(SelectWhereObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				TR val = default(TR);
				try
				{
					val = parent.selector(value);
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
				bool flag = false;
				try
				{
					flag = parent.predicate(val);
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
				if (flag)
				{
					observer.OnNext(val);
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

		private readonly UniRx.IObservable<T> source;

		private readonly Func<T, TR> selector;

		private readonly Func<TR, bool> predicate;

		public SelectWhereObservable(UniRx.IObservable<T> source, Func<T, TR> selector, Func<TR, bool> predicate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.selector = selector;
			this.predicate = predicate;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return source.Subscribe(new SelectWhere(this, observer, cancel));
		}
	}
}
