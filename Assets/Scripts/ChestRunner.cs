using Big;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class ChestRunner : Singleton<ChestRunner>
{
	public List<RewardAction> RewardActions;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanOpenNormalChest;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanBuyNormalChestWithKeys;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanBuyNormalChestWithGems;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanOpenSilverChest;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanBuySilverChestWithGems;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanOpenGoldChest;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> CanBuyGoldChestWithGems;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> HaveFreeChestsToOpen;

	private ReactiveProperty<int> DoubleAdTime;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DoubleAdAvailable;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DoubleAdTimeLeft;

	[PropertyBool]
	public ReactiveProperty<bool> SequenceDone = new ReactiveProperty<bool>();

	public ChestRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		CanOpenNormalChest = (from chests in PlayerData.Instance.NormalChests
			select chests > 0).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanOpenSilverChest = (from chests in PlayerData.Instance.SilverChests
			select chests > 0).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanOpenGoldChest = (from chests in PlayerData.Instance.GoldChests
			select chests > 0).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanBuyNormalChestWithKeys = (from keys in PlayerData.Instance.Keys
			select keys >= PersistentSingleton<GameSettings>.Instance.NormalChestKeyCost).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanBuyNormalChestWithGems = (from gems in PlayerData.Instance.Gems
			select gems >= PersistentSingleton<GameSettings>.Instance.NormalChestGemCost).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanBuySilverChestWithGems = (from gems in PlayerData.Instance.Gems
			select gems >= PersistentSingleton<GameSettings>.Instance.SilverChestGemCost).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanBuyGoldChestWithGems = (from gems in PlayerData.Instance.Gems
			select gems >= PersistentSingleton<GameSettings>.Instance.GoldChestGemCost).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		HaveFreeChestsToOpen = CanOpenNormalChest.CombineLatest(CanOpenSilverChest, (bool normal, bool silver) => normal || silver).CombineLatest(CanOpenGoldChest, (bool canOpen, bool gold) => canOpen || gold).TakeUntilDestroy(instance)
			.ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		DoubleAdTime = (from _ in TickerService.MasterTicksSlow
			select (float)Singleton<AdRunner>.Instance.GetNextTimeToShowAd(AdPlacement.KeyChest).TotalSeconds into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		DoubleAdAvailable = (from time in DoubleAdTime
			select time <= 0).CombineLatest(Singleton<AdRunner>.Instance.AdReady, (bool time, bool ad) => time && ad).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DoubleAdTimeLeft = DoubleAdTime.Select(delegate(int seconds)
		{
			string empty = string.Empty;
			return (seconds > 0) ? TextUtils.FormatSecondsShort(seconds) : PersistentSingleton<LocalizationService>.Instance.Text("AD.Placement.NotAvailable");
		}).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFinished
			where ad == AdPlacement.KeyChest
			select ad).Subscribe(delegate
		{
			KeyChestAdFinished();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementSkipped
			where ad == AdPlacement.KeyChest
			select ad).Subscribe(delegate
		{
			KeyChestAdSkipped();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFailed
			where ad == AdPlacement.KeyChest
			select ad).Subscribe(delegate
		{
			KeyChestAdSkipped();
		}).AddTo(instance);
	}

	private void KeyChestAdFinished()
	{
		BuyNormalChestWithKeys(doubleReward: true);
		BindingManager.Instance.KeyChestAnimObj.SetActive(value: true);
	}

	private void KeyChestAdSkipped()
	{
		BindingManager.Instance.KeyChestOpeningParent.ShowInfo();
	}

	public void OpenNormalChest()
	{
		if (PlayerData.Instance.NormalChests.Value > 0)
		{
			Singleton<FundRunner>.Instance.RemoveNormalChests(1);
			GenerateAndOpenChest(PersistentSingleton<GameSettings>.Instance.NormalChestItemCount, ChestEnum.SilverChest);
		}
	}

	public void OpenSilverChest()
	{
		if (PlayerData.Instance.SilverChests.Value > 0)
		{
			Singleton<FundRunner>.Instance.RemoveSilverChests(1);
			GenerateAndOpenChest(PersistentSingleton<GameSettings>.Instance.SilverChestItemCount, ChestEnum.SilverChest);
		}
	}

	public void OpenGoldChest()
	{
		if (PlayerData.Instance.GoldChests.Value > 0)
		{
			Singleton<FundRunner>.Instance.RemoveGoldChests(1);
			GenerateAndOpenChest(PersistentSingleton<GameSettings>.Instance.GoldChestItemCount, ChestEnum.GoldChest);
		}
	}

	public void BuyNormalChestWithKeys(bool doubleReward)
	{
		if (PlayerData.Instance.Keys.Value >= PersistentSingleton<GameSettings>.Instance.NormalChestKeyCost)
		{
			Singleton<FundRunner>.Instance.RemoveKeys(PersistentSingleton<GameSettings>.Instance.NormalChestKeyCost);
			int num = PersistentSingleton<GameSettings>.Instance.NormalChestItemCount;
			if (doubleReward)
			{
				num *= 2;
			}
			GenerateAndOpenChest(num, ChestEnum.KeyChest);
		}
	}

	public void BuyNormalChestWithGems()
	{
		BuyChestWithGems(PersistentSingleton<GameSettings>.Instance.NormalChestGemCost, PersistentSingleton<GameSettings>.Instance.NormalChestItemCount, ChestEnum.BronzeChest);
	}

	public void BuySilverChestWithGems()
	{
		BuyChestWithGems(PersistentSingleton<GameSettings>.Instance.SilverChestGemCost, PersistentSingleton<GameSettings>.Instance.SilverChestItemCount, ChestEnum.SilverChest);
	}

	public void BuyGoldChestWithGems()
	{
		BuyChestWithGems(PersistentSingleton<GameSettings>.Instance.GoldChestGemCost, PersistentSingleton<GameSettings>.Instance.GoldChestItemCount, ChestEnum.GoldChest);
	}

	public void BuyChestWithGems(int gems, int rewardAmount, ChestEnum chest)
	{
		if (PlayerData.Instance.Gems.Value < gems)
		{
			NotEnoughGemsForChest(gems);
			BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
		}
		else
		{
			Singleton<FundRunner>.Instance.RemoveGems(gems, chest.ToString(), "chests");
			GenerateAndOpenChest(rewardAmount, chest);
		}
	}

	public void NotEnoughGemsForChest(int gems)
	{
		int missingGems = gems - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}

	private void GenerateAndOpenChest(int rewardAmount, ChestEnum chest)
	{
		Singleton<FundRunner>.Instance.OpenChests(1);
		Singleton<FakeFundRunner>.Instance.CopyFunds();
		List<ChestRewardConfig> rewards = CreateChestRewards(rewardAmount, chest);
		RewardActions = RewardFactory.ToRewardActions(rewards, PlayerData.Instance.LifetimeCreatures.Value);
		foreach (RewardAction rewardAction in RewardActions)
		{
			rewardAction.GiveReward();
		}
		ShowChestOpening();
	}

	private void ShowChestOpening()
	{
		SequenceDone.Value = false;
		BindingManager.Instance.ChestOpeningParent.SetActive(value: true);
	}

	public void CompleteChestSequence()
	{
		SequenceDone.Value = true;
	}

	public List<ChestRewardConfig> CreateChestRewards(int amount, ChestEnum chest)
	{
		return RandomRewards(amount, chest);
	}

	private List<ChestRewardConfig> RandomRewards(int amount, ChestEnum correctChest)
	{
		IEnumerable<WeightedObject<ChestRewardConfig>> enumerable = from chest in PersistentSingleton<Economies>.Instance.KeyChest
			where IsApplicable(chest.Value.Reward.Type)
			select chest;
		switch (correctChest)
		{
		case ChestEnum.KeyChest:
			enumerable = from chest in PersistentSingleton<Economies>.Instance.KeyChest
				where IsApplicable(chest.Value.Reward.Type)
				select chest;
			break;
		case ChestEnum.BronzeChest:
			enumerable = from chest in PersistentSingleton<Economies>.Instance.BronzeChest
				where IsApplicable(chest.Value.Reward.Type)
				select chest;
			break;
		case ChestEnum.SilverChest:
			enumerable = from chest in PersistentSingleton<Economies>.Instance.SilverChest
				where IsApplicable(chest.Value.Reward.Type)
				select chest;
			break;
		case ChestEnum.GoldChest:
			enumerable = from chest in PersistentSingleton<Economies>.Instance.GoldChest
				where IsApplicable(chest.Value.Reward.Type)
				select chest;
			break;
		}
		List<ChestRewardConfig> list = new List<ChestRewardConfig>();
		for (int i = 0; i < amount; i++)
		{
			ChestRewardConfig reward = enumerable.AllotObject();
			if (reward.Reward.Type == RewardEnum.AddToRelics)
			{
				BigDouble a = Singleton<EconomyHelpers>.Instance.GetRelicsFromBoss(PlayerData.Instance.LifetimeChunk.Value) * PersistentSingleton<GameSettings>.Instance.ChestRelicMultiplier;
				BigDouble right = BigDouble.Max(a, new BigDouble(1.0)).Round();
				reward = new ChestRewardConfig(new RewardData(reward.Reward.Type, reward.Reward.Amount * right), reward.Rarity);
			}
			if (reward.Reward.Type == RewardEnum.AddToSkillAutoMine || reward.Reward.Type == RewardEnum.AddToSkillGoldfinger || reward.Reward.Type == RewardEnum.AddToSkillTapBoost || reward.Reward.Type == RewardEnum.AddToSkillTeamBoost || reward.Reward.Type == RewardEnum.AddToSkillTNT)
			{
				enumerable = from x in enumerable
					where x.Value.Reward.Type != reward.Reward.Type
					select x;
			}
			list.Add(reward);
		}
		return list;
	}

	private bool IsApplicable(RewardEnum type)
	{
		SkillRunner orCreateSkillRunner = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TNT);
		switch (type)
		{
		case RewardEnum.AddToRelics:
		case RewardEnum.MultiplyCoins:
		case RewardEnum.AddToGems:
		case RewardEnum.AddToKeys:
			return true;
		case RewardEnum.Bottle:
			return PlayerData.Instance.LifetimeCreatures.Value > 1;
		case RewardEnum.AddToSkillAutoMine:
			orCreateSkillRunner = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.AutoMine);
			return orCreateSkillRunner.CooldownSeconds.Value > 15 && !orCreateSkillRunner.Active.Value;
		case RewardEnum.AddToSkillGoldfinger:
			orCreateSkillRunner = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.Goldfinger);
			return orCreateSkillRunner.CooldownSeconds.Value > 15 && !orCreateSkillRunner.Active.Value;
		case RewardEnum.AddToSkillTNT:
			orCreateSkillRunner = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TNT);
			return orCreateSkillRunner.CooldownSeconds.Value > 15 && !orCreateSkillRunner.Active.Value;
		case RewardEnum.AddToSkillTapBoost:
			orCreateSkillRunner = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TapBoost);
			return orCreateSkillRunner.CooldownSeconds.Value > 15 && !orCreateSkillRunner.Active.Value;
		case RewardEnum.AddToSkillTeamBoost:
			orCreateSkillRunner = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TeamBoost);
			return orCreateSkillRunner.CooldownSeconds.Value > 15 && !orCreateSkillRunner.Active.Value;
		default:
			return true;
		}
	}
}
