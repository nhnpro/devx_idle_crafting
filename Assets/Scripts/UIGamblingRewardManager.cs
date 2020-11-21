using Big;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIGamblingRewardManager : MonoBehaviour
{
	protected void Start()
	{
		Singleton<GamblingRunner>.Instance.CurrentGamblingLevel.Subscribe(delegate
		{
			PopulateRewards(Singleton<GamblingRunner>.Instance.Rewards.Value);
		}).AddTo(this);
	}

	private void PopulateRewards(BigDouble[] rewards)
	{
		base.transform.DestroyChildrenImmediate();
		for (int i = 0; i < rewards.Length; i++)
		{
			if (rewards[i] > BigDouble.ZERO)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("StringValue", rewards[i].ToString());
				PropertyManager instance = Singleton<PropertyManager>.Instance;
				RewardEnum rewardEnum = (RewardEnum)i;
				GameObject gameObject = instance.InstantiateFromResources("UI/TimeMachine/MiniRewards/TimeMachineReward_" + rewardEnum.ToString(), Vector3.zero, Quaternion.identity, dictionary);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}
}
