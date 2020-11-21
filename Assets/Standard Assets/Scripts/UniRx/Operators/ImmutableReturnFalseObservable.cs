using System;

namespace UniRx.Operators
{
	internal class ImmutableReturnFalseObservable : UniRx.IObservable<bool>, IOptimizedObservable<bool>
	{
		internal static ImmutableReturnFalseObservable Instance = new ImmutableReturnFalseObservable();

		private ImmutableReturnFalseObservable()
		{
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public IDisposable Subscribe(UniRx.IObserver<bool> observer)
		{
			observer.OnNext(value: false);
			observer.OnCompleted();
			return Disposable.Empty;
		}
	}
}
