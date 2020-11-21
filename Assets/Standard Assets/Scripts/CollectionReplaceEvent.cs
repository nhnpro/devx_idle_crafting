using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniRx
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CollectionReplaceEvent<T> : IEquatable<CollectionReplaceEvent<T>>
	{
		public int Index
		{
			get;
			private set;
		}

		public T OldValue
		{
			get;
			private set;
		}

		public T NewValue
		{
			get;
			private set;
		}

		public CollectionReplaceEvent(int index, T oldValue, T newValue)
		{
			this = default(CollectionReplaceEvent<T>);
			Index = index;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public override string ToString()
		{
			return $"Index:{Index} OldValue:{OldValue} NewValue:{NewValue}";
		}

		public override int GetHashCode()
		{
			return Index.GetHashCode() ^ (EqualityComparer<T>.Default.GetHashCode(OldValue) << 2) ^ (EqualityComparer<T>.Default.GetHashCode(NewValue) >> 2);
		}

		public bool Equals(CollectionReplaceEvent<T> other)
		{
			return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
		}
	}
}
