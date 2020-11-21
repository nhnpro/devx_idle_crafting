using UniRx;

[PropertyClass]
public class BoosterRunner
{
	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BoosterMaxReached;

	[PropertyInt]
	public ReadOnlyReactiveProperty<int> BoosterCost;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BoosterAvailable;

	[PropertyString]
	public ReactiveProperty<string> BoosterName = new ReactiveProperty<string>(string.Empty);

	[PropertyString]
	public ReactiveProperty<string> BoosterDescription = new ReactiveProperty<string>(string.Empty);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> BoosterBonus;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> BoosterNextUpgrade;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> ActiveBooster;

	private int m_maxBoosterAmount;

	private BoosterConfig m_boosterConfig;

	public int BoosterIndex
	{
		get;
		private set;
	}

	public BoosterRunner(int booster)
	{
		SceneLoader instance = SceneLoader.Instance;
		BoosterIndex = booster;
		m_boosterConfig = PersistentSingleton<Economies>.Instance.Boosters[BoosterIndex];
		if (BoosterIndex == 0)
		{
			m_maxBoosterAmount = PersistentSingleton<GameSettings>.Instance.MaxDamageBoosterPurchaseAmount;
		}
		else
		{
			m_maxBoosterAmount = PersistentSingleton<GameSettings>.Instance.MaxBoosterPurchaseAmount;
		}
		BoosterMaxReached = (from currentAmount in PlayerData.Instance.BoostersBought[BoosterIndex]
			select currentAmount >= m_maxBoosterAmount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BoosterCost = (from amount in PlayerData.Instance.BoostersBought[BoosterIndex]
			select Singleton<EconomyHelpers>.Instance.GetBoosterCost(BoosterIndex, amount)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BoosterAvailable = BoosterCost.CombineLatest(PlayerData.Instance.Gems, (int cost, int gems) => cost <= gems).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		BoosterName.Value = PersistentSingleton<LocalizationService>.Instance.Text("GemBooster.Title." + (BoosterEnum)BoosterIndex);
		BoosterDescription.Value = PersistentSingleton<LocalizationService>.Instance.Text("GemBooster.Desc." + (BoosterEnum)BoosterIndex);
		BoosterBonus = (from _ in PlayerData.Instance.BoostersEffect[BoosterIndex]
			select BoosterCollectionRunner.GetBoosterBonusString((BoosterEnum)BoosterIndex)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BoosterNextUpgrade = (from _ in BoosterCost
			select BoosterCollectionRunner.GetBoosterNextUpgradeString((BoosterEnum)BoosterIndex)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		ActiveBooster = (from active in Singleton<BoosterCollectionRunner>.Instance.ActiveBooster
			select active == BoosterIndex).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void BuyBooster()
	{
		if (PlayerData.Instance.BoostersBought[BoosterIndex].Value < m_maxBoosterAmount)
		{
			if (PlayerData.Instance.Gems.Value < BoosterCost.Value)
			{
				NotEnoughGemsForBooster();
				return;
			}
			Singleton<FundRunner>.Instance.RemoveGems(BoosterCost.Value, ((BoosterEnum)BoosterIndex).ToString(), "boosters");
			PlayerData.Instance.BoostersEffect[BoosterIndex].Value += m_boosterConfig.RewardAmount;
			PlayerData.Instance.BoostersBought[BoosterIndex].Value++;
			PersistentSingleton<MainSaver>.Instance.PleaseSave("booster_bought_" + (BoosterEnum)BoosterIndex);
		}
	}

	public void NotEnoughGemsForBooster()
	{
		int missingGems = BoosterCost.Value - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}
}
