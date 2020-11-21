using UniRx;

[PropertyClass]
public class GearViewRunner
{
	public ReactiveProperty<int> BundleBuyAmount = new ReactiveProperty<int>(0);

	[PropertyString]
	public ReactiveProperty<string> LocalizedName;

	[PropertyString]
	public ReactiveProperty<string> LocalizedDesc;

	[PropertyString]
	public ReactiveProperty<string> LocalizedChunkLevelRequirement;

	[PropertyString]
	public ReactiveProperty<string> Boost1Text;

	[PropertyString]
	public ReactiveProperty<string> Boost2Text;

	[PropertyString]
	public ReactiveProperty<string> Boost1Step;

	[PropertyString]
	public ReactiveProperty<string> Boost2Step;

	[PropertyString]
	public ReactiveProperty<string> BundleBoost1;

	[PropertyString]
	public ReactiveProperty<string> BundleBoost2;

	[PropertyBool]
	public ReactiveProperty<bool> Show;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> ShowPrestige;

	public GearViewRunner(int gearIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		GearConfig config = PersistentSingleton<Economies>.Instance.Gears[gearIndex];
		LocalizedName = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Gear.Name." + gearIndex));
		LocalizedDesc = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Gear.Name.Desc." + gearIndex));
		GearRunner gearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gearIndex);
		int chunkUnlockLevel = PersistentSingleton<Economies>.Instance.GearSets[gearRunner.SetIndex].ChunkUnlockLevel;
		LocalizedChunkLevelRequirement = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevelRequirement", chunkUnlockLevel));
		Boost1Text = (from amount in gearRunner.Boost1Amount
			select (gearRunner.Level.Value <= 0) ? config.Boost1.Mult.Amount : amount into amount
			select BonusTypeHelper.GetAttributeText(config.Boost1.Mult.BonusType, amount)).TakeUntilDestroy(instance).ToReactiveProperty();
		Boost2Text = (from amount in gearRunner.Boost2Amount
			select (gearRunner.Level.Value <= 0) ? config.Boost2.Mult.Amount : amount into amount
			select BonusTypeHelper.GetAttributeText(config.Boost2.Mult.BonusType, amount)).TakeUntilDestroy(instance).ToReactiveProperty();
		Show = (from show in Singleton<GearSetCollectionRunner>.Instance.MaxSetsToShow
			select show > gearRunner.SetIndex).TakeUntilDestroy(instance).ToReactiveProperty();
		ShowPrestige = Show.CombineLatest(gearRunner.UpgradeAfterPrestigeAvailable, (bool show, bool avail) => show && avail).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		Boost1Step = new ReactiveProperty<string>(BonusTypeHelper.GetStepText(config.Boost1.Mult.BonusType, config.Boost1.LevelUpAmount));
		Boost2Step = new ReactiveProperty<string>(BonusTypeHelper.GetStepText(config.Boost2.Mult.BonusType, config.Boost2.LevelUpAmount));
		BundleBoost1 = (from comb in BundleBuyAmount.CombineLatest(gearRunner.Level, (int amount, int lvl) => new
			{
				amount,
				lvl
			})
			select (comb.lvl != 0) ? GetBonusAmount(config.Boost1, comb.amount, initial: false) : GetBonusAmount(config.Boost1, comb.amount, initial: true) into amount
			select BonusTypeHelper.GetAttributeText(config.Boost1.Mult.BonusType, amount)).TakeUntilDestroy(instance).ToReactiveProperty();
		BundleBoost2 = (from comb in BundleBuyAmount.CombineLatest(gearRunner.Level, (int amount, int lvl) => new
			{
				amount,
				lvl
			})
			select (comb.lvl != 0) ? GetBonusAmount(config.Boost2, comb.amount, initial: false) : GetBonusAmount(config.Boost2, comb.amount, initial: true) into amount
			select BonusTypeHelper.GetAttributeText(config.Boost2.Mult.BonusType, amount)).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private float GetBonusAmount(GearBoostStruct boost, int boostAmount, bool initial)
	{
		if (initial)
		{
			return boost.Mult.Amount + boost.LevelUpAmount * (float)(boostAmount - 1);
		}
		if (BonusTypeHelper.IsTimeType(boost.Mult.BonusType))
		{
			return boost.LevelUpAmount * (float)boostAmount;
		}
		return boost.LevelUpAmount * (float)boostAmount + 1f;
	}
}
