using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class DistinctUntilChangedObservable<T> : OperatorObservableBase<T>
	{
		private class DistinctUntilChanged : OperatorObserverBase<T, T>
		{
			private readonly DistinctUntilChangedObservable<T> parent;

			private bool isFirst = true;

			private T prevKey = default(T);

			public DistinctUntilChanged(DistinctUntilChangedObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				try
				{
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
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					try
					{
						flag = parent.comparer.Equals(value, prevKey);
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
				}
				if (!flag)
				{
					prevKey = value;
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

		private readonly IEqualityComparer<T> comparer;

		public DistinctUntilChangedObservable(UniRx.IObservable<T> source, IEqualityComparer<T> comparer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.comparer = comparer;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new DistinctUntilChanged(this, observer, cancel));
		}
	}
	internal class DistinctUntilChangedObservable<T, TKey> : OperatorObservableBase<T>
	{
		private class DistinctUntilChanged : OperatorObserverBase<T, T>
		{
			private readonly DistinctUntilChangedObservable<T, TKey> parent;

			private bool isFirst = true;

			private TKey prevKey = default(TKey);

			public DistinctUntilChanged(DistinctUntilChangedObservable<T, TKey> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				TKey x;
				try
				{
					x = parent.keySelector(value);
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
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					try
					{
						flag = parent.comparer.Equals(x, prevKey);
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
				}
				if (!flag)
				{
					prevKey = x;
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

		private readonly IEqualityComparer<TKey> comparer;

		private readonly Func<T, TKey> keySelector;

		public DistinctUntilChangedObservable(UniRx.IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.comparer = comparer;
			this.keySelector = keySelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new DistinctUntilChanged(this, observer, cancel));
		}
	}
}
