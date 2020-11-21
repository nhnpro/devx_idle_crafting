using System;

public static class TextUtils
{
	public static string FormatSeconds(long secs)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(secs);
		int num = (int)timeSpan.TotalHours;
		string str = (num <= 0) ? string.Empty : $"{num}h ";
		string str2 = (num <= 0 && timeSpan.Minutes <= 0) ? string.Empty : $"{timeSpan.Minutes}m ";
		string str3 = Math.Max(0, timeSpan.Seconds) + "s ";
		return str + str2 + str3;
	}

	public static string FormatSecondsShort(long secs)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(secs);
		int num = (int)timeSpan.TotalHours;
		string str = (num <= 0) ? string.Empty : $"{num}h ";
		string text = (num <= 0 && timeSpan.Minutes <= 0) ? string.Empty : $"{timeSpan.Minutes}m ";
		string text2 = Math.Max(0, timeSpan.Seconds) + "s ";
		if (num > 0)
		{
			return str + text;
		}
		if (timeSpan.Minutes > 0)
		{
			return text + text2;
		}
		return text2;
	}

	public static string FormatSecondsShortWithDays(long secs)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(secs);
		int num = (int)timeSpan.TotalDays;
		int hours = timeSpan.Hours;
		string str = (num <= 0) ? string.Empty : $"{num}d ";
		string text = (num <= 0 && hours <= 0) ? string.Empty : $"{hours}h ";
		string text2 = (hours <= 0 && timeSpan.Minutes <= 0) ? string.Empty : $"{timeSpan.Minutes}m ";
		string text3 = Math.Max(0, timeSpan.Seconds) + "s ";
		if (num > 0)
		{
			return str + text;
		}
		if (hours > 0)
		{
			return text + text2;
		}
		if (timeSpan.Minutes > 0)
		{
			return text2 + text3;
		}
		return text3;
	}
}
