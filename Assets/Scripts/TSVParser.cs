using System.Collections.Generic;
using System.Linq;

public static class TSVParser
{
	public static IEnumerable<string[]> Parse(string input)
	{
		List<string[]> list = new List<string[]>();
		int length = input.Length;
		List<string> list2 = new List<string>();
		int num2;
		for (int num = 0; num < length; num = num2 + 1)
		{
			bool flag = input[num] == '"';
			if (flag)
			{
				num++;
			}
			num2 = ((!flag) ? WordEnd(input, num, '\t', '\n') : WordEnd(input, num, '"'));
			list2.Add(input.Substring(num, num2 - num));
			if (flag)
			{
				num2++;
			}
			if (IsRowEnd(input, num2))
			{
				list.Add((from v in list2
					select v.Trim()).ToArray());
				list2 = new List<string>();
			}
		}
		if (list2.Count() > 0)
		{
			list.Add((from v in list2
				select v.Trim()).ToArray());
		}
		return list;
	}

	private static int WordEnd(string input, int start, char stop)
	{
		int length = input.Length;
		for (int i = start; i < length; i++)
		{
			if (input[i] == stop)
			{
				return i;
			}
		}
		return length;
	}

	private static int WordEnd(string input, int start, char stop1, char stop2)
	{
		int length = input.Length;
		for (int i = start; i < length; i++)
		{
			char c = input[i];
			if (c == stop1 || c == stop2)
			{
				return i;
			}
		}
		return length;
	}

	private static bool IsRowEnd(string input, int index)
	{
		return index >= input.Length || input[index] == '\n';
	}
}
