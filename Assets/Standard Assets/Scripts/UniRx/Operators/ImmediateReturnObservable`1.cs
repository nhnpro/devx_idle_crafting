using System;

namespace UniRx.Operators
{
	internal class ImmediateReturnObservable<T> : UniRx.IObservable<T>, IOptimizedObservable<T>
	{
		private readonly T value;

		public ImmediateReturnObservable(T value)
		{
			this.value = value;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			observer.OnNext(value);
			observer.OnCompleted();
			return Disposable.Empty;
		}
	}
}
