using System;

namespace UniRx.Operators
{
	internal class ImmutableReturnTrueObservable : UniRx.IObservable<bool>, IOptimizedObservable<bool>
	{
		internal static ImmutableReturnTrueObservable Instance = new ImmutableReturnTrueObservable();

		private ImmutableReturnTrueObservable()
		{
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public IDisposable Subscribe(UniRx.IObserver<bool> observer)
		{
			observer.OnNext(value: true);
			observer.OnCompleted();
			return Disposable.Empty;
		}
	}
}
