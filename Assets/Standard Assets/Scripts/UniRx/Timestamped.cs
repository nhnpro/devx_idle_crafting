using System;
using System.Collections.Generic;
using System.Globalization;

namespace UniRx
{
	[Serializable]
	public struct Timestamped<T> : IEquatable<Timestamped<T>>
	{
		private readonly DateTimeOffset _timestamp;

		private readonly T _value;

		public T Value => _value;

		public DateTimeOffset Timestamp => _timestamp;

		public Timestamped(T value, DateTimeOffset timestamp)
		{
			_timestamp = timestamp;
			_value = value;
		}

		public bool Equals(Timestamped<T> other)
		{
			return other.Timestamp.Equals(Timestamp) && EqualityComparer<T>.Default.Equals(Value, other.Value);
		}

		public static bool operator ==(Timestamped<T> first, Timestamped<T> second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(Timestamped<T> first, Timestamped<T> second)
		{
			return !first.Equals(second);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Timestamped<T>))
			{
				return false;
			}
			Timestamped<T> other = (Timestamped<T>)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			int num = (Value != null) ? Value.GetHashCode() : 1979;
			return _timestamp.GetHashCode() ^ num;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0}@{1}", Value, Timestamp);
		}
	}
	public static class Timestamped
	{
		public static Timestamped<T> Create<T>(T value, DateTimeOffset timestamp)
		{
			return new Timestamped<T>(value, timestamp);
		}
	}
}
