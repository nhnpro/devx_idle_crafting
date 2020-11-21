namespace UniRx
{
	public interface ISubject<TSource, TResult> : IObserver<TSource>, UniRx.IObservable<TResult>
	{
	}
	public interface ISubject<T> : ISubject<T, T>, IObserver<T>, UniRx.IObservable<T>
	{
	}
}
