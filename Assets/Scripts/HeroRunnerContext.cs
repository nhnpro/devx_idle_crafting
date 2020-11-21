using System.Collections.Generic;
using UnityEngine;

public class HeroRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_heroIndex;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("HeroIndex"))
		{
			m_heroIndex = (int)parameters["HeroIndex"];
		}
		Add("HeroIndex", m_heroIndex);
		if (m_heroIndex != -1)
		{
			Add("HeroRunner", Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex));
			Add("HeroDamageRunner", Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(m_heroIndex));
			Add("HeroCostRunner", Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroCostRunner(m_heroIndex));
		}
	}

	public void OnHeroUpgrade()
	{
		Singleton<UpgradeRunner>.Instance.UpgradeHero(m_heroIndex);
	}

	public void OnFeedBerry()
	{
		Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex).ConsumeBerry();
	}

	public void OnFeedBerries()
	{
		Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex).EvolveTiers();
	}

	public void OnShowInfoPopup()
	{
		if (m_heroIndex == 0)
		{
			BindingManager.Instance.HeroInfoManager.ShowInfo();
		}
		else
		{
			BindingManager.Instance.CompanionInfoManager.ShowInfo(m_heroIndex);
		}
	}
}
