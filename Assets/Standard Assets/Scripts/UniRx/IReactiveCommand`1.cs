namespace UniRx
{
	public interface IReactiveCommand<T> : UniRx.IObservable<T>
	{
		IReadOnlyReactiveProperty<bool> CanExecute
		{
			get;
		}

		bool Execute(T parameter);
	}
}
