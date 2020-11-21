namespace UniRx
{
	public interface IReadOnlyReactiveProperty<T> : UniRx.IObservable<T>
	{
		T Value
		{
			get;
		}

		bool HasValue
		{
			get;
		}
	}
}
