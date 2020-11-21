using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIAPBundleProfileParent : MonoBehaviour
{
	[SerializeField]
	private BundleEnum m_bundlePart;

	private RewardData reward;

	protected void OnEnable()
	{
		StartCoroutine(PopulateReward());
	}

	private IEnumerator PopulateReward()
	{
		yield return null;
		base.transform.DestroyChildrenImmediate();
		string[] rewardStringAndPrefabPath = GetRewardStringAndPath();
		Dictionary<string, object> pars = new Dictionary<string, object>
		{
			{
				"GearIndex",
				PlayerData.Instance.CurrentBundleGearIndex.Value
			},
			{
				"StringValue",
				rewardStringAndPrefabPath[0]
			}
		};
		GameObject go = Singleton<PropertyManager>.Instance.InstantiateFromResources(rewardStringAndPrefabPath[1], Vector3.zero, Quaternion.identity, pars);
		go.transform.SetParent(base.transform, worldPositionStays: false);
		if (m_bundlePart == BundleEnum.Main && reward.Type == RewardEnum.AddToGearLevel)
		{
			Singleton<GearCollectionRunner>.Instance.GetOrCreateGearViewRunner(PlayerData.Instance.CurrentBundleGearIndex.Value).BundleBuyAmount.Value = reward.Amount.ToInt();
		}
	}

	private string[] GetRewardStringAndPath()
	{
		switch (m_bundlePart)
		{
		case BundleEnum.Main:
			reward = Singleton<IAPBundleRunner>.Instance.MainReward.Value;
			reward = TransformCoinAmount(reward);
			return Singleton<IAPRunner>.Instance.GetRewardStringAndPrefabPath(reward, mainType: true);
		case BundleEnum.Extra1:
			reward = Singleton<IAPBundleRunner>.Instance.Extra1Reward.Value;
			reward = TransformCoinAmount(reward);
			return Singleton<IAPRunner>.Instance.GetRewardStringAndPrefabPath(reward, mainType: false);
		case BundleEnum.Extra2:
			reward = Singleton<IAPBundleRunner>.Instance.Extra2Reward.Value;
			reward = TransformCoinAmount(reward);
			return Singleton<IAPRunner>.Instance.GetRewardStringAndPrefabPath(reward, mainType: false);
		default:
			return null;
		}
	}

	private RewardData TransformCoinAmount(RewardData reward)
	{
		int value = PlayerData.Instance.LifetimeChunk.Value;
		RewardEnum type = reward.Type;
		if (type == RewardEnum.AddToCoinsSmall || type == RewardEnum.AddToCoinsMedium || type == RewardEnum.AddToCoinsLarge)
		{
			return new RewardData(reward.Type, Singleton<EconomyHelpers>.Instance.GetCoinRewardAmount(reward.Type, value));
		}
		return reward;
	}
}
