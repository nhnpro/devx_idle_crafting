using System;
using System.Collections.Generic;

public static class ListExtensions
{
	public static void EnsureSize<T>(this List<T> list, int size, Func<int, T> ctor)
	{
		for (int i = list.Count; i <= size; i++)
		{
			list.Add(ctor(i));
		}
	}

	public static void EnsureContains<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> ctor)
	{
		if (!dict.ContainsKey(key))
		{
			dict.Add(key, ctor());
		}
	}
}
