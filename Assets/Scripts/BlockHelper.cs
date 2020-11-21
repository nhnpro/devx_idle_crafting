using UnityEngine;

public static class BlockHelper
{
	public static float GetBlockHpMultiplier(BlockType type)
	{
		switch (type)
		{
		case BlockType.Grass:
			return PersistentSingleton<GameSettings>.Instance.BlockHpSoftest;
		case BlockType.Dirt:
			return PersistentSingleton<GameSettings>.Instance.BlockHpSoft;
		case BlockType.Wood:
			return PersistentSingleton<GameSettings>.Instance.BlockHpMedium;
		case BlockType.Stone:
			return PersistentSingleton<GameSettings>.Instance.BlockHpMedium;
		case BlockType.Metal:
			return PersistentSingleton<GameSettings>.Instance.BlockHpHardest;
		case BlockType.Gold:
			return PersistentSingleton<GameSettings>.Instance.BlockHpSoftest;
		case BlockType.Diamond:
			return PersistentSingleton<GameSettings>.Instance.BlockHpHardest;
		case BlockType.TNT:
			return PersistentSingleton<GameSettings>.Instance.BlockHpHardest;
		default:
			return PersistentSingleton<GameSettings>.Instance.BlockHpSoftest;
		}
	}

	public static void WarningOnWrongBoxSize(GameObject go)
	{
		if (IsXZInRange(go.GetComponent<BoxCollider>().size, 0.9999f, 1.1f) || IsXZInRange(go.GetComponent<BoxCollider>().size, 1.9999f, 2.1f) || IsXZInRange(go.GetComponent<BoxCollider>().size, 3.9999f, 4.1f))
		{
			UnityEngine.Debug.LogWarning("BoxCollider size should not be unit width/depth");
		}
	}

	private static bool IsXZInRange(Vector3 v, float min, float max)
	{
		return IsInRange(v.x, min, max) || IsInRange(v.z, min, max);
	}

	private static bool IsInRange(float val, float min, float max)
	{
		return min <= val && val <= max;
	}
}
