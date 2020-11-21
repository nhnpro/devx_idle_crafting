using System.Collections.Generic;
using UnityEngine;

public static class RewardContentHelper
{
	public static GameObject CreateCard(RewardAction action)
	{
		switch (action.Reward.Type)
		{
		case RewardEnum.AddToGems:
			return CreateGemCard((GemRewardAction)action);
		case RewardEnum.MultiplyCoins:
			return CreateCoinCard((MultiplyCoinRewardAction)action);
		case RewardEnum.AddToKeys:
			return CreateKeyCard((KeyRewardAction)action);
		case RewardEnum.AddToRelics:
			return CreateRelicsCard((RelicRewardAction)action);
		case RewardEnum.AddToSkillAutoMine:
		case RewardEnum.AddToSkillGoldfinger:
		case RewardEnum.AddToSkillTapBoost:
		case RewardEnum.AddToSkillTeamBoost:
		case RewardEnum.AddToSkillTNT:
			return CreateSkillCard((SkillRewardAction)action);
		case RewardEnum.AddToBerries:
			return CreateBerryCard((BerryRewardAction)action);
		case RewardEnum.AddChestGold:
			return CreateGoldChestCard((GoldChestRewardAction)action);
		case RewardEnum.AddChestSilver:
			return CreateSilverChestCard((SilverChestRewardAction)action);
		case RewardEnum.AddChestBronze:
			return CreateBronzeChestCard((BronzeChestRewardAction)action);
		default:
			return null;
		}
	}

	private static GameObject CreateGemCard(GemRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		if (!string.IsNullOrEmpty(action.FriendId))
		{
			dictionary.Add("StringValue", action.FriendId + ".png");
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Gems", Vector3.zero, Quaternion.identity, dictionary);
		}
		switch (action.Rarity)
		{
		case RarityEnum.Common:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Gems_Common", Vector3.zero, Quaternion.identity, dictionary);
		case RarityEnum.Rare:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Gems_Rare", Vector3.zero, Quaternion.identity, dictionary);
		default:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Gems_Epic", Vector3.zero, Quaternion.identity, dictionary);
		}
	}

	private static GameObject CreateCoinCard(MultiplyCoinRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("BigDoubleValue", action.CoinAmount);
		if (!string.IsNullOrEmpty(action.FriendId))
		{
			dictionary.Add("StringValue", action.FriendId + ".png");
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Coins", Vector3.zero, Quaternion.identity, dictionary);
		}
		switch (action.Rarity)
		{
		case RarityEnum.Common:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Coins_Common", Vector3.zero, Quaternion.identity, dictionary);
		case RarityEnum.Rare:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Coins_Rare", Vector3.zero, Quaternion.identity, dictionary);
		default:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Coins_Epic", Vector3.zero, Quaternion.identity, dictionary);
		}
	}

	private static GameObject CreateRelicsCard(RelicRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("BigDoubleValue", action.Reward.Amount);
		if (!string.IsNullOrEmpty(action.FriendId))
		{
			dictionary.Add("StringValue", action.FriendId + ".png");
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Jelly", Vector3.zero, Quaternion.identity, dictionary);
		}
		switch (action.Rarity)
		{
		case RarityEnum.Common:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Relic_Common", Vector3.zero, Quaternion.identity, dictionary);
		case RarityEnum.Rare:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Relic_Rare", Vector3.zero, Quaternion.identity, dictionary);
		default:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Relic_Epic", Vector3.zero, Quaternion.identity, dictionary);
		}
	}

	private static GameObject CreateKeyCard(KeyRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		if (!string.IsNullOrEmpty(action.FriendId))
		{
			dictionary.Add("StringValue", action.FriendId + ".png");
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Keys", Vector3.zero, Quaternion.identity, dictionary);
		}
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_Key_Common", Vector3.zero, Quaternion.identity, dictionary);
	}

	private static GameObject CreateSkillCard(SkillRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		string str = string.Empty;
		switch (action.Rarity)
		{
		case RarityEnum.Rare:
			str = "_Rare";
			break;
		case RarityEnum.Epic:
			str = "_Epic";
			break;
		}
		switch (action.Skill)
		{
		case SkillsEnum.AutoMine:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_SkillItem_AutoMine" + str, Vector3.zero, Quaternion.identity, dictionary);
		case SkillsEnum.Goldfinger:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_SkillItem_Goldfinger" + str, Vector3.zero, Quaternion.identity, dictionary);
		case SkillsEnum.TNT:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_SkillItem_TNT" + str, Vector3.zero, Quaternion.identity, dictionary);
		case SkillsEnum.TapBoost:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_SkillItem_TapBoost" + str, Vector3.zero, Quaternion.identity, dictionary);
		case SkillsEnum.TeamBoost:
			return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ChestItems/ChestItem_SkillItem_TeamBoost" + str, Vector3.zero, Quaternion.identity, dictionary);
		default:
			return null;
		}
	}

	private static GameObject CreateBerryCard(BerryRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		dictionary.Add("StringValue", action.FriendId + ".png");
		dictionary.Add("HeroIndex", action.HeroIndex);
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Berry", Vector3.zero, Quaternion.identity, dictionary);
	}

	private static GameObject CreateGoldChestCard(GoldChestRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		dictionary.Add("StringValue", action.FriendId + ".png");
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Chest_Gold", Vector3.zero, Quaternion.identity, dictionary);
	}

	private static GameObject CreateSilverChestCard(SilverChestRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		dictionary.Add("StringValue", action.FriendId + ".png");
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Chest_Silver", Vector3.zero, Quaternion.identity, dictionary);
	}

	private static GameObject CreateBronzeChestCard(BronzeChestRewardAction action)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IntValue", action.Reward.Amount.ToInt());
		dictionary.Add("StringValue", action.FriendId + ".png");
		return Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GiftItems/Rewards/GiftReward_Chest_Bronze", Vector3.zero, Quaternion.identity, dictionary);
	}
}
