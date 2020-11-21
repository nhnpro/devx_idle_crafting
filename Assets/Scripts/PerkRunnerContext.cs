using System.Collections.Generic;
using UnityEngine;

public class PerkRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_perkIndex;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("PerkIndex"))
		{
			m_perkIndex = (int)parameters["PerkIndex"];
		}
		HeroRunner heroRunner = (HeroRunner)Singleton<PropertyManager>.Instance.GetContext("HeroRunner", base.transform);
		Add("PerkRunner", heroRunner.GetOrCreatePerkRunner(m_perkIndex));
	}

	public void OnShowInfoPopup()
	{
		HeroRunner heroRunner = (HeroRunner)Singleton<PropertyManager>.Instance.GetContext("HeroRunner", base.transform);
		BindingManager.Instance.PerkInfoManager.ShowInfo(heroRunner.HeroIndex, m_perkIndex);
	}
}
