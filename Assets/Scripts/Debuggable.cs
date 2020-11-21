using System;
using UnityEngine;

public static class Debuggable
{
	public static void debug(this object context, string line, Exception e)
	{
	}

	public static void debug(this object context, string line)
	{
	}

	public static void debugTMP(this object context, string line)
	{
	}

	public static string path(this GameObject go, string leaf = "")
	{
		Transform parent = go.transform.parent;
		string text = go.name + ((!(leaf == string.Empty)) ? ("." + leaf) : string.Empty);
		return (!(parent == null)) ? parent.gameObject.path(text) : text;
	}
}
