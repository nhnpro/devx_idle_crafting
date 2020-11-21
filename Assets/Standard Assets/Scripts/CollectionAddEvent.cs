using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniRx
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CollectionAddEvent<T> : IEquatable<CollectionAddEvent<T>>
	{
		public int Index
		{
			get;
			private set;
		}

		public T Value
		{
			get;
			private set;
		}

		public CollectionAddEvent(int index, T value)
		{
			this = default(CollectionAddEvent<T>);
			Index = index;
			Value = value;
		}

		public override string ToString()
		{
			return $"Index:{Index} Value:{Value}";
		}

		public override int GetHashCode()
		{
			return Index.GetHashCode() ^ (EqualityComparer<T>.Default.GetHashCode(Value) << 2);
		}

		public bool Equals(CollectionAddEvent<T> other)
		{
			return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
		}
	}
}
