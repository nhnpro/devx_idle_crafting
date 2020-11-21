namespace UniRx
{
	public interface IMessageReceiver
	{
		UniRx.IObservable<T> Receive<T>();
	}
}
