namespace UniRx
{
	public interface IMessagePublisher
	{
		void Publish<T>(T message);
	}
}
