using System;
using System.Text;

namespace Big
{
	public class BigString
	{
		public static readonly string[] names = new string[31]
		{
			string.Empty,
			"K",
			"M",
			"B",
			"T",
			"aa",
			"bb",
			"cc",
			"dd",
			"ee",
			"ff",
			"gg",
			"hh",
			"ii",
			"jj",
			"kk",
			"ll",
			"mm",
			"nn",
			"oo",
			"pp",
			"qq",
			"rr",
			"ss",
			"tt",
			"uu",
			"vv",
			"ww",
			"xx",
			"yy",
			"zz"
		};

		private static readonly int[] multiplier_exponents = new int[3]
		{
			1,
			10,
			100
		};

		private static readonly int maxExponent = names.Length * 3;

		public static string ToString(BigDouble d, int precision = 2)
		{
			if (d == BigDouble.ZERO)
			{
				return "0";
			}
			if (d == BigDouble.PositiveInfinity)
			{
				return "INF";
			}
			if (d == BigDouble.NegativeInfinity)
			{
				return "-INF";
			}
			if (d.exponent < 0)
			{
				return "0.0";
			}
			int num = (int)d.exponent % 3;
			long num2 = d.exponent - num;
			string format = (d.exponent >= 3) ? ("F" + (precision - num)) : string.Empty;
			string text = Math.Round(d.numerator * (double)multiplier_exponents[num], precision - num).ToString(format);
			if (num2 == 0)
			{
				return text;
			}
			string str = (num2 >= maxExponent) ? doubleLetterCurrency(num2) : names[num2 / 3];
			string str2 = string.Empty;
			if (num2 > maxExponent - 1)
			{
				str2 = "+";
			}
			return text + str + str2;
		}

		public static string doubleLetterCurrency(long exp)
		{
			long num = (exp - maxExponent) / 3;
			StringBuilder stringBuilder = new StringBuilder();
			char c = 'a';
			while (true)
			{
				char value = (char)((int)c + num % 26);
				stringBuilder.Insert(0, value);
				if (stringBuilder.Length >= 2 && num < 26)
				{
					break;
				}
				num /= 26;
				if (stringBuilder.Length >= 2)
				{
					num--;
				}
				c = 'A';
			}
			return stringBuilder.ToString();
		}
	}
}
