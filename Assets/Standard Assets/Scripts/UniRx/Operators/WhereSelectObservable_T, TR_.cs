using System;

namespace UniRx.Operators
{
	internal class WhereSelectObservable<T, TR> : OperatorObservableBase<TR>
	{
		private class WhereSelect : OperatorObserverBase<T, TR>
		{
			private readonly WhereSelectObservable<T, TR> parent;

			public WhereSelect(WhereSelectObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				bool flag = false;
				try
				{
					flag = parent.predicate(value);
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
				if (flag)
				{
					TR val = default(TR);
					try
					{
						val = parent.selector(value);
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

		private readonly Func<T, bool> predicate;

		private readonly Func<T, TR> selector;

		public WhereSelectObservable(UniRx.IObservable<T> source, Func<T, bool> predicate, Func<T, TR> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
			this.selector = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return source.Subscribe(new WhereSelect(this, observer, cancel));
		}
	}
}
