using System;

namespace UniRx
{
	public interface IConnectableObservable<T> : UniRx.IObservable<T>
	{
		IDisposable Connect();
	}
}
