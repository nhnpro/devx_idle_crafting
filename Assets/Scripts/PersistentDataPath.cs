using UnityEngine;

public static class PersistentDataPath
{
	public const bool AndroidUsesExternal = false;

	public static string Get()
	{
		return Application.persistentDataPath;
	}
}
