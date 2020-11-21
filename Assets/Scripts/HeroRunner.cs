using System.Collections.Generic;
using UniRx;

[PropertyClass]
public class HeroRunner
{
	[PropertyString]
	public ReactiveProperty<string> LocalizedName;

	[PropertyString]
	public ReactiveProperty<string> LocalizedNameDesc;

	[PropertyInt]
	public ReactiveProperty<int> Level;

	[PropertyInt]
	public ReactiveProperty<int> LifetimeLevel;

	[PropertyInt]
	public ReactiveProperty<int> Tier;

	[PropertyInt]
	public ReactiveProperty<int> Rank;

	[PropertyInt]
	public ReactiveProperty<int> Berries;

	[PropertyInt]
	public ReactiveProperty<int> UnusedBerries;

	[PropertyInt]
	public ReactiveProperty<int> BerryIndex = new ReactiveProperty<int>(1);

	[PropertyInt]
	public ReactiveProperty<int> ChunkIndex;

	[PropertyBool]
	public ReactiveProperty<bool> FoundOnce;

	[PropertyBool]
	public ReactiveProperty<bool> Found;

	[PropertyBool]
	public ReactiveProperty<bool> IsNextLevelMilestone;

	[PropertyInt]
	public ReactiveProperty<int> UnlockedPerkCount;

	[PropertyFloat]
	public ReactiveProperty<float> MilestoneProgress;

	[PropertyFloat]
	public ReactiveProperty<float> BerryProgress;

	[PropertyString]
	public ReactiveProperty<string> BerryRequirement;

	[PropertyString]
	public ReactiveProperty<string> NextMilestoneText;

	[PropertyBool]
	public ReactiveProperty<bool> TierUpAvailable;

	public ReadOnlyReactiveProperty<PerkUnlockedInfo> PerkUnlockTriggered;

	public ReadOnlyReactiveProperty<BonusMultConfig> MiniMilestoneTriggered;

	[PropertyString]
	public ReactiveProperty<string> MiniMilestoneTitle;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> MiniMilestoneText;

	public ReadOnlyReactiveProperty<int> HeroTierUpgradeTriggered;

	private HeroState m_heroState;

	private HeroConfig m_heroConfig;

	private List<PerkRunner> m_perkRunners = new List<PerkRunner>();

	private float m_currentMiniMilestoneMultiplier = 1f;

	public int HeroIndex
	{
		get;
		private set;
	}

	public HeroRunner(int heroIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		UIIngameNotifications IngameNotifications = BindingManager.Instance.IngameNotifications;
		HeroIndex = heroIndex;
		BerryIndex.SetValueAndForceNotify(HeroIndex % 5);
		m_heroConfig = PersistentSingleton<Economies>.Instance.Heroes[HeroIndex];
		m_heroState = HeroStateFactory.GetOrCreateHeroState(HeroIndex);
		Level = m_heroState.Level;
		LifetimeLevel = m_heroState.LifetimeLevel;
		Tier = m_heroState.Tier;
		Berries = m_heroState.Berries;
		UnusedBerries = m_heroState.UnusedBerries;
		(from lvl in Level
			where lvl > LifetimeLevel.Value
			select lvl).Subscribe(delegate(int lvl)
		{
			LifetimeLevel.Value = lvl;
		}).AddTo(instance);
		Rank = (from tier in Tier
			select (int)Singleton<EconomyHelpers>.Instance.GetRank(tier)).TakeUntilDestroy(instance).ToReactiveProperty();
		LocalizedName = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Companion.Name." + HeroIndex));
		LocalizedNameDesc = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Companion.Name.Desc." + HeroIndex));
		MiniMilestoneTitle = new ReactiveProperty<string>(LocalizedName + " " + PersistentSingleton<LocalizationService>.Instance.Text("Attribute.Milestone") + "!");
		ChunkIndex = new ReactiveProperty<int>(m_heroConfig.UnlockAtChunk);
		FoundOnce = (from level in LifetimeLevel
			select level >= 1).TakeUntilDestroy(instance).ToReactiveProperty();
		Found = (from level in Level
			select level >= 1).TakeUntilDestroy(instance).ToReactiveProperty();
		IsNextLevelMilestone = (from lvl in Level
			select Singleton<EconomyHelpers>.Instance.IsMilestone(heroIndex, lvl + 1)).TakeUntilDestroy(instance).ToReactiveProperty();
		UnlockedPerkCount = (from heroLevel in Level
			select Singleton<EconomyHelpers>.Instance.GetUnlockedPerkCount(heroLevel)).TakeUntilDestroy(instance).ToReactiveProperty();
		PerkUnlockTriggered = (from pair in UnlockedPerkCount.Pairwise()
			where pair.Previous != pair.Current
			where pair.Current > 0
			select new PerkUnlockedInfo(HeroIndex, pair.Current - 1)).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		if (heroIndex > 0)
		{
			(from _ in PerkUnlockTriggered
				where LifetimeLevel.Value > Level.Value
				select _).Subscribe(delegate(PerkUnlockedInfo perk)
			{
				IngameNotifications.InstantiatePerkNotification(perk);
			}).AddTo(instance);
		}
		MiniMilestoneTriggered = (from lvl in Level.Pairwise()
			select Singleton<EconomyHelpers>.Instance.GetMiniMilestoneOrNull(HeroIndex, lvl.Previous, lvl.Current) into cfg
			where cfg != null
			select cfg).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		MiniMilestoneText = MiniMilestoneTriggered.Select(delegate(BonusMultConfig cfg)
		{
			if (IngameNotifications.CurrentHeroIndex == HeroIndex && IngameNotifications.CurrentNotification != null && IngameNotifications.CurrentNotification.activeSelf)
			{
				m_currentMiniMilestoneMultiplier *= cfg.Amount;
				return GetMilestoneString(cfg.BonusType, m_currentMiniMilestoneMultiplier);
			}
			m_currentMiniMilestoneMultiplier = cfg.Amount;
			return GetMilestoneString(cfg.BonusType, m_currentMiniMilestoneMultiplier);
		}).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		MiniMilestoneText.Subscribe(delegate
		{
			IngameNotifications.InstantiateMiniMilestoneNotification(this);
		}).AddTo(instance);
		HeroTierUpgradeTriggered = (from pair in Tier.Pairwise()
			where pair.Previous != pair.Current
			select HeroIndex).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		MilestoneProgress = (from level in Level
			select Singleton<EconomyHelpers>.Instance.GetMilestoneProgress(level)).TakeUntilDestroy(instance).ToReactiveProperty();
		BerryProgress = (from tier_berry in Tier.CombineLatest(Berries, (int tier, int berries) => new
			{
				tier,
				berries
			})
			select Singleton<EconomyHelpers>.Instance.GetBerryProgress(tier_berry.tier, tier_berry.berries)).TakeUntilDestroy(instance).ToReactiveProperty();
		BerryRequirement = (from tier_berry in Tier.CombineLatest(Berries, (int tier, int berries) => new
			{
				tier,
				berries
			})
			select Singleton<EconomyHelpers>.Instance.GetBerryRequirementText(tier_berry.tier, tier_berry.berries)).TakeUntilDestroy(instance).ToReactiveProperty();
		TierUpAvailable = (from berry_tier in Berries.CombineLatest(UnusedBerries, (int berries, int unused) => berries + unused).CombineLatest(Tier, (int berries, int tier) => new
			{
				berries,
				tier
			})
			select berry_tier.berries >= Singleton<EconomyHelpers>.Instance.GetTierBerryDeltaReq(berry_tier.tier)).TakeUntilDestroy(instance).ToReactiveProperty();
		NextMilestoneText = (from lvl in Level
			where lvl >= 1
			select Singleton<EconomyHelpers>.Instance.GetNextMilestoneText(HeroIndex, lvl)).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private string GetMilestoneString(BonusTypeEnum bonusType, float amount)
	{
		return BonusTypeHelper.GetAttributeText(bonusType, amount);
	}

	public void AddUnusedBerries(int amount)
	{
		UnusedBerries.Value += amount;
	}

	public PerkRunner GetOrCreatePerkRunner(int perk)
	{
		m_perkRunners.EnsureSize(perk, (int count) => new PerkRunner(count, this));
		return m_perkRunners[perk];
	}

	public void ConsumeBerry()
	{
		if (UnusedBerries.Value <= 0)
		{
			return;
		}
		Berries.Value++;
		UnusedBerries.Value--;
		while (Berries.Value >= Singleton<EconomyHelpers>.Instance.GetTierBerryDeltaReq(Tier.Value))
		{
			Berries.Value -= Singleton<EconomyHelpers>.Instance.GetTierBerryDeltaReq(Tier.Value);
			Tier.Value++;
			if (Tier.Value < 5)
			{
				BindingManager.Instance.CompanionEvolveManager.ShowInfo(HeroIndex);
			}
		}
	}

	public void EvolveTiers()
	{
		Berries.Value += UnusedBerries.Value;
		UnusedBerries.Value = 0;
		while (Berries.Value >= Singleton<EconomyHelpers>.Instance.GetTierBerryDeltaReq(Tier.Value))
		{
			Berries.Value -= Singleton<EconomyHelpers>.Instance.GetTierBerryDeltaReq(Tier.Value);
			Tier.Value++;
		}
	}
}
