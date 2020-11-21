using UnityEngine;

public class UIRewardCollector : MonoBehaviour
{
	public void OnCollectChestReward()
	{
		Singleton<CollectableRewardRunner>.Instance.CollectChestReward(base.transform);
	}

	public void OnCollectRandomReward()
	{
		Singleton<CollectableRewardRunner>.Instance.CollectRandomReward(base.transform);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
