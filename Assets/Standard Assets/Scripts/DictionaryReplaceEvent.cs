using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniRx
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DictionaryReplaceEvent<TKey, TValue> : IEquatable<DictionaryReplaceEvent<TKey, TValue>>
	{
		public TKey Key
		{
			get;
			private set;
		}

		public TValue OldValue
		{
			get;
			private set;
		}

		public TValue NewValue
		{
			get;
			private set;
		}

		public DictionaryReplaceEvent(TKey key, TValue oldValue, TValue newValue)
		{
			this = default(DictionaryReplaceEvent<TKey, TValue>);
			Key = key;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public override string ToString()
		{
			return $"Key:{Key} OldValue:{OldValue} NewValue:{NewValue}";
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ (EqualityComparer<TValue>.Default.GetHashCode(OldValue) << 2) ^ (EqualityComparer<TValue>.Default.GetHashCode(NewValue) >> 2);
		}

		public bool Equals(DictionaryReplaceEvent<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<TValue>.Default.Equals(NewValue, other.NewValue);
		}
	}
}
