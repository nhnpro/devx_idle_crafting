using System;
using System.Linq;

namespace Big
{
	[Serializable]
	public class BigDouble : IComparable<BigDouble>
	{
		public const double SQRT_10 = 3.16227766016838;

		public static readonly BigDouble ZERO = new BigDouble(0.0, long.MinValue);

		public static readonly BigDouble PositiveInfinity = new BigDouble(double.PositiveInfinity, long.MaxValue);

		public static readonly BigDouble NegativeInfinity = new BigDouble(double.NegativeInfinity, long.MaxValue);

		public static readonly BigDouble MaxFloat = new BigDouble(1.0, 37L);

		public static readonly BigDouble MaxDouble = new BigDouble(1.0, 307L);

		public readonly double numerator;

		public readonly long exponent;

		public BigDouble(double numerator)
		{
			BigDouble bigDouble = create(numerator, 0L);
			this.numerator = bigDouble.numerator;
			exponent = bigDouble.exponent;
		}

		protected BigDouble(double numerator, long exponent)
		{
			this.numerator = numerator;
			this.exponent = exponent;
		}

		public static BigDouble create(double numerator, long exponent = 0L)
		{
			if (numerator == 0.0)
			{
				return ZERO;
			}
			if ((numerator >= 1.0 && numerator < 10.0) || (numerator <= -1.0 && numerator > -10.0))
			{
				return new BigDouble(numerator, exponent);
			}
			if (numerator == double.PositiveInfinity)
			{
				return PositiveInfinity;
			}
			if (numerator == double.NegativeInfinity)
			{
				return NegativeInfinity;
			}
			return normalize(numerator, exponent);
		}

		private static BigDouble normalize(double numerator, long exponent)
		{
			while (numerator < 1.0 && numerator > -1.0)
			{
				numerator *= 10.0;
				exponent--;
			}
			while (numerator >= 10.0 || numerator <= -10.0)
			{
				numerator *= 0.1;
				exponent++;
			}
			return new BigDouble(numerator, exponent);
		}

		public BigDouble Floor(int precision)
		{
			if (Equals(ZERO))
			{
				return ZERO;
			}
			double num = (exponent >= precision) ? Math.Pow(10.0, precision) : Math.Pow(10.0, exponent);
			double num2 = Math.Floor(numerator * num) * 1.0 / num;
			return new BigDouble(num2, exponent);
		}

		public override string ToString()
		{
			return ToString(31);
		}

		public string ToString(int startingExponent)
		{
			if (this == ZERO)
			{
				return "0";
			}
			if (exponent < startingExponent && exponent > -startingExponent)
			{
				return string.Empty + numerator * Math.Pow(10.0, exponent);
			}
			string text = (exponent < 0) ? string.Empty : "+";
			return numerator + "E" + text + exponent;
		}

		public static BigDouble Parse(string scientific)
		{
			int num = scientific.IndexOf("E");
			int length = (num <= 0) ? scientific.Count() : num;
			double num2 = double.Parse(scientific.Substring(0, length));
			long num3 = (num <= 0) ? 0 : long.Parse(scientific.Substring(num + 1));
			return create(num2, num3);
		}

		public BigDouble Round(int precision = 0)
		{
			double num = Math.Round(numerator, precision, MidpointRounding.AwayFromZero);
			return create(num, exponent);
		}

		public int ToInt()
		{
			float num = ToFloat();
			return (num > 2.14748365E+09f) ? int.MaxValue : ((!(num < -2.14748365E+09f)) ? Convert.ToInt32(num) : int.MinValue);
		}

		public long ToLong()
		{
			double num = ToDouble();
			return (num > 9.2233720368547758E+18) ? long.MaxValue : ((!(num < -9.2233720368547758E+18)) ? Convert.ToInt64(num) : long.MinValue);
		}

		public float ToFloat()
		{
			BigDouble bigDouble = (!(this < MaxFloat)) ? MaxFloat : this;
			return (float)(bigDouble.numerator * Math.Pow(10.0, bigDouble.exponent));
		}

		public double ToDouble()
		{
			BigDouble bigDouble = (!(this < MaxDouble)) ? MaxDouble : this;
			return bigDouble.numerator * Math.Pow(10.0, bigDouble.exponent);
		}

		public int CompareTo(BigDouble another)
		{
			if (this > another)
			{
				return 1;
			}
			if (this == another)
			{
				return 0;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			return numerator == ((BigDouble)obj).numerator && exponent == ((BigDouble)obj).exponent;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public BigDouble RoundSmall(int expToRound = 200)
		{
			if (exponent <= expToRound)
			{
				return create(Math.Round(numerator, (int)exponent, MidpointRounding.AwayFromZero), exponent);
			}
			return this;
		}

		public static implicit operator BigDouble(long value)
		{
			return create(value, 0L);
		}

		public static implicit operator BigDouble(double value)
		{
			return create(value, 0L);
		}

		public static bool operator >(BigDouble left, BigDouble right)
		{
			return (left.exponent > right.exponent) ? (left.numerator > 0.0) : ((left.exponent != right.exponent) ? (right.numerator < 0.0) : (left.numerator > right.numerator));
		}

		public static bool operator <(BigDouble left, BigDouble right)
		{
			return right > left;
		}

		public static bool operator >=(BigDouble left, BigDouble right)
		{
			return (left.exponent > right.exponent) ? (left.numerator > 0.0) : ((left.exponent != right.exponent) ? (right.numerator < 0.0) : (left.numerator >= right.numerator));
		}

		public static bool operator <=(BigDouble left, BigDouble right)
		{
			return right >= left;
		}

		public static bool operator ==(BigDouble left, BigDouble right)
		{
			return left.numerator == right.numerator && left.exponent == right.exponent;
		}

		public static bool operator !=(BigDouble left, BigDouble right)
		{
			return left.numerator != right.numerator || left.exponent != right.exponent;
		}

		public static BigDouble operator *(BigDouble left, BigDouble right)
		{
			return create(left.numerator * right.numerator, left.exponent + right.exponent);
		}

		public static BigDouble operator /(BigDouble left, BigDouble right)
		{
			if (right == ZERO)
			{
				return (!(left.numerator >= 0.0)) ? NegativeInfinity : PositiveInfinity;
			}
			return create(left.numerator / right.numerator, left.exponent - right.exponent);
		}

		private static BigDouble PlusLeftBigger(BigDouble bigger, BigDouble smaller)
		{
			if (smaller == ZERO)
			{
				return bigger;
			}
			double num = smaller.numerator * Math.Pow(10.0, smaller.exponent - bigger.exponent);
			return create(bigger.numerator + num, bigger.exponent);
		}

		public static BigDouble operator +(BigDouble left, BigDouble right)
		{
			return (left.exponent < right.exponent) ? PlusLeftBigger(right, left) : PlusLeftBigger(left, right);
		}

		public static BigDouble operator -(BigDouble left, BigDouble right)
		{
			return (left.exponent < right.exponent) ? PlusLeftBigger(-right, left) : PlusLeftBigger(left, -right);
		}

		public static BigDouble operator -(BigDouble left)
		{
			return create(0.0 - left.numerator, left.exponent);
		}

		public static BigDouble operator ++(BigDouble left)
		{
			return left + 1L;
		}

		public static BigDouble operator --(BigDouble left)
		{
			return left - 1L;
		}

		public BigDouble Pow(double exp)
		{
			return Pow(this, exp);
		}

		public static BigDouble Pow(BigDouble num, double exp)
		{
			double num2 = (double)num.exponent * exp % 1.0;
			double d = Math.Pow(num.numerator, exp) * Math.Pow(10.0, num2);
			if (double.IsInfinity(d))
			{
				BigDouble bigDouble = Pow(num, exp / 2.0);
				return bigDouble * bigDouble;
			}
			long num3 = (long)(exp * (double)num.exponent - num2);
			return create(d, num3);
		}

		public BigDouble Sqrt()
		{
			return Sqrt(this);
		}

		public static BigDouble Sqrt(BigDouble d)
		{
			bool flag = d.exponent % 2 == 0;
			double num = (!flag) ? 3.16227766016838 : 1.0;
			int num2 = (!flag && d.exponent < 0) ? (-1) : 0;
			long num3 = d.exponent / 2 + num2;
			double num4 = Math.Sqrt(d.numerator) * num;
			return create(num4, num3);
		}

		public static BigDouble Max(BigDouble a, BigDouble b)
		{
			return (!(a >= b)) ? b : a;
		}

		public static BigDouble Min(BigDouble a, BigDouble b)
		{
			return (!(a <= b)) ? b : a;
		}
	}
}
