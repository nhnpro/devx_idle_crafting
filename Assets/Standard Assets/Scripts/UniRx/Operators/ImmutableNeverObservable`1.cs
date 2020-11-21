using System;

namespace UniRx.Operators
{
	internal class ImmutableNeverObservable<T> : UniRx.IObservable<T>, IOptimizedObservable<T>
	{
		internal static ImmutableNeverObservable<T> Instance = new ImmutableNeverObservable<T>();

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			return Disposable.Empty;
		}
	}
}
