using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
	public static IEnumerable<GameObject> Children(this GameObject source)
	{
		IEnumerator enumerator = source.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform ct = (Transform)enumerator.Current;
				yield return ct.gameObject;
			}
		}
		finally
		{
			IDisposable disposable;
			IDisposable disposable2 = disposable = (enumerator as IDisposable);
			if (disposable != null)
			{
				disposable2.Dispose();
			}
		}
	}

	public static IEnumerable<T> InfiniteCycle<T>(this T[] contents, long start = 0L)
	{
		int length = contents.Length;
		int index = (int)(start % length) - 1;
		while (true)
		{
			index = (index + 1) % length;
			yield return contents[index];
		}
	}

	public static string GetPath(this GameObject go)
	{
		string text = string.Empty;
		Transform transform = go.transform;
		while (transform != null)
		{
			text = transform.gameObject.name + "/" + text;
			transform = transform.parent;
		}
		return text;
	}

	public static void DestroyChildrenImmediate(this Transform trans)
	{
		for (int num = trans.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.DestroyImmediate(trans.GetChild(num).gameObject);
		}
	}

	public static void DestroyChildrenWithImmediate<T>(this Transform trans)
	{
		for (int num = trans.childCount - 1; num >= 0; num--)
		{
			if (trans.GetChild(num).gameObject.GetComponent<T>() != null)
			{
				UnityEngine.Object.DestroyImmediate(trans.GetChild(num).gameObject);
			}
		}
	}

	public static GameObject InstantiateFromResources(string path)
	{
		GameObject gameObject = Resources.Load(path) as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		return UnityEngine.Object.Instantiate(gameObject);
	}

	public static GameObject InstantiateFromResources(string path, Vector3 pos, Quaternion rot)
	{
		GameObject original = Resources.Load(path) as GameObject;
		return UnityEngine.Object.Instantiate(original, pos, rot);
	}
}
