using UnityEngine;

public struct BlockSpawnPoint
{
	public BlockType Type;

	public string PrefabPath;

	public Vector3 Coordinates;

	public BlockSpawnPoint(BlockType type, string prefabPath, Vector3 coordinates)
	{
		Type = type;
		PrefabPath = prefabPath;
		Coordinates = coordinates;
	}
}
