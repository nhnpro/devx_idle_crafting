using System;

namespace UniRx.Operators
{
	internal class SelectObservable<T, TR> : OperatorObservableBase<TR>, ISelect<TR>
	{
		private class Select : OperatorObserverBase<T, TR>
		{
			private readonly SelectObservable<T, TR> parent;

			public Select(SelectObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
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

		private class Select_ : OperatorObserverBase<T, TR>
		{
			private readonly SelectObservable<T, TR> parent;

			private int index;

			public Select_(SelectObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				index = 0;
			}

			public override void OnNext(T value)
			{
				TR val = default(TR);
				try
				{
					val = parent.selectorWithIndex(value, index++);
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

		private readonly UniRx.IObservable<T> source;

		private readonly Func<T, TR> selector;

		private readonly Func<T, int, TR> selectorWithIndex;

		public SelectObservable(UniRx.IObservable<T> source, Func<T, TR> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.selector = selector;
		}

		public SelectObservable(UniRx.IObservable<T> source, Func<T, int, TR> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			selectorWithIndex = selector;
		}

		public UniRx.IObservable<TR> CombinePredicate(Func<TR, bool> predicate)
		{
			if (selector != null)
			{
				return new SelectWhereObservable<T, TR>(source, selector, predicate);
			}
			return new WhereObservable<TR>(this, predicate);
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			if (selector != null)
			{
				return source.Subscribe(new Select(this, observer, cancel));
			}
			return source.Subscribe(new Select_(this, observer, cancel));
		}
	}
}
