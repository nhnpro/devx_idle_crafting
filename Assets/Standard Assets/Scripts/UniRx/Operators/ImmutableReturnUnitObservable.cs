using System;

namespace UniRx.Operators
{
	internal class ImmutableReturnUnitObservable : UniRx.IObservable<Unit>, IOptimizedObservable<Unit>
	{
		internal static ImmutableReturnUnitObservable Instance = new ImmutableReturnUnitObservable();

		private ImmutableReturnUnitObservable()
		{
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public IDisposable Subscribe(UniRx.IObserver<Unit> observer)
		{
			observer.OnNext(Unit.Default);
			observer.OnCompleted();
			return Disposable.Empty;
		}
	}
}
