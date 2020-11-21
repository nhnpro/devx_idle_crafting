using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIGamblingRewardCardParent : MonoBehaviour
{
	[SerializeField]
	private int rewardIndex;

	private void Start()
	{
		Singleton<GamblingRunner>.Instance.RewardCards.Subscribe(delegate(RewardData[] rewards)
		{
			PopulateRewardCard(rewards[rewardIndex]);
		}).AddTo(this);
	}

	private void PopulateRewardCard(RewardData reward)
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("StringValue", reward.Amount.ToString());
		GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/TimeMachine/Cards/TimeMachineCard_" + reward.Type.ToString(), Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
