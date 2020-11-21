using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
	public interface IReadOnlyReactiveCollection<T> : IEnumerable<T>, IEnumerable
	{
		int Count
		{
			get;
		}

		T this[int index]
		{
			get;
		}

		UniRx.IObservable<CollectionAddEvent<T>> ObserveAdd();

		UniRx.IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);

		UniRx.IObservable<CollectionMoveEvent<T>> ObserveMove();

		UniRx.IObservable<CollectionRemoveEvent<T>> ObserveRemove();

		UniRx.IObservable<CollectionReplaceEvent<T>> ObserveReplace();

		UniRx.IObservable<Unit> ObserveReset();
	}
}
