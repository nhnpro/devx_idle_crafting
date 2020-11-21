using Big;
using System;
using System.Runtime.CompilerServices;
using UniRx;

[PropertyClass]
public class GearRunner
{
	public const int GearsPerSet = 3;

	[PropertyInt]
	public ReactiveProperty<int> Level;

	[PropertyBool]
	public ReactiveProperty<bool> Unlocked;

	[PropertyBool]
	public ReactiveProperty<bool> UpgradeAvailable;

	[PropertyBool]
	public ReactiveProperty<bool> MaxLevelReached;

	[PropertyBool]
	public ReactiveProperty<bool> SalvageAvailable;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> SalvageRelicAmount;

	[PropertyInt]
	public ReactiveProperty<int> SalvageGemCost;

	public ReactiveProperty<CraftingRequirement> UpgradeRequirement;

	[PropertyInt]
	public ReactiveProperty<int> UpgradeGemCost;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> UpgradeAvailableWithGems;

	[PropertyBool]
	public ReactiveProperty<bool> UpgradeAfterPrestigeAvailable;

	public ReactiveProperty<float> Boost1Amount;

	public ReactiveProperty<float> Boost2Amount;

	[CompilerGenerated]
	private static Func<BigDouble, int> _003C_003Ef__mg_0024cache0;

	public int GearIndex
	{
		get;
		private set;
	}

	public int SetIndex => GearIndex / 3;

	public GearConfig Config
	{
		get;
		private set;
	}

	public GearRunner(int gearIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		GearIndex = gearIndex;
		Config = PersistentSingleton<Economies>.Instance.Gears[gearIndex];
		GearState orCreateGearState = GearStateFactory.GetOrCreateGearState(gearIndex);
		Level = orCreateGearState.Level;
		Level.Skip(1).Subscribe(delegate
		{
			if (PersistentSingleton<GameAnalytics>.Instance != null)
			{
				PersistentSingleton<GameAnalytics>.Instance.GearUpgraded.Value = this;
			}
		}).AddTo(instance);
		SalvageRelicAmount = (from lvl in Level
			select GetSalvageAmount(GearIndex, lvl)).TakeUntilDestroy(instance).ToReactiveProperty();
		SalvageGemCost = SalvageRelicAmount.Select(SalvageRelicsToGems.Evaluate).ToReactiveProperty();
		Unlocked = (from lvl in Level
			select lvl >= 1).TakeUntilDestroy(instance).ToReactiveProperty();
		(from pair in Unlocked.Pairwise()
			where pair.Current && !pair.Previous
			select pair).Subscribe(delegate
		{
			PlayerData.Instance.LifetimeGears.Value++;
		}).AddTo(instance);
		SalvageAvailable = PlayerData.Instance.Gems.CombineLatest(SalvageGemCost, (int gems, int cost) => gems >= cost).TakeUntilDestroy(instance).ToReactiveProperty();
		UpgradeRequirement = (from lvl in Level
			select Singleton<EconomyHelpers>.Instance.GetGearUpgradeCost(GearIndex, lvl)).TakeUntilDestroy(instance).ToReactiveProperty();
		MaxLevelReached = (from level in Level
			select level >= Config.MaxLevel).TakeUntilDestroy(instance).ToReactiveProperty();
		UniRx.IObservable<CraftingRequirement> left = GearCollectionRunner.CreateBlocksObservable();
		UpgradeAvailable = left.CombineLatest(UpgradeRequirement, (CraftingRequirement have, CraftingRequirement req) => have.Satisfies(req)).CombineLatest(MaxLevelReached, (bool sat, bool max) => sat && !max).TakeUntilDestroy(instance)
			.ToReactiveProperty();
		UpgradeGemCost = left.CombineLatest(UpgradeRequirement, (CraftingRequirement have, CraftingRequirement req) => Singleton<EconomyHelpers>.Instance.GetCraftingGemCost(have, req)).TakeUntilDestroy(instance).ToReactiveProperty();
		UpgradeAvailableWithGems = (from comb in UpgradeGemCost.CombineLatest(PlayerData.Instance.Gems, (int cost, int gems) => new
			{
				cost,
				gems
			})
			select (comb.cost <= comb.gems) ? true : false).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		UniRx.IObservable<CraftingRequirement> left2 = GearCollectionRunner.CreateAfterPrestigeBlocksObservable();
		UpgradeAfterPrestigeAvailable = left2.CombineLatest(UpgradeRequirement, (CraftingRequirement have, CraftingRequirement req) => have.Satisfies(req)).CombineLatest(MaxLevelReached, (bool sat, bool max) => sat && !max).TakeUntilDestroy(instance)
			.ToReactiveProperty();
		Boost1Amount = Observable.Return(Config.Boost1.Mult.Amount).CombineLatest(CreateGearBonusObservable(Config.Boost1.Mult.BonusType, Config.Boost1.LevelUpAmount), (float sum, Func<float, float> f) => f(sum)).TakeUntilDestroy(instance)
			.ToReactiveProperty();
		Boost2Amount = Observable.Return(Config.Boost2.Mult.Amount).CombineLatest(CreateGearBonusObservable(Config.Boost2.Mult.BonusType, Config.Boost2.LevelUpAmount), (float sum, Func<float, float> f) => f(sum)).TakeUntilDestroy(instance)
			.ToReactiveProperty();
	}

	private UniRx.IObservable<Func<float, float>> CreateGearBonusObservable(BonusTypeEnum bonusType, float levelUpAmount)
	{
		return from lvl in Level
			select (lvl > 0) ? CreateGearBonusOperator(lvl - 1, levelUpAmount) : BonusTypeHelper.CreateIdentityFunc(bonusType);
	}

	private Func<float, float> CreateGearBonusOperator(int lvl, float levelUpAmount)
	{
		return (float input) => input + levelUpAmount * (float)lvl;
	}

	private BigDouble GetSalvageAmount(int id, int level)
	{
		return new BigDouble(1.0);
	}
}
