using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniRx
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DictionaryAddEvent<TKey, TValue> : IEquatable<DictionaryAddEvent<TKey, TValue>>
	{
		public TKey Key
		{
			get;
			private set;
		}

		public TValue Value
		{
			get;
			private set;
		}

		public DictionaryAddEvent(TKey key, TValue value)
		{
			this = default(DictionaryAddEvent<TKey, TValue>);
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return $"Key:{Key} Value:{Value}";
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ (EqualityComparer<TValue>.Default.GetHashCode(Value) << 2);
		}

		public bool Equals(DictionaryAddEvent<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
		}
	}
}
