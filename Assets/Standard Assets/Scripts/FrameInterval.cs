using System;
using System.Collections.Generic;
using System.Globalization;

namespace UniRx
{
	[Serializable]
	public struct FrameInterval<T> : IEquatable<FrameInterval<T>>
	{
		private readonly int _interval;

		private readonly T _value;

		public T Value => _value;

		public int Interval => _interval;

		public FrameInterval(T value, int interval)
		{
			_interval = interval;
			_value = value;
		}

		public bool Equals(FrameInterval<T> other)
		{
			return other.Interval.Equals(Interval) && EqualityComparer<T>.Default.Equals(Value, other.Value);
		}

		public static bool operator ==(FrameInterval<T> first, FrameInterval<T> second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(FrameInterval<T> first, FrameInterval<T> second)
		{
			return !first.Equals(second);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FrameInterval<T>))
			{
				return false;
			}
			FrameInterval<T> other = (FrameInterval<T>)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			int num = (Value != null) ? Value.GetHashCode() : 1963;
			return Interval.GetHashCode() ^ num;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0}@{1}", Value, Interval);
		}
	}
}
