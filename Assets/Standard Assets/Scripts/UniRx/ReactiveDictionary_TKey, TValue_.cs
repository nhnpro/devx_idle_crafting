using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace UniRx
{
	[Serializable]
	public class ReactiveDictionary<TKey, TValue> : IReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IEnumerable, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, IDisposable, ISerializable, IDeserializationCallback, IReadOnlyReactiveDictionary<TKey, TValue>, ICollection
	{
		[NonSerialized]
		private bool isDisposed;

		[SerializeField]
		private readonly Dictionary<TKey, TValue> inner;

		private bool disposedValue;

		[NonSerialized]
		private Subject<int> countChanged;

		[NonSerialized]
		private Subject<Unit> collectionReset;

		[NonSerialized]
		private Subject<DictionaryAddEvent<TKey, TValue>> dictionaryAdd;

		[NonSerialized]
		private Subject<DictionaryRemoveEvent<TKey, TValue>> dictionaryRemove;

		[NonSerialized]
		private Subject<DictionaryReplaceEvent<TKey, TValue>> dictionaryReplace;

		object IDictionary.this[object key]
		{
			get
			{
				return this[(TKey)key];
			}
			set
			{
				this[(TKey)key] = (TValue)value;
			}
		}

		bool IDictionary.IsFixedSize => ((IDictionary)inner).IsFixedSize;

		bool IDictionary.IsReadOnly => ((IDictionary)inner).IsReadOnly;

		bool ICollection.IsSynchronized => ((ICollection)inner).IsSynchronized;

		ICollection IDictionary.Keys => ((IDictionary)inner).Keys;

		object ICollection.SyncRoot => ((ICollection)inner).SyncRoot;

		ICollection IDictionary.Values => ((IDictionary)inner).Values;

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)inner).IsReadOnly;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => inner.Keys;

		ICollection<TValue> IDictionary<TKey, TValue>.Values => inner.Values;

		public TValue this[TKey key]
		{
			get
			{
				return inner[key];
			}
			set
			{
				if (TryGetValue(key, out TValue value2))
				{
					inner[key] = value;
					if (dictionaryReplace != null)
					{
						dictionaryReplace.OnNext(new DictionaryReplaceEvent<TKey, TValue>(key, value2, value));
					}
					return;
				}
				inner[key] = value;
				if (dictionaryAdd != null)
				{
					dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
				}
				if (countChanged != null)
				{
					countChanged.OnNext(Count);
				}
			}
		}

		public int Count => inner.Count;

		public Dictionary<TKey, TValue>.KeyCollection Keys => inner.Keys;

		public Dictionary<TKey, TValue>.ValueCollection Values => inner.Values;

		public ReactiveDictionary()
		{
			inner = new Dictionary<TKey, TValue>();
		}

		public ReactiveDictionary(IEqualityComparer<TKey> comparer)
		{
			inner = new Dictionary<TKey, TValue>(comparer);
		}

		public ReactiveDictionary(Dictionary<TKey, TValue> innerDictionary)
		{
			inner = innerDictionary;
		}

		public void Add(TKey key, TValue value)
		{
			inner.Add(key, value);
			if (dictionaryAdd != null)
			{
				dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
			}
			if (countChanged != null)
			{
				countChanged.OnNext(Count);
			}
		}

		public void Clear()
		{
			int count = Count;
			inner.Clear();
			if (collectionReset != null)
			{
				collectionReset.OnNext(Unit.Default);
			}
			if (count > 0 && countChanged != null)
			{
				countChanged.OnNext(Count);
			}
		}

		public bool Remove(TKey key)
		{
			if (inner.TryGetValue(key, out TValue value))
			{
				bool flag = inner.Remove(key);
				if (flag)
				{
					if (dictionaryRemove != null)
					{
						dictionaryRemove.OnNext(new DictionaryRemoveEvent<TKey, TValue>(key, value));
					}
					if (countChanged != null)
					{
						countChanged.OnNext(Count);
					}
				}
				return flag;
			}
			return false;
		}

		public bool ContainsKey(TKey key)
		{
			return inner.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return inner.TryGetValue(key, out value);
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		private void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
		{
			if (subject != null)
			{
				try
				{
					subject.OnCompleted();
				}
				finally
				{
					subject.Dispose();
					subject = null;
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					DisposeSubject(ref countChanged);
					DisposeSubject(ref collectionReset);
					DisposeSubject(ref dictionaryAdd);
					DisposeSubject(ref dictionaryRemove);
					DisposeSubject(ref dictionaryReplace);
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		public UniRx.IObservable<int> ObserveCountChanged()
		{
			if (isDisposed)
			{
				return Observable.Empty<int>();
			}
			return countChanged ?? (countChanged = new Subject<int>());
		}

		public UniRx.IObservable<Unit> ObserveReset()
		{
			if (isDisposed)
			{
				return Observable.Empty<Unit>();
			}
			return collectionReset ?? (collectionReset = new Subject<Unit>());
		}

		public UniRx.IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd()
		{
			if (isDisposed)
			{
				return Observable.Empty<DictionaryAddEvent<TKey, TValue>>();
			}
			return dictionaryAdd ?? (dictionaryAdd = new Subject<DictionaryAddEvent<TKey, TValue>>());
		}

		public UniRx.IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove()
		{
			if (isDisposed)
			{
				return Observable.Empty<DictionaryRemoveEvent<TKey, TValue>>();
			}
			return dictionaryRemove ?? (dictionaryRemove = new Subject<DictionaryRemoveEvent<TKey, TValue>>());
		}

		public UniRx.IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace()
		{
			if (isDisposed)
			{
				return Observable.Empty<DictionaryReplaceEvent<TKey, TValue>>();
			}
			return dictionaryReplace ?? (dictionaryReplace = new Subject<DictionaryReplaceEvent<TKey, TValue>>());
		}

		void IDictionary.Add(object key, object value)
		{
			Add((TKey)key, (TValue)value);
		}

		bool IDictionary.Contains(object key)
		{
			return ((IDictionary)inner).Contains(key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)inner).CopyTo(array, index);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			((ISerializable)inner).GetObjectData(info, context);
		}

		public void OnDeserialization(object sender)
		{
			((IDeserializationCallback)inner).OnDeserialization(sender);
		}

		void IDictionary.Remove(object key)
		{
			Remove((TKey)key);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)inner).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)inner).CopyTo(array, arrayIndex);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)inner).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (TryGetValue(item.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
			{
				Remove(item.Key);
				return true;
			}
			return false;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)inner).GetEnumerator();
		}
	}
}
