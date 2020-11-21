using System;

namespace UniRx
{
	public interface IAsyncMessageReceiver
	{
		IDisposable Subscribe<T>(Func<T, UniRx.IObservable<Unit>> asyncMessageReceiver);
	}
}
