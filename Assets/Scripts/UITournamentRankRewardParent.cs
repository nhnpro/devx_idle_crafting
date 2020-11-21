using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITournamentRankRewardParent : MonoBehaviour
{
	private enum Placement
	{
		Current,
		Next,
		All,
		Top3
	}

	[SerializeField]
	private Placement m_placement;

	private void OnEnable()
	{
		StartCoroutine(PopulateRewards());
	}

	private IEnumerator PopulateRewards()
	{
		yield return null;
		base.transform.DestroyChildrenImmediate();
		int rank = Singleton<TournamentRunner>.Instance.PlayerRank.Value;
		List<TournamentConfig> cards = new List<TournamentConfig>();
		switch (m_placement)
		{
		case Placement.Current:
			cards.Add(Singleton<EconomyHelpers>.Instance.GetTournamentPriceBracket(rank));
			break;
		case Placement.Next:
			cards.Add(Singleton<EconomyHelpers>.Instance.GetNextTournamentPriceBracket(rank));
			break;
		case Placement.All:
			for (int k = 0; k < PersistentSingleton<Economies>.Instance.Tournaments.Count; k++)
			{
				cards.Add(PersistentSingleton<Economies>.Instance.Tournaments[k]);
			}
			break;
		case Placement.Top3:
			for (int j = 0; j < 3; j++)
			{
				cards.Add(PersistentSingleton<Economies>.Instance.Tournaments[j]);
			}
			break;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			CreateRewardCard(cards[i].Rank);
			yield return null;
		}
	}

	private void CreateRewardCard(int rank)
	{
		string str = rank.ToString();
		string arg = string.Empty;
		if (rank > 3)
		{
			str = "Normal";
			arg = (Singleton<EconomyHelpers>.Instance.GetNextTournamentPriceBracket(rank).Rank + 1).ToString() + "-";
		}
		string value = arg + rank;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", rank);
		dictionary.Add("StringValue", value);
		GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/Tournaments/TournamentRankReward" + str, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
