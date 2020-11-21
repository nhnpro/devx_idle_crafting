using System.Collections.Generic;

namespace UniRx
{
	public static class ReactivePropertyExtensions
	{
		public static ReactiveProperty<T> ToReactiveProperty<T>(this UniRx.IObservable<T> source)
		{
			return new ReactiveProperty<T>(source);
		}

		public static ReactiveProperty<T> ToReactiveProperty<T>(this UniRx.IObservable<T> source, T initialValue)
		{
			return new ReactiveProperty<T>(source, initialValue);
		}

		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this UniRx.IObservable<T> source)
		{
			return new ReadOnlyReactiveProperty<T>(source);
		}

		public static ReadOnlyReactiveProperty<T> ToSequentialReadOnlyReactiveProperty<T>(this UniRx.IObservable<T> source)
		{
			return new ReadOnlyReactiveProperty<T>(source, distinctUntilChanged: false);
		}

		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this UniRx.IObservable<T> source, T initialValue)
		{
			return new ReadOnlyReactiveProperty<T>(source, initialValue);
		}

		public static ReadOnlyReactiveProperty<T> ToSequentialReadOnlyReactiveProperty<T>(this UniRx.IObservable<T> source, T initialValue)
		{
			return new ReadOnlyReactiveProperty<T>(source, initialValue, distinctUntilChanged: false);
		}

		public static UniRx.IObservable<T> SkipLatestValueOnSubscribe<T>(this IReadOnlyReactiveProperty<T> source)
		{
			return (!source.HasValue) ? source : source.Skip(1);
		}

		public static UniRx.IObservable<bool> CombineLatestValuesAreAllTrue(this IEnumerable<UniRx.IObservable<bool>> sources)
		{
			return sources.CombineLatest().Select(delegate(IList<bool> xs)
			{
				foreach (bool x in xs)
				{
					if (!x)
					{
						return false;
					}
				}
				return true;
			});
		}

		public static UniRx.IObservable<bool> CombineLatestValuesAreAllFalse(this IEnumerable<UniRx.IObservable<bool>> sources)
		{
			return sources.CombineLatest().Select(delegate(IList<bool> xs)
			{
				foreach (bool x in xs)
				{
					if (x)
					{
						return false;
					}
				}
				return true;
			});
		}
	}
}
