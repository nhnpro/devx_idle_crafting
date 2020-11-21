using Big;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class DrillRunner : Singleton<DrillRunner>
{
	private int m_maxDrillAds = 7;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DrillCollectable;

	[PropertyBool]
	public ReactiveProperty<bool> DrillAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DrillAdAvailable;

	private ReactiveProperty<int> DrillAdCooldown;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DrillAdCooldownLeft;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DurationLeft;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DrillMaxLevel;

	private ReactiveProperty<bool> UpdateTimer = new ReactiveProperty<bool>(initialValue: true);

	public List<string[]> Rewards = new List<string[]>();

	public DrillRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		DrillAvailable = (from synced in ServerTimeService.IsSynced
			select (synced)).TakeUntilDestroy(instance).ToReactiveProperty();
		UniRx.IObservable<int> observable = (from upd in TickerService.MasterTicksSlow.CombineLatest(UpdateTimer, (long ticker, bool upd) => upd)
			where upd
			select upd into _
			select (float)new TimeSpan(PlayerData.Instance.DrillTimeStamp.Value - ServerTimeService.NowTicks()).TotalSeconds into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged();
		DurationLeft = (from seconds in observable
			select TextUtils.FormatSecondsShort(seconds)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrillCollectable = observable.CombineLatest(PlayerData.Instance.DrillLevel, (int dur, int lvl) => dur == 0 && lvl > 0).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrillMaxLevel = (from lvl in PlayerData.Instance.DrillLevel
			select lvl >= m_maxDrillAds).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		DrillAdCooldown = (from _ in TickerService.MasterTicksSlow
			select (float)Singleton<AdRunner>.Instance.GetNextTimeToShowAd(AdPlacement.Drill).TotalSeconds into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		DrillAdAvailable = (from avail in DrillAdCooldown.CombineLatest(PlayerData.Instance.DrillLevel, (int cd, int lvl) => cd <= 0 && lvl < m_maxDrillAds)
			select (avail)).CombineLatest(Singleton<AdRunner>.Instance.AdReady, (bool time, bool ad) => time && ad).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrillAdCooldownLeft = DrillAdCooldown.Select(delegate(int seconds)
		{
			string empty = string.Empty;
			return (seconds > 0) ? TextUtils.FormatSecondsShort(seconds) : PersistentSingleton<LocalizationService>.Instance.Text("AD.Placement.NotAvailable");
		}).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFinished
			where ad == AdPlacement.Drill
			select ad).Subscribe(delegate
		{
			DrillAdFinished();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementSkipped
			where ad == AdPlacement.Drill
			select ad).Subscribe(delegate
		{
			DrillAdSkipped();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFailed
			where ad == AdPlacement.Drill
			select ad).Subscribe(delegate
		{
			DrillAdSkipped();
		}).AddTo(instance);
	}

	private void DrillAdFinished()
	{
		if (PlayerData.Instance.DrillTimeStamp.Value == 0)
		{
			PlayerData.Instance.DrillTimeStamp.Value = ServerTimeService.NowTicks();
		}
		long num = Math.Max(PlayerData.Instance.DrillTimeStamp.Value - ServerTimeService.NowTicks(), 0L);
		PlayerData.Instance.DrillTimeStamp.Value = ServerTimeService.NowTicks() + num + TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.DrillDuration).Ticks;
		PlayerData.Instance.DrillLevel.Value++;
		UpdateTimer.SetValueAndForceNotify(value: true);
		BindingManager.Instance.DrillSuccessParent.ShowInfo();
	}

	private void DrillAdSkipped()
	{
		if (PlayerData.Instance.DrillLevel.Value < 1)
		{
			BindingManager.Instance.DrillEntryParent.ShowInfo();
		}
	}

	public void CollectDrillReward()
	{
		Rewards.Clear();
		List<RewardData> drillRewards = Singleton<EconomyHelpers>.Instance.GetDrillRewards(PlayerData.Instance.DrillLevel.Value);
		foreach (RewardData item in drillRewards)
		{
			CollectOneReward(item);
		}
		PlayerData.Instance.DrillTimeStamp.Value = 0L;
		PlayerData.Instance.DrillLevel.Value = 0;
	}

	private void CollectOneReward(RewardData reward)
	{
		switch (reward.Type)
		{
		case RewardEnum.AddChestBronze:
			Singleton<FundRunner>.Instance.AddNormalChests(reward.Amount.ToInt(), "drill");
			Rewards.Add(new string[2]
			{
				BigString.ToString(reward.Amount),
				"UI/BundleItems/BundleItem_Chest_Bronze"
			});
			break;
		case RewardEnum.AddChestSilver:
			Singleton<FundRunner>.Instance.AddSilverChests(reward.Amount.ToInt(), "drill");
			Rewards.Add(new string[2]
			{
				BigString.ToString(reward.Amount),
				"UI/BundleItems/BundleItem_Chest_Silver"
			});
			break;
		case RewardEnum.AddChestGold:
			Singleton<FundRunner>.Instance.AddGoldChests(reward.Amount.ToInt(), "drill");
			Rewards.Add(new string[2]
			{
				BigString.ToString(reward.Amount),
				"UI/BundleItems/BundleItem_Chest_Gold"
			});
			break;
		case RewardEnum.AddToKeys:
			Singleton<FundRunner>.Instance.AddKeys(reward.Amount.ToInt(), "drill");
			Rewards.Add(new string[2]
			{
				BigString.ToString(reward.Amount),
				"UI/BundleItems/BundleItem_Keys"
			});
			break;
		case RewardEnum.AddToGems:
			Singleton<FundRunner>.Instance.AddGems(reward.Amount.ToInt(), "drill", "rewards");
			Rewards.Add(new string[2]
			{
				BigString.ToString(reward.Amount),
				"UI/BundleItems/BundleItem_Gems"
			});
			break;
		case RewardEnum.AddToCoins:
			GiveGoldReward();
			break;
		case RewardEnum.AddToResources:
			GiveResourceReward();
			break;
		}
	}

	private void GiveGoldReward()
	{
		int chunk = (PlayerData.Instance.MainChunk.Value <= PersistentSingleton<GameSettings>.Instance.DrillCoinMinimumConfig) ? PersistentSingleton<GameSettings>.Instance.DrillCoinMinimumConfig : PlayerData.Instance.MainChunk.Value;
		BiomeConfig biomeConfig = Singleton<EconomyHelpers>.Instance.GetBiomeConfig(chunk);
		BigDouble bigDouble = biomeConfig.BlockReward * PersistentSingleton<GameSettings>.Instance.DrillCoinMultiplier;
		Singleton<FundRunner>.Instance.AddCoins(bigDouble);
		Rewards.Add(new string[2]
		{
			BigString.ToString(bigDouble),
			"UI/BundleItems/BundleItem_Coins"
		});
	}

	private void GiveResourceReward()
	{
		int chunk = (PlayerData.Instance.LifetimeChunk.Value <= PersistentSingleton<GameSettings>.Instance.DrillResourceMinimumConfig) ? PersistentSingleton<GameSettings>.Instance.DrillResourceMinimumConfig : PlayerData.Instance.LifetimeChunk.Value;
		ChunkGeneratingConfig chunkGeneratingConfig = Singleton<EconomyHelpers>.Instance.GetChunkGeneratingConfig(chunk, bossFight: false);
		float num = (from w in chunkGeneratingConfig.Materials
			select w.Weight).Aggregate(0f, (float a, float b) => a + b);
		int[] array = new int[6];
		foreach (WeightedObject<BlockType> material in chunkGeneratingConfig.Materials)
		{
			array[(int)material.Value] = (int)(material.Weight / num * (float)chunkGeneratingConfig.MaxBlocks * PersistentSingleton<GameSettings>.Instance.DrillResourceMultiplier);
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] > 0)
			{
				Singleton<FundRunner>.Instance.AddResource((BlockType)i, array[i]);
				List<string[]> rewards = Rewards;
				string[] obj = new string[2]
				{
					array[i].ToString(),
					null
				};
				BlockType blockType = (BlockType)i;
				obj[1] = "UI/BundleItems/BundleItem_Resource_" + blockType.ToString();
				rewards.Add(obj);
			}
		}
	}
}
