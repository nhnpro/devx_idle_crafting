using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIPerkUnlocked : AlwaysStartBehaviour
{
	[SerializeField]
	private Transform m_perkParent;

	[SerializeField]
	private Text m_perkName;

	[SerializeField]
	private Text m_perkAttribute;

	public override void AlwaysStart()
	{
		UniRx.IObservable<PerkUnlockedInfo> observable = Observable.Never<PerkUnlockedInfo>();
		foreach (HeroRunner item in Singleton<HeroTeamRunner>.Instance.Companions())
		{
			observable = observable.Merge(item.PerkUnlockTriggered);
		}
		observable.TakeWhile((PerkUnlockedInfo _) => this != null).Subscribe(delegate(PerkUnlockedInfo perk)
		{
			ShowPerkUnlock(perk.HeroIndex, perk.PerkIndex);
		}).AddTo(this);
	}

	private void ShowPerkUnlock(int hero, int perk)
	{
		if (PlayerData.Instance.HeroStates[hero].Level.Value >= PlayerData.Instance.HeroStates[hero].LifetimeLevel.Value)
		{
			base.gameObject.SetActive(value: true);
			BonusMultConfig bonusMultConfig = PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[hero].Items[perk];
			m_perkParent.transform.DestroyChildrenImmediate();
			GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath(bonusMultConfig));
			gameObject.transform.SetParent(m_perkParent.transform, worldPositionStays: false);
			m_perkName.text = PersistentSingleton<LocalizationService>.Instance.Text("Perk." + bonusMultConfig.BonusType + ".Title");
			m_perkAttribute.text = BonusTypeHelper.GetAttributeText(bonusMultConfig.BonusType, bonusMultConfig.Amount);
		}
	}

	private string GetPrefabPath(BonusMultConfig cfg)
	{
		return "UI/PerkPanels/PerkPanel." + cfg.BonusType;
	}
}
