namespace UniRx
{
	public interface IGroupedObservable<TKey, TElement> : UniRx.IObservable<TElement>
	{
		TKey Key
		{
			get;
		}
	}
}
