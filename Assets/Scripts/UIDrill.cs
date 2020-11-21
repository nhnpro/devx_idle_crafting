using UnityEngine;

public class UIDrill : MonoBehaviour
{
	public void OnCollectDrillReward()
	{
		Singleton<DrillRunner>.Instance.CollectDrillReward();
	}
}
