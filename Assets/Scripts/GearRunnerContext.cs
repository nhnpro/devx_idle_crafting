using System.Collections.Generic;
using UnityEngine;

public class GearRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_gearIndex;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("GearIndex"))
		{
			m_gearIndex = (int)parameters["GearIndex"];
		}
		Add("GearRunner", Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(m_gearIndex));
		Add("GearViewRunner", Singleton<GearCollectionRunner>.Instance.GetOrCreateGearViewRunner(m_gearIndex));
	}

	public void OnShowInfoPopup()
	{
		BindingManager.Instance.GearInfoManager.ShowInfo(m_gearIndex);
	}

	public void OnUpgrade()
	{
		Singleton<UpgradeRunner>.Instance.UpgradeGear(m_gearIndex);
	}

	public void OnUpgradeWithGems()
	{
		Singleton<UpgradeRunner>.Instance.UpgradeGearWithGems(m_gearIndex);
	}

	public void OnNotEnoughGemsForUpgrade()
	{
		Singleton<UpgradeRunner>.Instance.NotEnoughGemsForUpgrade(m_gearIndex);
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
	}
}
