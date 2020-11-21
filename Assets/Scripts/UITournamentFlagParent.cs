using UnityEngine;

public class UITournamentFlagParent : MonoBehaviour
{
	protected void Start()
	{
		string prefabPath = GetPrefabPath();
		if (prefabPath != null)
		{
			GameObject gameObject = GameObjectExtensions.InstantiateFromResources(prefabPath);
			if (gameObject != null)
			{
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}

	private string GetPrefabPath()
	{
		LeaderboardEntry leaderboardEntry = (LeaderboardEntry)Singleton<PropertyManager>.Instance.GetContext("LeaderboardEntry", base.transform);
		return leaderboardEntry.PicturePath.Value;
	}
}
