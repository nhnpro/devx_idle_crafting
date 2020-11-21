namespace UniRx
{
	public interface IOptimizedObservable<T> : UniRx.IObservable<T>
	{
		bool IsRequiredSubscribeOnCurrentThread();
	}
}
