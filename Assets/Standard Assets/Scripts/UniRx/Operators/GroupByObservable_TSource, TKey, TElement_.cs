using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class GroupByObservable<TSource, TKey, TElement> : OperatorObservableBase<IGroupedObservable<TKey, TElement>>
	{
		private class GroupBy : OperatorObserverBase<TSource, IGroupedObservable<TKey, TElement>>
		{
			private readonly GroupByObservable<TSource, TKey, TElement> parent;

			private readonly Dictionary<TKey, ISubject<TElement>> map;

			private ISubject<TElement> nullKeySubject;

			private CompositeDisposable groupDisposable;

			private RefCountDisposable refCountDisposable;

			public GroupBy(GroupByObservable<TSource, TKey, TElement> parent, IObserver<IGroupedObservable<TKey, TElement>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				if (parent.capacity.HasValue)
				{
					map = new Dictionary<TKey, ISubject<TElement>>(parent.capacity.Value, parent.comparer);
				}
				else
				{
					map = new Dictionary<TKey, ISubject<TElement>>(parent.comparer);
				}
			}

			public IDisposable Run()
			{
				groupDisposable = new CompositeDisposable();
				refCountDisposable = new RefCountDisposable(groupDisposable);
				groupDisposable.Add(parent.source.Subscribe(this));
				return refCountDisposable;
			}

			public override void OnNext(TSource value)
			{
				TKey val = default(TKey);
				try
				{
					val = parent.keySelector(value);
				}
				catch (Exception exception)
				{
					Error(exception);
					return;
				}
				bool flag = false;
				ISubject<TElement> value2 = null;
				try
				{
					if (val == null)
					{
						if (nullKeySubject == null)
						{
							nullKeySubject = new Subject<TElement>();
							flag = true;
						}
						value2 = nullKeySubject;
					}
					else if (!map.TryGetValue(val, out value2))
					{
						value2 = new Subject<TElement>();
						map.Add(val, value2);
						flag = true;
					}
				}
				catch (Exception exception2)
				{
					Error(exception2);
					return;
				}
				if (flag)
				{
					GroupedObservable<TKey, TElement> value3 = new GroupedObservable<TKey, TElement>(val, value2, refCountDisposable);
					observer.OnNext(value3);
				}
				TElement val2 = default(TElement);
				try
				{
					val2 = parent.elementSelector(value);
				}
				catch (Exception exception3)
				{
					Error(exception3);
					return;
				}
				value2.OnNext(val2);
			}

			public override void OnError(Exception error)
			{
				Error(error);
			}

			public override void OnCompleted()
			{
				try
				{
					if (nullKeySubject != null)
					{
						nullKeySubject.OnCompleted();
					}
					foreach (ISubject<TElement> value in map.Values)
					{
						value.OnCompleted();
					}
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}

			private void Error(Exception exception)
			{
				try
				{
					if (nullKeySubject != null)
					{
						nullKeySubject.OnError(exception);
					}
					foreach (ISubject<TElement> value in map.Values)
					{
						value.OnError(exception);
					}
					observer.OnError(exception);
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly UniRx.IObservable<TSource> source;

		private readonly Func<TSource, TKey> keySelector;

		private readonly Func<TSource, TElement> elementSelector;

		private readonly int? capacity;

		private readonly IEqualityComparer<TKey> comparer;

		public GroupByObservable(UniRx.IObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int? capacity, IEqualityComparer<TKey> comparer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.keySelector = keySelector;
			this.elementSelector = elementSelector;
			this.capacity = capacity;
			this.comparer = comparer;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IGroupedObservable<TKey, TElement>> observer, IDisposable cancel)
		{
			return new GroupBy(this, observer, cancel).Run();
		}
	}
}
