using Big;
using UniRx;

[PropertyClass]
public class GoldBoosterRunner
{
	[PropertyInt]
	public ReactiveProperty<int> BoosterCost = new ReactiveProperty<int>();

	[PropertyString]
	public ReactiveProperty<string> BoosterName = new ReactiveProperty<string>();

	[PropertyString]
	public ReactiveProperty<string> BoosterDescription = new ReactiveProperty<string>();

	[PropertyBigDouble]
	public ReadOnlyReactiveProperty<BigDouble> BoosterReward;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BoosterAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> ActiveBooster;

	public int GoldBoosterIndex
	{
		get;
		private set;
	}

	public GoldBoosterRunner(int booster)
	{
		SceneLoader instance = SceneLoader.Instance;
		GoldBoosterIndex = booster;
		BoosterCost.Value = PersistentSingleton<GameSettings>.Instance.GoldBoosterPrices[GoldBoosterIndex];
		BoosterName.Value = PersistentSingleton<LocalizationService>.Instance.Text("GemBooster.Title." + (GoldBoosterEnum)GoldBoosterIndex);
		BoosterDescription.Value = PersistentSingleton<LocalizationService>.Instance.Text("GemBooster.Desc." + (GoldBoosterEnum)GoldBoosterIndex);
		ReadOnlyReactiveProperty<BiomeConfig> source = (from chunk in PlayerData.Instance.LifetimeChunk
			select Singleton<EconomyHelpers>.Instance.GetBiomeConfig(chunk)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BoosterReward = (from biome in source
			select biome.BlockReward * PersistentSingleton<GameSettings>.Instance.GoldBoosterMultipliers[GoldBoosterIndex] into reward
			select BigDouble.Max(reward, PersistentSingleton<GameSettings>.Instance.GoldBoosterMinRewards[GoldBoosterIndex])).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BoosterAvailable = BoosterCost.CombineLatest(PlayerData.Instance.Gems, (int cost, int gems) => cost <= gems).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		ActiveBooster = (from active in Singleton<GoldBoosterCollectionRunner>.Instance.ActiveBooster
			select active == GoldBoosterIndex).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void BuyGoldBooster()
	{
		if (PlayerData.Instance.Gems.Value < BoosterCost.Value)
		{
			NotEnoughGemsForBooster();
			return;
		}
		Singleton<FundRunner>.Instance.RemoveGems(BoosterCost.Value, ((GoldBoosterEnum)GoldBoosterIndex).ToString(), "coinpacks");
		Singleton<FundRunner>.Instance.AddCoins(BoosterReward.Value);
		PersistentSingleton<MainSaver>.Instance.PleaseSave("booster_bought_" + (GoldBoosterEnum)GoldBoosterIndex);
	}

	public void NotEnoughGemsForBooster()
	{
		int missingGems = BoosterCost.Value - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}
}
