using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class DistinctObservable<T> : OperatorObservableBase<T>
	{
		private class Distinct : OperatorObserverBase<T, T>
		{
			private readonly HashSet<T> hashSet;

			public Distinct(DistinctObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				hashSet = ((parent.comparer != null) ? new HashSet<T>(parent.comparer) : new HashSet<T>());
			}

			public override void OnNext(T value)
			{
				T val = default(T);
				bool flag = false;
				try
				{
					flag = hashSet.Add(value);
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

		private readonly IEqualityComparer<T> comparer;

		public DistinctObservable(UniRx.IObservable<T> source, IEqualityComparer<T> comparer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.comparer = comparer;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new Distinct(this, observer, cancel));
		}
	}
	internal class DistinctObservable<T, TKey> : OperatorObservableBase<T>
	{
		private class Distinct : OperatorObserverBase<T, T>
		{
			private readonly DistinctObservable<T, TKey> parent;

			private readonly HashSet<TKey> hashSet;

			public Distinct(DistinctObservable<T, TKey> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				hashSet = ((parent.comparer != null) ? new HashSet<TKey>(parent.comparer) : new HashSet<TKey>());
			}

			public override void OnNext(T value)
			{
				TKey val = default(TKey);
				bool flag = false;
				try
				{
					val = parent.keySelector(value);
					flag = hashSet.Add(val);
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

		private readonly IEqualityComparer<TKey> comparer;

		private readonly Func<T, TKey> keySelector;

		public DistinctObservable(UniRx.IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.comparer = comparer;
			this.keySelector = keySelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new Distinct(this, observer, cancel));
		}
	}
}
