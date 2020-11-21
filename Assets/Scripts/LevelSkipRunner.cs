using Big;
using System;
using UniRx;
using UnityEngine;

[PropertyClass]
public class LevelSkipRunner : Singleton<LevelSkipRunner>
{
	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> FreeSkipAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> SmallSkipAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> MediumSkipAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> LargeSkipAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> NoSkipsAvailable;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> FreeSkipToLevel;

	[PropertyInt]
	public ReactiveProperty<int> FreeSkipCost = new ReactiveProperty<int>(0);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> SmallSkipToLevel;

	[PropertyInt]
	public ReactiveProperty<int> SmallSkipCost = new ReactiveProperty<int>(0);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> MediumSkipToLevel;

	[PropertyInt]
	public ReactiveProperty<int> MediumSkipCost = new ReactiveProperty<int>(0);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> LargeSkipToLevel;

	[PropertyInt]
	public ReactiveProperty<int> LargeSkipCost = new ReactiveProperty<int>(0);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> SkipToLevel;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> MediumSkipUnlocked;

	[PropertyString]
	public ReactiveProperty<string> MediumSkipUnlockLevel = new ReactiveProperty<string>(string.Empty);

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> LargeSkipUnlocked;

	[PropertyString]
	public ReactiveProperty<string> LargeSkipUnlockLevel = new ReactiveProperty<string>(string.Empty);

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> LevelSkipUnlocked;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> CoinReward = new ReactiveProperty<BigDouble>(BigDouble.ZERO);

	public int[] SkippedCompanions = new int[2];

	public long[] SkippedResources = new long[5];

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AdAvailable;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> AdTimeLeft;

	private ReactiveProperty<int> AdTime;

	public LevelSkipEnum SelectedSkip = LevelSkipEnum.Free;

	public ReactiveProperty<bool> SkippingLevels = new ReactiveProperty<bool>(initialValue: false);

	public LevelSkipRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		FreeSkipAvailable = (from chunk in PlayerData.Instance.MainChunk.CombineLatest(PlayerData.Instance.LifetimeChunk, (int curr, int high) => new
			{
				curr,
				high
			}).Delay(TimeSpan.FromSeconds(1.5))
			select chunk.curr < (int)(Mathf.Max(PersistentSingleton<GameSettings>.Instance.FreeSkipMinimum, (float)chunk.high * PersistentSingleton<GameSettings>.Instance.FreeLevelSkipPercent) * 0.5f)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SmallSkipAvailable = (from small in PlayerData.Instance.LifetimePrestiges.CombineLatest(PlayerData.Instance.LevelSkipsBought[0], (int pres, int skips) => skips)
			select small < PersistentSingleton<GameSettings>.Instance.LevelSkipAmount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MediumSkipAvailable = (from medium in PlayerData.Instance.LifetimePrestiges.CombineLatest(PlayerData.Instance.LevelSkipsBought[1], (int pres, int skips) => skips)
			select medium < PersistentSingleton<GameSettings>.Instance.LevelSkipAmount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		LargeSkipAvailable = (from large in PlayerData.Instance.LifetimePrestiges.CombineLatest(PlayerData.Instance.LevelSkipsBought[2], (int pres, int skips) => skips)
			select large < PersistentSingleton<GameSettings>.Instance.LevelSkipAmount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		NoSkipsAvailable = (from avail in SmallSkipAvailable.CombineLatest(MediumSkipAvailable, LargeSkipAvailable, (bool small, bool medium, bool large) => !small && !medium && !large).Delay(TimeSpan.FromSeconds(1.5))
			select (avail)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		FreeSkipToLevel = (from chunk in PlayerData.Instance.LifetimeChunk
			select PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", (int)Mathf.Max(PersistentSingleton<GameSettings>.Instance.FreeSkipMinimum, (float)chunk * PersistentSingleton<GameSettings>.Instance.FreeLevelSkipPercent))).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		FreeSkipCost.Value = PersistentSingleton<GameSettings>.Instance.FreeLevelSkipCost;
		SmallSkipToLevel = (from lvl in PlayerData.Instance.MainChunk
			select PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", lvl + PersistentSingleton<GameSettings>.Instance.SmallLevelSkipEffect)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SmallSkipCost.Value = PersistentSingleton<GameSettings>.Instance.SmallLevelSkipCost;
		MediumSkipToLevel = (from lvl in PlayerData.Instance.MainChunk
			select PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", lvl + PersistentSingleton<GameSettings>.Instance.MediumLevelSkipEffect)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MediumSkipCost.Value = PersistentSingleton<GameSettings>.Instance.MediumLevelSkipCost;
		LargeSkipToLevel = (from lvl in PlayerData.Instance.MainChunk
			select PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", lvl + PersistentSingleton<GameSettings>.Instance.LargeLevelSkipEffect)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		LargeSkipCost.Value = PersistentSingleton<GameSettings>.Instance.LargeLevelSkipCost;
		SkipToLevel = (from lvl in PlayerData.Instance.MainChunk
			select PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", lvl)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MediumSkipUnlocked = (from lvl in PlayerData.Instance.LifetimeChunk
			select lvl >= PersistentSingleton<GameSettings>.Instance.MediumSkipUnlockLevel).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MediumSkipUnlockLevel.Value = PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", PersistentSingleton<GameSettings>.Instance.MediumSkipUnlockLevel);
		LargeSkipUnlocked = (from lvl in PlayerData.Instance.LifetimeChunk
			select lvl >= PersistentSingleton<GameSettings>.Instance.LargeSkipUnlockLevel).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		LargeSkipUnlockLevel.Value = PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", PersistentSingleton<GameSettings>.Instance.LargeSkipUnlockLevel);
		PlayerData.Instance.LifetimePrestiges.Skip(1).Subscribe(delegate
		{
			for (int i = 0; i < 3; i++)
			{
				PlayerData.Instance.LevelSkipsBought[i].Value = 0;
			}
		}).AddTo(instance);
		LevelSkipUnlocked = PlayerData.Instance.LifetimePrestiges.CombineLatest(PlayerData.Instance.MainChunk, (int prest, int chunk) => prest > 1 || (prest == 1 && chunk >= 5)).ToReadOnlyReactiveProperty().AddTo(instance);
		(from unlocked in LevelSkipUnlocked.Pairwise()
			where !unlocked.Previous && unlocked.Current
			select unlocked).Subscribe(delegate
		{
			BindingManager.Instance.GoatUnlockedParent.ShowInfo();
		}).AddTo(instance);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		AdTime = (from _ in TickerService.MasterTicks
			select (float)Singleton<AdRunner>.Instance.GetNextTimeToShowAd(AdPlacement.LevelSkip).TotalSeconds into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		AdAvailable = (from time in AdTime
			select time <= 0).CombineLatest(Singleton<AdRunner>.Instance.AdReady, (bool time, bool ad) => time && ad).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		AdTimeLeft = AdTime.Select(delegate(int seconds)
		{
			string empty = string.Empty;
			return (seconds > 0) ? TextUtils.FormatSecondsShort(seconds) : PersistentSingleton<LocalizationService>.Instance.Text("AD.Placement.NotAvailable");
		}).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFinished
			where ad == AdPlacement.LevelSkip
			select ad).Subscribe(delegate
		{
			LevelSkipAdFinished();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementSkipped
			where ad == AdPlacement.LevelSkip
			select ad).Subscribe(delegate
		{
			LevelSkipAdSkipped();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFailed
			where ad == AdPlacement.LevelSkip
			select ad).Subscribe(delegate
		{
			LevelSkipAdSkipped();
		}).AddTo(instance);
	}

	public void BuyLevelSkipWithGems()
	{
		int num = 0;
		int skipAmount = 0;
		switch (SelectedSkip)
		{
		case LevelSkipEnum.Free:
			num = PersistentSingleton<GameSettings>.Instance.FreeLevelSkipCost;
			skipAmount = CalculateInitialSkip();
			break;
		case LevelSkipEnum.Small:
			num = PersistentSingleton<GameSettings>.Instance.SmallLevelSkipCost;
			skipAmount = PersistentSingleton<GameSettings>.Instance.SmallLevelSkipEffect;
			break;
		case LevelSkipEnum.Medium:
			num = PersistentSingleton<GameSettings>.Instance.MediumLevelSkipCost;
			skipAmount = PersistentSingleton<GameSettings>.Instance.MediumLevelSkipEffect;
			break;
		case LevelSkipEnum.Large:
			num = PersistentSingleton<GameSettings>.Instance.LargeLevelSkipCost;
			skipAmount = PersistentSingleton<GameSettings>.Instance.LargeLevelSkipEffect;
			break;
		}
		if (PlayerData.Instance.Gems.Value < num)
		{
			NotEnoughGemsForLevelSkip(num);
			BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
			return;
		}
		Singleton<FundRunner>.Instance.RemoveGems(num, "levelSkip_" + SelectedSkip.ToString(), "levelSkips");
		if (SelectedSkip < LevelSkipEnum.SkipsNum)
		{
			PlayerData.Instance.LevelSkipsBought[(int)SelectedSkip].Value++;
		}
		LevelSkip(skipAmount);
	}

	public void NotEnoughGemsForLevelSkip(int gems)
	{
		int missingGems = gems - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}

	public void LevelSkip(int skipAmount)
	{
		int value = PlayerData.Instance.MainChunk.Value;
		SkippingLevels.Value = true;
		for (int i = 0; i < skipAmount; i++)
		{
			Singleton<WorldRunner>.Instance.IncreaseChunk();
		}
		Singleton<WorldRunner>.Instance.LevelSkip();
		(from seq in Singleton<WorldRunner>.Instance.MapSequence
			where !seq
			select seq).Take(1).Subscribe(delegate
		{
			SkippingLevels.Value = false;
		}).AddTo(SceneLoader.Instance);
		int value2 = PlayerData.Instance.MainChunk.Value;
		ChunkGeneratingConfig chunkGeneratingConfig = Singleton<EconomyHelpers>.Instance.GetChunkGeneratingConfig(value2, bossFight: false);
		BigDouble bigDouble = chunkGeneratingConfig.MaxBlocks * Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value.BlockReward * Singleton<CumulativeBonusRunner>.Instance.BonusMult[1].Value * PlayerData.Instance.BoostersEffect[1].Value * PersistentSingleton<GameSettings>.Instance.ChunksMoneysAfterSkip;
		CoinReward.Value = bigDouble;
		Singleton<FundRunner>.Instance.AddCoins(bigDouble);
		int[] array = new int[5];
		for (int j = 0; j < chunkGeneratingConfig.MaxBlocks - chunkGeneratingConfig.GoldBlockAverage; j++)
		{
			array[(int)chunkGeneratingConfig.Materials.AllotObject()]++;
		}
		for (int k = 0; k < array.Length; k++)
		{
			long num = (long)((float)(array[k] * skipAmount) * PersistentSingleton<GameSettings>.Instance.ResourceMultAfterSkip);
			PlayerData.Instance.BlocksInBackpack[k].Value += num;
			SkippedResources[k] = num;
		}
		SkippedCompanions[0] = Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(value);
		SkippedCompanions[1] = Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(value2);
	}

	private int CalculateInitialSkip()
	{
		int b = (int)((float)PlayerData.Instance.LifetimeChunk.Value * PersistentSingleton<GameSettings>.Instance.FreeLevelSkipPercent);
		return Mathf.Max(PersistentSingleton<GameSettings>.Instance.FreeSkipMinimum, b) - PlayerData.Instance.MainChunk.Value;
	}

	private void LevelSkipAdFinished()
	{
		LevelSkip(CalculateInitialSkip());
	}

	private void LevelSkipAdSkipped()
	{
		BindingManager.Instance.GoatEntryParent.ShowInfo();
	}
}
