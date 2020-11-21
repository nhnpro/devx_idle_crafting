using System;

namespace UniRx.Operators
{
	internal class WhereObservable<T> : OperatorObservableBase<T>
	{
		private class Where : OperatorObserverBase<T, T>
		{
			private readonly WhereObservable<T> parent;

			public Where(WhereObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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
					observer.OnNext(value);
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

		private class Where_ : OperatorObserverBase<T, T>
		{
			private readonly WhereObservable<T> parent;

			private int index;

			public Where_(WhereObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				index = 0;
			}

			public override void OnNext(T value)
			{
				bool flag = false;
				try
				{
					flag = parent.predicateWithIndex(value, index++);
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
					observer.OnNext(value);
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

		private readonly Func<T, int, bool> predicateWithIndex;

		public WhereObservable(UniRx.IObservable<T> source, Func<T, bool> predicate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
		}

		public WhereObservable(UniRx.IObservable<T> source, Func<T, int, bool> predicateWithIndex)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicateWithIndex = predicateWithIndex;
		}

		public UniRx.IObservable<T> CombinePredicate(Func<T, bool> combinePredicate)
		{
			if (predicate != null)
			{
				return new WhereObservable<T>(source, (T x) => predicate(x) && combinePredicate(x));
			}
			return new WhereObservable<T>(this, combinePredicate);
		}

		public UniRx.IObservable<TR> CombineSelector<TR>(Func<T, TR> selector)
		{
			if (predicate != null)
			{
				return new WhereSelectObservable<T, TR>(source, predicate, selector);
			}
			return new SelectObservable<T, TR>(this, selector);
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (predicate != null)
			{
				return source.Subscribe(new Where(this, observer, cancel));
			}
			return source.Subscribe(new Where_(this, observer, cancel));
		}
	}
}
