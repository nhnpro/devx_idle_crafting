using UnityEngine;

public class UIShowAd : MonoBehaviour
{
	public void OnShowDailyDoubleAd()
	{
		Singleton<AdRunner>.Instance.ShowAd(AdPlacement.DailyDouble);
	}

	public void OnShowHammerTimeAd()
	{
		Singleton<AdRunner>.Instance.ShowAd(AdPlacement.HammerTime);
	}

	public void OnShowKeyChestAd()
	{
		Singleton<AdRunner>.Instance.ShowAd(AdPlacement.KeyChest);
	}

	public void OnShowDrillAd()
	{
		Singleton<AdRunner>.Instance.ShowAd(AdPlacement.Drill);
	}

	public void OnShowLevelSkipAd()
	{
		Singleton<AdRunner>.Instance.ShowAd(AdPlacement.LevelSkip);
	}
}
