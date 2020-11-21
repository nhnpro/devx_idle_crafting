using System.Collections.Generic;
using UnityEngine;

public class UIBossRewardManager : MonoBehaviour
{
	protected void Start()
	{
		base.transform.DestroyChildrenImmediate();
		foreach (RewardAction rewardAction in Singleton<BossSuccessRunner>.Instance.RewardActions)
		{
			GameObject gameObject = CreateCard(rewardAction);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}
	}

	private GameObject CreateCard(RewardAction action)
	{
		switch (action.Reward.Type)
		{
		case RewardEnum.AddToCoins:
			return CreateCoinCard((AddCoinRewardAction)action);
		case RewardEnum.AddToRelics:
			return CreateRelicsCard((RelicRewardAction)action);
		default:
			return null;
		}
	}

	private GameObject CreateCoinCard(AddCoinRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("BigDoubleValue", action.Reward.Amount);
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/BossRewardItems/BossReward_Coins", Vector3.zero, Quaternion.identity, dictionary);
	}

	private GameObject CreateRelicsCard(RelicRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("BigDoubleValue", action.Reward.Amount);
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/BossRewardItems/BossReward_Relics", Vector3.zero, Quaternion.identity, dictionary);
	}
}
