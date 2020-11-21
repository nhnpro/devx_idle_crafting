using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniRx
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CollectionMoveEvent<T> : IEquatable<CollectionMoveEvent<T>>
	{
		public int OldIndex
		{
			get;
			private set;
		}

		public int NewIndex
		{
			get;
			private set;
		}

		public T Value
		{
			get;
			private set;
		}

		public CollectionMoveEvent(int oldIndex, int newIndex, T value)
		{
			this = default(CollectionMoveEvent<T>);
			OldIndex = oldIndex;
			NewIndex = newIndex;
			Value = value;
		}

		public override string ToString()
		{
			return $"OldIndex:{OldIndex} NewIndex:{NewIndex} Value:{Value}";
		}

		public override int GetHashCode()
		{
			return OldIndex.GetHashCode() ^ (NewIndex.GetHashCode() << 2) ^ (EqualityComparer<T>.Default.GetHashCode(Value) >> 2);
		}

		public bool Equals(CollectionMoveEvent<T> other)
		{
			return OldIndex.Equals(other.OldIndex) && NewIndex.Equals(other.NewIndex) && EqualityComparer<T>.Default.Equals(Value, other.Value);
		}
	}
}
