using Big;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[PropertyClass]
public class IAPRunner : Singleton<IAPRunner>
{
	public ReactiveProperty<IAPStarted> IAPStarted = Observable.Never<IAPStarted>().ToReactiveProperty();

	public ReadOnlyReactiveProperty<IAPNotCompleted> IAPNotCompleted;

	public ReadOnlyReactiveProperty<IAPTransactionState> IAPCompleted;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> PurchaseInProgress;

	[PropertyBool]
	public ReactiveProperty<bool> PurchaseSuccess = Observable.Never<bool>().ToReactiveProperty();

	[PropertyBool]
	public ReactiveProperty<bool> PurchaseFail = Observable.Never<bool>().ToReactiveProperty();

	public List<string[]> PurchaseRewards = new List<string[]>();

	public IAPRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		PurchaseInProgress = PersistentSingleton<IAPService>.Instance.PurchaseInProgress.TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		IAPCompleted = (from completed in PersistentSingleton<IAPService>.Instance.IAPCompleted
			select (completed)).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		IAPCompleted.Subscribe(delegate
		{
			if (IAPCompleted.Value.Config.Reward.Type != 0)
			{
				RewardForPurchase(IAPCompleted.Value.Config);
				PurchaseSuccess.SetValueAndForceNotify(value: true);
			}
		}).AddTo(instance);
		IAPNotCompleted = (from notCompleted in PersistentSingleton<IAPService>.Instance.IAPNotCompleted
			where notCompleted.Status == "Failed" || notCompleted.Status == "Cancelled"
			select (notCompleted)).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		IAPNotCompleted.Do(delegate
		{
		}).Subscribe(delegate
		{
			PurchaseFail.SetValueAndForceNotify(value: true);
		}).AddTo(instance);
	}

	public void BuyIAP(IAPConfig config, string placement)
	{
		IAPStarted.Value = new IAPStarted(config, placement);
		PersistentSingleton<IAPService>.Instance.InitiatePurchase(config);
	}

	private void RewardForPurchase(IAPConfig cnfg)
	{
		PurchaseRewards.Clear();
		GiveOneReward(cnfg.Reward);
		RewardData[] extraRewards = cnfg.ExtraRewards;
		foreach (RewardData rewardData in extraRewards)
		{
			if (rewardData != null)
			{
				GiveOneReward(rewardData);
			}
		}
	}

	private void GiveOneReward(RewardData reward)
	{
		if (reward.Type == RewardEnum.AddToSkillRandomUnlocked)
		{
			reward = new RewardData(RandomizeUnlockedSkill(), reward.Amount);
		}
		switch (reward.Type)
		{
		case RewardEnum.AddBoosterDailyDoubleBoost:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.DailyDoubleBoost, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddBoosterDamageMultiplier:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.DamageMultiplier, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddBoosterGoldMultiplier:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.GoldMultiplier, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddBoosterHammerDuration:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.HammerDuration, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddBoosterKeyDropChance:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.KeyDropChance, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddBoosterShardMultiplier:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.ShardMultiplier, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddBoosterBerryDropChance:
			Singleton<BoosterCollectionRunner>.Instance.GiveBooster(BoosterEnum.BerryDropChance, reward.Amount.ToFloat());
			break;
		case RewardEnum.AddChestBronze:
			Singleton<FundRunner>.Instance.AddNormalChests(reward.Amount.ToInt(), reward.Type.ToString());
			break;
		case RewardEnum.AddChestGold:
			Singleton<FundRunner>.Instance.AddGoldChests(reward.Amount.ToInt(), reward.Type.ToString());
			break;
		case RewardEnum.AddChestSilver:
			Singleton<FundRunner>.Instance.AddSilverChests(reward.Amount.ToInt(), reward.Type.ToString());
			break;
		case RewardEnum.AddToCoins:
			Singleton<FundRunner>.Instance.AddCoins(reward.Amount);
			break;
		case RewardEnum.AddToCoinsSmall:
		case RewardEnum.AddToCoinsMedium:
		case RewardEnum.AddToCoinsLarge:
			reward = TransformCoinReward(reward);
			Singleton<FundRunner>.Instance.AddCoins(reward.Amount);
			break;
		case RewardEnum.AddToGems:
			Singleton<FundRunner>.Instance.AddGems(reward.Amount.ToInt(), reward.Type.ToString(), "iaps");
			break;
		case RewardEnum.AddToKeys:
			Singleton<FundRunner>.Instance.AddKeys(reward.Amount.ToInt(), reward.Type.ToString());
			break;
		case RewardEnum.AddToRelics:
			Singleton<FundRunner>.Instance.AddRelicsToFunds(reward.Amount);
			break;
		case RewardEnum.MultiplyCoins:
		{
			BigDouble amount = PlayerData.Instance.Coins.Value * (reward.Amount - 1L);
			Singleton<FundRunner>.Instance.AddCoins(amount);
			break;
		}
		case RewardEnum.AddToSkillAutoMine:
			Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.AutoMine).AddAmount(reward.Amount.ToInt());
			break;
		case RewardEnum.AddToSkillGoldfinger:
			Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.Goldfinger).AddAmount(reward.Amount.ToInt());
			break;
		case RewardEnum.AddToSkillTapBoost:
			Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TapBoost).AddAmount(reward.Amount.ToInt());
			break;
		case RewardEnum.AddToSkillTeamBoost:
			Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TeamBoost).AddAmount(reward.Amount.ToInt());
			break;
		case RewardEnum.AddToSkillTNT:
			Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TNT).AddAmount(reward.Amount.ToInt());
			break;
		case RewardEnum.AddToGearLevel:
			Singleton<UpgradeRunner>.Instance.UpgradeGearWithoutLimit(PlayerData.Instance.CurrentBundleGearIndex.Value, reward.Amount.ToInt());
			break;
		}
		PurchaseRewards.Add(GetRewardStringAndPrefabPath(reward, mainType: false));
	}

	private RewardData TransformCoinReward(RewardData reward)
	{
		int value = PlayerData.Instance.LifetimeChunk.Value;
		return new RewardData(reward.Type, Singleton<EconomyHelpers>.Instance.GetCoinRewardAmount(reward.Type, value));
	}

	private RewardEnum RandomizeUnlockedSkill()
	{
		List<RewardEnum> unlockedSkills = Singleton<EconomyHelpers>.Instance.GetUnlockedSkills();
		RewardEnum result = RewardEnum.Invalid;
		if (unlockedSkills.Count > 0)
		{
			result = unlockedSkills[Random.Range(0, unlockedSkills.Count)];
		}
		return result;
	}

	public string[] GetRewardStringAndPrefabPath(RewardData reward, bool mainType)
	{
		string str = string.Empty;
		if (mainType)
		{
			str = "Main_";
		}
		string str2 = string.Empty;
		string str3 = "+";
		str3 = ((reward.Type != RewardEnum.AddToGems) ? (str3 + BigString.ToString(reward.Amount)) : (str3 + reward.Amount.ToInt().ToString()));
		switch (reward.Type)
		{
		case RewardEnum.AddBoosterDailyDoubleBoost:
			str2 = "Booster_DailyDouble";
			str3 += "X";
			break;
		case RewardEnum.AddBoosterDamageMultiplier:
			str2 = "Booster_Damage";
			str3 += "X";
			break;
		case RewardEnum.AddBoosterGoldMultiplier:
			str2 = "Booster_Gold";
			str3 += "X";
			break;
		case RewardEnum.AddBoosterHammerDuration:
			str2 = "Booster_Hammer";
			str3 = BoosterCollectionRunner.FormatSecondsForBoosters(reward.Amount.ToInt());
			break;
		case RewardEnum.AddBoosterKeyDropChance:
			str2 = "Booster_KeyChance";
			str3 += "X";
			break;
		case RewardEnum.AddBoosterShardMultiplier:
			str2 = "Booster_ShardMultiplier";
			str3 += "X";
			break;
		case RewardEnum.AddBoosterBerryDropChance:
			str2 = "Booster_BerryChance";
			str3 += "X";
			break;
		case RewardEnum.AddChestBronze:
			str2 = "Chest_Bronze";
			break;
		case RewardEnum.AddChestGold:
			str2 = "Chest_Gold";
			break;
		case RewardEnum.AddChestSilver:
			str2 = "Chest_Silver";
			break;
		case RewardEnum.AddToCoins:
		case RewardEnum.AddToCoinsSmall:
		case RewardEnum.AddToCoinsMedium:
		case RewardEnum.AddToCoinsLarge:
			str2 = "Coins";
			break;
		case RewardEnum.MultiplyCoins:
			str2 = "Coins";
			str3 += "X";
			break;
		case RewardEnum.AddToGems:
			str2 = "Gems";
			break;
		case RewardEnum.AddToRelics:
			str2 = "Jelly";
			break;
		case RewardEnum.AddToSkillAutoMine:
			str2 = "Tornado";
			break;
		case RewardEnum.AddToSkillGoldfinger:
			str2 = "Goldfinger";
			break;
		case RewardEnum.AddToSkillTapBoost:
			str2 = "Dynamite";
			break;
		case RewardEnum.AddToSkillTeamBoost:
			str2 = "EnchantedCreatures";
			break;
		case RewardEnum.AddToSkillTNT:
			str2 = "TNT";
			break;
		case RewardEnum.AddToSkillRandomUnlocked:
			str2 = "Skill";
			break;
		case RewardEnum.AddToGearLevel:
			str2 = "Gear";
			break;
		}
		return new string[2]
		{
			str3,
			"UI/BundleItems/" + str + "BundleItem_" + str2
		};
	}

	public void RestorePurchases()
	{
		PersistentSingleton<IAPService>.Instance.RestorePurchases();
	}
}
