using System;

namespace UniRx
{
	public interface IObservable<T>
	{
		IDisposable Subscribe(UniRx.IObserver<T> observer);
	}
}
