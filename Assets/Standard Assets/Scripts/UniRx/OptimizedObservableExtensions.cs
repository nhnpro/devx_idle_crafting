namespace UniRx
{
	public static class OptimizedObservableExtensions
	{
		public static bool IsRequiredSubscribeOnCurrentThread<T>(this UniRx.IObservable<T> source)
		{
			return (source as IOptimizedObservable<T>)?.IsRequiredSubscribeOnCurrentThread() ?? true;
		}

		public static bool IsRequiredSubscribeOnCurrentThread<T>(this UniRx.IObservable<T> source, IScheduler scheduler)
		{
			if (scheduler == Scheduler.CurrentThread)
			{
				return true;
			}
			return source.IsRequiredSubscribeOnCurrentThread();
		}
	}
}
