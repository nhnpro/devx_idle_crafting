using System;
using System.Collections.Generic;

namespace UniRx
{
	[Serializable]
	public struct Pair<T> : IEquatable<Pair<T>>
	{
		private readonly T previous;

		private readonly T current;

		public T Previous => previous;

		public T Current => current;

		public Pair(T previous, T current)
		{
			this.previous = previous;
			this.current = current;
		}

		public override int GetHashCode()
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int hashCode = @default.GetHashCode(previous);
			return ((hashCode << 5) + hashCode) ^ @default.GetHashCode(current);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Pair<T>))
			{
				return false;
			}
			return Equals((Pair<T>)obj);
		}

		public bool Equals(Pair<T> other)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			return @default.Equals(previous, other.Previous) && @default.Equals(current, other.Current);
		}

		public override string ToString()
		{
			return $"({previous}, {current})";
		}
	}
}
