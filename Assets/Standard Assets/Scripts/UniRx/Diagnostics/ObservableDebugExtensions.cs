namespace UniRx.Diagnostics
{
	public static class ObservableDebugExtensions
	{
		public static UniRx.IObservable<T> Debug<T>(this UniRx.IObservable<T> source, string label = null)
		{
			return source;
		}

		public static UniRx.IObservable<T> Debug<T>(this UniRx.IObservable<T> source, Logger logger)
		{
			return source;
		}
	}
}
