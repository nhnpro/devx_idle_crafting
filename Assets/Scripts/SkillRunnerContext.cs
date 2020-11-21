using System.Collections.Generic;
using UnityEngine;

public class SkillRunnerContext : PropertyContext
{
	[SerializeField]
	private SkillsEnum m_skill;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("SkillsEnum"))
		{
			m_skill = (SkillsEnum)parameters["SkillsEnum"];
		}
		Add("SkillRunner", Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(m_skill));
	}

	public void OnSkillActivated()
	{
		Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(m_skill).ActivateSkill();
	}

	public void OnSkillInfo()
	{
		BindingManager.Instance.SkillInfoManager.ShowInfo(m_skill);
	}

	public void OnBuyMore()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(m_skill.ToString(), "buy");
		Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(m_skill).BuyMore(m_skill);
	}

	public void OnWatchAd()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(m_skill.ToString(), "watchAd");
		Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(m_skill).WatchAd();
	}

	public void OnNotEnoughGems()
	{
		Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(m_skill).NotEnoughGems();
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
	}

	public void OnPopupClose()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(m_skill.ToString(), "close");
	}
}
