using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITournamentsAdditionalRewardsParent : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(PopulateRewards());
	}

	private IEnumerator PopulateRewards()
	{
		yield return null;
		base.transform.DestroyChildrenImmediate();
		TournamentConfig config = Singleton<EconomyHelpers>.Instance.GetTournamentPriceBracket(Singleton<TournamentRunner>.Instance.PlayerRank.Value);
		Dictionary<string, object> pars = new Dictionary<string, object>();
		for (int i = 0; i < 3; i++)
		{
			if (config.Rewards[i] == null)
			{
				continue;
			}
			string prefabPath = GetPrefabPath(config.Rewards[i].Type);
			if (prefabPath != string.Empty)
			{
				int num = config.Rewards[i].Amount.ToInt();
				if (num > 0)
				{
					pars.Clear();
					pars.Add("IntValue", num);
					GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources(prefabPath, Vector3.zero, Quaternion.identity, pars);
					gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				}
			}
		}
	}

	private string GetPrefabPath(RewardEnum type)
	{
		string result = string.Empty;
		switch (type)
		{
		case RewardEnum.AddToBerries:
			result = "UI/Tournaments/TournamentItems/TournamentItem_AddToBerries";
			break;
		case RewardEnum.AddToGems:
			result = "UI/Tournaments/TournamentItems/TournamentItem_AddToGems";
			break;
		}
		return result;
	}
}
