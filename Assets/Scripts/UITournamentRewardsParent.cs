using System.Collections.Generic;
using UnityEngine;

public class UITournamentRewardsParent : MonoBehaviour
{
	protected void Start()
	{
		base.transform.DestroyChildrenImmediate();
		ParameterRunner parameterRunner = (ParameterRunner)Singleton<PropertyManager>.Instance.GetContext("ParameterRunner", base.transform);
		TournamentConfig tournamentPriceBracket = Singleton<EconomyHelpers>.Instance.GetTournamentPriceBracket(parameterRunner.IntValue.Value);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		for (int i = 0; i < 3; i++)
		{
			if (tournamentPriceBracket.Rewards[i] != null)
			{
				dictionary.Clear();
				dictionary.Add("IntValue", tournamentPriceBracket.Rewards[i].Amount.ToInt());
				GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources(GetPrefabPath(tournamentPriceBracket.Rewards[i].Type), Vector3.zero, Quaternion.identity, dictionary);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}

	private string GetPrefabPath(RewardEnum type)
	{
		string str = "UI/Tournaments/TournamentReward";
		string str2 = string.Empty;
		switch (type)
		{
		case RewardEnum.AddToBerries:
			str2 = "Berries";
			break;
		case RewardEnum.AddToGems:
			str2 = "Gems";
			break;
		case RewardEnum.AddToMedals:
			str2 = "Tokens";
			break;
		}
		return str + str2;
	}
}
