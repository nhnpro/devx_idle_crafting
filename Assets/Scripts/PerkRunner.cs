using UniRx;

[PropertyClass]
public class PerkRunner
{
	private HeroRunner Hero;

	[PropertyBool]
	public ReactiveProperty<bool> Unlocked;

	[PropertyInt]
	public ReactiveProperty<int> AtLevel;

	[PropertyString]
	public ReactiveProperty<string> LocalizedTitle;

	[PropertyString]
	public ReactiveProperty<string> LocalizedDesc;

	public int PerkIndex
	{
		get;
		private set;
	}

	public PerkRunner(int perkIndex, HeroRunner hero)
	{
		SceneLoader instance = SceneLoader.Instance;
		PerkIndex = perkIndex;
		Hero = hero;
		Unlocked = (from count in Hero.UnlockedPerkCount
			select count > perkIndex).TakeUntilDestroy(instance).ToReactiveProperty();
		AtLevel = new ReactiveProperty<int>(PersistentSingleton<Economies>.Instance.PerkMilestones[PerkIndex]);
		BonusMultConfig bonusMultConfig = PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[Hero.HeroIndex].Items[PerkIndex];
		LocalizedTitle = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Perk." + bonusMultConfig.BonusType + ".Title"));
		LocalizedDesc = new ReactiveProperty<string>(BonusTypeHelper.GetAttributeText(bonusMultConfig.BonusType, bonusMultConfig.Amount));
	}
}
