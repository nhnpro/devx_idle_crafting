using UnityEngine;

public class UITournamentTrophyParent : MonoBehaviour
{
	private enum Trophy
	{
		Next,
		Current
	}

	[SerializeField]
	private Trophy m_whichTrophy;

	private void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		if (m_whichTrophy == Trophy.Next && PlayerData.Instance.Trophies.Value < 3)
		{
			GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath(PlayerData.Instance.Trophies.Value + 1), Vector3.zero, Quaternion.identity);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else if (m_whichTrophy == Trophy.Current)
		{
			GameObject gameObject2 = GameObjectExtensions.InstantiateFromResources(GetPrefabPath(PlayerData.Instance.Trophies.Value), Vector3.zero, Quaternion.identity);
			gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
		}
	}

	private string GetPrefabPath(int trophy)
	{
		string str = "UI/Tournaments/TournamentTrophy";
		TournamentTier tournamentTier = (TournamentTier)trophy;
		string str2 = tournamentTier.ToString();
		return str + str2;
	}
}
