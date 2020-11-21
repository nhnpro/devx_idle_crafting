namespace UniRx
{
	public static class LazyTaskExtensions
	{
		public static LazyTask<T> ToLazyTask<T>(this UniRx.IObservable<T> source)
		{
			return new LazyTask<T>(source);
		}
	}
}
