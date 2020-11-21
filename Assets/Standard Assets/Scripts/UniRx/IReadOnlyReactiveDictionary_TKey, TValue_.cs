using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
	public interface IReadOnlyReactiveDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		int Count
		{
			get;
		}

		TValue this[TKey index]
		{
			get;
		}

		bool ContainsKey(TKey key);

		bool TryGetValue(TKey key, out TValue value);

		UniRx.IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd();

		UniRx.IObservable<int> ObserveCountChanged();

		UniRx.IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove();

		UniRx.IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace();

		UniRx.IObservable<Unit> ObserveReset();
	}
}
