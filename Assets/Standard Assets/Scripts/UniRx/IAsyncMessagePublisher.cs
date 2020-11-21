namespace UniRx
{
	public interface IAsyncMessagePublisher
	{
		UniRx.IObservable<Unit> PublishAsync<T>(T message);
	}
}
