using Big;
using System;
using System.Runtime.CompilerServices;

public static class TSVExtracting
{
	[CompilerGenerated]
	private static Func<string, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<string, float> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<string, int> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<string, long> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Func<string, double> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Func<string, BigDouble> _003C_003Ef__mg_0024cache5;

	public static bool nonEmpty(this string[] row, int index)
	{
		return row.Length > index && row[index].Trim() != string.Empty;
	}

	public static bool asBool(this string[] row, int index, Func<string, bool> orDefault)
	{
		return Get(row, index, bool.Parse, orDefault);
	}

	public static float asFloat(this string[] row, int index, Func<string, float> orDefault)
	{
		return Get(row, index, float.Parse, orDefault);
	}

	public static int asInt(this string[] row, int index, Func<string, int> orDefault)
	{
		return Get(row, index, int.Parse, orDefault);
	}

	public static long asLong(this string[] row, int index, Func<string, long> orDefault)
	{
		return Get(row, index, long.Parse, orDefault);
	}

	public static double asDouble(this string[] row, int index, Func<string, double> orDefault)
	{
		return Get(row, index, double.Parse, orDefault);
	}

	public static string asString(this string[] row, int index, Func<string, string> orDefault)
	{
		return Get(row, index, (string f) => f, orDefault);
	}

	public static BigDouble asBigDouble(this string[] row, int index, Func<string, BigDouble> orDefault)
	{
		return Get(row, index, BigDouble.Parse, orDefault);
	}

	public static T asEnum<T>(this string[] row, int index, Func<string, T> orDefault) where T : struct
	{
		return Get(row, index, RXExt.ToEnum<T>, orDefault);
	}

	public static T asCustom<T>(this string[] row, int index, Func<string, T> convert, Func<string, T> orDefault)
	{
		return Get(row, index, (string f) => convert(f), orDefault);
	}

	public static T Get<T>(string[] row, int index, Func<string, T> convert, Func<string, T> orDefault)
	{
		string text = null;
		try
		{
			if (row.Length < index)
			{
				string arg = "Tried to get col " + index + " from " + string.Join(",", row) + ", it was not found, row length: " + row.Length + ". Returning default value.";
				return orDefault(arg);
			}
			text = row[index];
			return convert(text);
		}
		catch (Exception ex)
		{
			string arg2 = "Exception during converting col " + index + ":\"" + text + "\" from " + string.Join(",", row) + ", row length: " + row.Length + ". Returning default value, Exception: " + ex.Message;
			return orDefault(arg2);
		}
	}

	public static Func<string, T> toError<T>(this string[] row)
	{
		return delegate(string message)
		{
			throw new SystemException(message);
		};
	}
}
