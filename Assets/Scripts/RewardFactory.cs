using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RewardFactory
{
	public static List<RewardAction> ToRewardActions(List<ChestRewardConfig> rewards, int numUnlockedHeros)
	{
		return (from cfg in rewards
			select ToRewardAction(cfg.Reward, cfg.Rarity, numUnlockedHeros)).ToList();
	}

	public static RewardAction ToRewardAction(RewardData reward, RarityEnum rarity, int numUnlockedHeros = -1, string friendId = null, int heroIndex = -1)
	{
		switch (reward.Type)
		{
		case RewardEnum.Bottle:
			return null;
		case RewardEnum.AddToGems:
			return new GemRewardAction(reward, rarity, friendId);
		case RewardEnum.AddToRelics:
			return new RelicRewardAction(reward, rarity, friendId);
		case RewardEnum.AddToKeys:
			return new KeyRewardAction(reward, rarity, friendId);
		case RewardEnum.AddToCoins:
			return new AddCoinRewardAction(reward, rarity, friendId);
		case RewardEnum.MultiplyCoins:
			return new MultiplyCoinRewardAction(reward, rarity, friendId);
		case RewardEnum.AddToSkillAutoMine:
			return new SkillRewardAction(reward, rarity, SkillsEnum.AutoMine, friendId);
		case RewardEnum.AddToSkillTeamBoost:
			return new SkillRewardAction(reward, rarity, SkillsEnum.TeamBoost, friendId);
		case RewardEnum.AddToSkillTapBoost:
			return new SkillRewardAction(reward, rarity, SkillsEnum.TapBoost, friendId);
		case RewardEnum.AddToSkillGoldfinger:
			return new SkillRewardAction(reward, rarity, SkillsEnum.Goldfinger, friendId);
		case RewardEnum.AddToSkillTNT:
			return new SkillRewardAction(reward, rarity, SkillsEnum.TNT, friendId);
		case RewardEnum.AddToSkillRandomUnlocked:
			return new SkillRewardAction(reward, rarity, (SkillsEnum)Random.Range(0, 5), friendId);
		case RewardEnum.AddToBerries:
			return new BerryRewardAction(reward, rarity, Random.Range(1, numUnlockedHeros), friendId);
		case RewardEnum.AddToSpecificBerries:
			return new BerryRewardAction(reward, rarity, heroIndex, friendId);
		case RewardEnum.AddChestGold:
			return new GoldChestRewardAction(reward, rarity, friendId);
		case RewardEnum.AddChestSilver:
			return new SilverChestRewardAction(reward, rarity, friendId);
		case RewardEnum.AddChestBronze:
			return new BronzeChestRewardAction(reward, rarity, friendId);
		case RewardEnum.AddToMedals:
			return new MedalRewardAction(reward, rarity, friendId);
		default:
			return null;
		}
	}
}
