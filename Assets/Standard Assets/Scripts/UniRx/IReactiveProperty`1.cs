namespace UniRx
{
	public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>, UniRx.IObservable<T>
	{
		new T Value
		{
			get;
			set;
		}
	}
}
