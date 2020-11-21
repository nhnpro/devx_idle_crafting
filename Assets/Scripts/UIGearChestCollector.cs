using UnityEngine;

public class UIGearChestCollector : MonoBehaviour
{
	public void OnCollectGearChests()
	{
		Singleton<GearMilestoneRunner>.Instance.CollectGearChests(base.transform);
	}
}
