using System;

namespace UniRx
{
	internal static class Stubs
	{
		public static readonly Action Nop = delegate
		{
		};

		public static readonly Action<Exception> Throw = delegate(Exception ex)
		{
			throw ex;
		};

		public static UniRx.IObservable<TSource> CatchIgnore<TSource>(Exception ex)
		{
			return Observable.Empty<TSource>();
		}
	}
	internal static class Stubs<T>
	{
		public static readonly Action<T> Ignore = delegate
		{
		};

		public static readonly Func<T, T> Identity = (T t) => t;

		public static readonly Action<Exception, T> Throw = delegate(Exception ex, T _)
		{
			throw ex;
		};
	}
	internal static class Stubs<T1, T2>
	{
		public static readonly Action<T1, T2> Ignore = delegate
		{
		};

		public static readonly Action<Exception, T1, T2> Throw = delegate(Exception ex, T1 _, T2 __)
		{
			throw ex;
		};
	}
	internal static class Stubs<T1, T2, T3>
	{
		public static readonly Action<T1, T2, T3> Ignore = delegate
		{
		};

		public static readonly Action<Exception, T1, T2, T3> Throw = delegate(Exception ex, T1 _, T2 __, T3 ___)
		{
			throw ex;
		};
	}
}
