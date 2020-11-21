using Big;
using UniRx;
using UnityEngine;

[PropertyClass]
public class HeroDamageRunner
{
	private HeroRunner m_heroRunner;

	private HeroConfig m_heroConfig;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> DPS;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> Damage;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> TierDamageMult;

	[PropertyString]
	public ReactiveProperty<string> TierDamageString;

	[PropertyString]
	public ReactiveProperty<string> TierDamageNextUpgrade;

	public ReactiveProperty<bool> CriticalTap = new ReactiveProperty<bool>(initialValue: false);

	public float m_damageTimeStamp;

	public int HeroIndex
	{
		get;
		private set;
	}

	public HeroDamageRunner(int heroIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		HeroIndex = heroIndex;
		m_heroRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(heroIndex);
		m_heroConfig = PersistentSingleton<Economies>.Instance.Heroes[HeroIndex];
		TierDamageMult = (from tier in m_heroRunner.Tier
			select new BigDouble(Singleton<EconomyHelpers>.Instance.GetTierDamageMult(tier))).TakeUntilDestroy(instance).ToReactiveProperty();
		TierDamageString = (from tier in TierDamageMult
			select tier * 100.0 into upgrade
			select PersistentSingleton<LocalizationService>.Instance.Text("Attribute.CompanionDamage", upgrade)).TakeUntilDestroy(instance).ToReactiveProperty();
		TierDamageNextUpgrade = (from tier in m_heroRunner.Tier
			select Singleton<EconomyHelpers>.Instance.GetTierDamageMult(tier + 1) * 100f into upgrade
			select "+" + PersistentSingleton<LocalizationService>.Instance.Text("Attribute.StepMultiplier", upgrade)).TakeUntilDestroy(instance).ToReactiveProperty();
		if (HeroIndex == 0)
		{
			Damage = (from lvl in m_heroRunner.Level
				select GetDamage(lvl)).CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[0], (BigDouble damage, BigDouble mult) => damage * mult).CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[5], (BigDouble damage, BigDouble mult) => damage * mult).CombineLatest(Singleton<PerkMilestoneRunner>.Instance.MiniTapDamageMult, (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(PlayerData.Instance.BoostersEffect[0], (BigDouble damage, float mult) => damage * mult)
				.CombineLatest(Singleton<TeamBoostRunner>.Instance.DamageMult, (BigDouble damage, float mult) => damage * mult)
				.CombineLatest(TierDamageMult, (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(Singleton<HammerTimeRunner>.Instance.Active, (BigDouble damage, bool active) => (!active) ? damage : (damage * PersistentSingleton<GameSettings>.Instance.GoldenHammerMultiplier))
				.CombineLatest(Singleton<TournamentRunner>.Instance.TrophyHeroDamageMultiplier, (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(Singleton<TournamentRunner>.Instance.TrophyAllDamageMultiplier, (BigDouble damage, BigDouble mult) => damage * mult)
				.TakeUntilDestroy(instance)
				.ToReactiveProperty();
		}
		else
		{
			Damage = (from lvl in m_heroRunner.Level
				select GetDamage(lvl)).CombineLatest(Singleton<PerkMilestoneRunner>.Instance.CompanionDamageMult[HeroIndex], (BigDouble dmg, BigDouble compdmg) => dmg * compdmg).CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[0], (BigDouble damage, BigDouble mult) => damage * mult).CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[24], (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[(int)GetBonusTypeForHeroCategory(m_heroConfig.Category)], (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(PlayerData.Instance.BoostersEffect[0], (BigDouble damage, float mult) => damage * mult)
				.CombineLatest(Singleton<TeamBoostRunner>.Instance.DamageMult, (BigDouble damage, float mult) => damage * mult)
				.CombineLatest(TierDamageMult, (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(Singleton<TournamentRunner>.Instance.TrophyCompanionDamageMultiplier, (BigDouble damage, BigDouble mult) => damage * mult)
				.CombineLatest(Singleton<TournamentRunner>.Instance.TrophyAllDamageMultiplier, (BigDouble damage, BigDouble mult) => damage * mult)
				.TakeUntilDestroy(instance)
				.ToReactiveProperty();
		}
		DPS = (from damage in Damage
			select (HeroIndex != 0) ? damage : (damage * 75L)).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private BonusTypeEnum GetBonusTypeForHeroCategory(HeroCategory ctgr)
	{
		switch (ctgr)
		{
		case HeroCategory.GroundMelee:
			return BonusTypeEnum.GroundMeleeCompanionDamage;
		case HeroCategory.GroundRanged:
			return BonusTypeEnum.GroundRangedCompanionDamage;
		case HeroCategory.AirMelee:
			return BonusTypeEnum.AirMeleeCompanionDamage;
		case HeroCategory.AirRanged:
			return BonusTypeEnum.AirRangedCompanionDamage;
		default:
			return BonusTypeEnum.None;
		}
	}

	public void EvaluateDamageAndCritical(out BigDouble damage, out bool critical)
	{
		critical = IsNextCriticalTap();
		BigDouble value = Damage.Value;
		BigDouble right = Singleton<CumulativeBonusRunner>.Instance.BonusMult[3].Value * PersistentSingleton<GameSettings>.Instance.CriticalTapMultBase;
		damage = ((!critical) ? value : (value * right));
	}

	private bool IsNextCriticalTap()
	{
		BigDouble right = Singleton<CumulativeBonusRunner>.Instance.BonusMult[4].Value * PersistentSingleton<GameSettings>.Instance.CriticalTapChanceBase;
		CriticalTap.SetValueAndForceNotify(Random.Range(0f, 1f) <= right);
		return CriticalTap.Value;
	}

	private BigDouble GetDamage(int level)
	{
		if (!m_heroRunner.Found.Value)
		{
			return 0L;
		}
		return BigDouble.Pow(m_heroConfig.DamageMultiplier, level - 1) * m_heroConfig.InitialDamage;
	}

	public void SceneEnterHappened()
	{
		m_damageTimeStamp = Time.realtimeSinceStartup;
	}

	public BigDouble RequestHeroDamage()
	{
		float num = Mathf.Min(1f, Time.realtimeSinceStartup - m_damageTimeStamp);
		m_damageTimeStamp = Time.realtimeSinceStartup;
		return Damage.Value * num;
	}
}
