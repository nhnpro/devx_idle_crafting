using Big;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class PerkMilestoneRunner : Singleton<PerkMilestoneRunner>
{
	public ReactiveProperty<BigDouble>[] BonusMult = new ReactiveProperty<BigDouble>[25];

	public ReactiveProperty<BigDouble>[] CompanionDamageMult;

	public ReactiveProperty<BigDouble> MiniTapDamageMult;

	public PerkMilestoneRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		for (int i = 0; i < 25; i++)
		{
			BonusMult[i] = CreateObservableForAll((BonusTypeEnum)i).TakeUntilDestroy(instance).ToReactiveProperty();
		}
		MiniTapDamageMult = (from level in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(0).Level
			select GetMiniDamageMult(PersistentSingleton<Economies>.Instance.HeroMiniMilestones, level, isHero: true)).TakeUntilDestroy(instance).ToReactiveProperty();
		CompanionDamageMult = new ReactiveProperty<BigDouble>[Singleton<EconomyHelpers>.Instance.GetNumHeroes()];
		for (int j = 1; j < CompanionDamageMult.Length; j++)
		{
			CompanionDamageMult[j] = CreateCompanionDamage(j).TakeUntilDestroy(instance).ToReactiveProperty();
		}
	}

	private UniRx.IObservable<BigDouble> CreateCompanionDamage(int i)
	{
		return from level in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(i).Level
			select GetDamageMult(BonusTypeEnum.CompanionDamage, i, level) * GetMiniDamageMult(PersistentSingleton<Economies>.Instance.CompanionMiniMilestones, level, isHero: false);
	}

	private UniRx.IObservable<BigDouble> CreateObservableForAll(BonusTypeEnum bonusType)
	{
		List<UniRx.IObservable<float>> multipliers = PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs.SelectMany((PerkMilestoneConfig config) => from i in Enumerable.Range(0, config.Items.Count)
			where config.Items[i].BonusType == bonusType
			select CreateHeroBonusMultiplier(bonusType, config.Hero, config.Items[i].Amount, PersistentSingleton<Economies>.Instance.PerkMilestones[i])).ToList();
		return from mult in BonusTypeHelper.CreateCombine(bonusType, multipliers)
			select new BigDouble(mult);
	}

	private UniRx.IObservable<float> CreateHeroBonusMultiplier(BonusTypeEnum bonusType, int heroIndex, float mult, int milestone)
	{
		HeroRunner orCreateHeroRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(heroIndex);
		return from level in orCreateHeroRunner.Level
			select (level < milestone) ? BonusTypeHelper.GetOrigin(bonusType) : mult;
	}

	private BigDouble GetDamageMult(BonusTypeEnum bonusId, int heroIndex, int level)
	{
		List<BonusMultConfig> items = PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[heroIndex].Items;
		BigDouble bigDouble = new BigDouble(1.0);
		for (int i = 0; i < items.Count && PersistentSingleton<Economies>.Instance.PerkMilestones[i] <= level; i++)
		{
			if (items[i].BonusType == bonusId)
			{
				bigDouble *= items[i].Amount;
			}
		}
		return bigDouble;
	}

	private BigDouble GetMiniDamageMult(List<BonusMultConfig> configs, int level, bool isHero)
	{
		BigDouble bigDouble = new BigDouble(1.0);
		int i = PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep;
		int num = 0;
		for (; i <= level; i += PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep)
		{
			if (isHero || !PersistentSingleton<Economies>.Instance.PerkMilestones.Contains(i))
			{
				int index = num % configs.Count;
				BonusMultConfig bonusMultConfig = configs[index];
				bigDouble *= bonusMultConfig.Amount;
			}
			num++;
		}
		return bigDouble;
	}
}
