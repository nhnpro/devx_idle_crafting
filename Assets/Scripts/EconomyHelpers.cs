using Big;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EconomyHelpers : Singleton<EconomyHelpers>
{
	public int GetNumHeroes()
	{
		return PersistentSingleton<Economies>.Instance.Heroes.Count;
	}

	public List<RewardEnum> GetUnlockedSkills()
	{
		List<RewardEnum> list = new List<RewardEnum>();
		int value = PlayerData.Instance.HeroStates[0].LifetimeLevel.Value;
		int i;
		for (i = 0; i < 5; i++)
		{
			if (PersistentSingleton<Economies>.Instance.Skills.Find(delegate(SkillConfig s)
			{
				string name = s.Name;
				SkillsEnum skillsEnum = (SkillsEnum)i;
				return name == skillsEnum.ToString();
			}).LevelReq <= value)
			{
				switch (i)
				{
				case 0:
					list.Add(RewardEnum.AddToSkillAutoMine);
					break;
				case 3:
					list.Add(RewardEnum.AddToSkillGoldfinger);
					break;
				case 4:
					list.Add(RewardEnum.AddToSkillTNT);
					break;
				case 2:
					list.Add(RewardEnum.AddToSkillTapBoost);
					break;
				case 1:
					list.Add(RewardEnum.AddToSkillTeamBoost);
					break;
				}
			}
		}
		return list;
	}

	public BigDouble GetUpgradeCost(int hero, int level)
	{
		HeroConfig heroConfig = PersistentSingleton<Economies>.Instance.Heroes[hero];
		BigDouble bigDouble = (!Singleton<EconomyHelpers>.Instance.IsMilestone(hero, level + 1)) ? (BigDouble.Pow(heroConfig.CostMultiplier, level - 1) * heroConfig.InitialCost) : (PersistentSingleton<GameSettings>.Instance.CompanionMilestoneCostMultiplier * BigDouble.Pow(heroConfig.CostMultiplier, level - 1) * heroConfig.InitialCost);
		return (hero != 0) ? bigDouble : (bigDouble * Singleton<CumulativeBonusRunner>.Instance.BonusMult[6].Value);
	}

	public BigDouble GetUpgradeCostRepeat(int hero, int level, int repeat)
	{
		HeroConfig heroConfig = PersistentSingleton<Economies>.Instance.Heroes[hero];
		BigDouble bigDouble = 0L;
		for (int i = 0; i < repeat; i++)
		{
			bigDouble += GetUpgradeCost(hero, level + i);
		}
		return bigDouble;
	}

	private void GetUpgradeMax(int hero, int level, BigDouble max, out BigDouble cost, out int lvls)
	{
		HeroConfig heroConfig = PersistentSingleton<Economies>.Instance.Heroes[hero];
		lvls = 1;
		cost = GetUpgradeCost(hero, level);
		GetPerkMilestoneLevel(level, out int _, out int next);
		if (hero == 0)
		{
			next = GetNextSkillUnlockLevel();
		}
		while (true)
		{
			BigDouble upgradeCost = GetUpgradeCost(hero, level + lvls);
			if (cost + upgradeCost > max || level + lvls == next)
			{
				break;
			}
			cost += upgradeCost;
			lvls++;
		}
	}

	public BigDouble GetUpgradeMaxCost(int hero, int level, BigDouble max)
	{
		GetUpgradeMax(hero, level, max, out BigDouble cost, out int _);
		return cost;
	}

	public int GetUpgradeMaxLevels(int hero, int level, BigDouble max)
	{
		GetUpgradeMax(hero, level, max, out BigDouble _, out int lvls);
		return lvls;
	}

	public int GetUnlockedPerkCount(int heroLevel)
	{
		for (int i = 0; i < PersistentSingleton<Economies>.Instance.PerkMilestones.Count; i++)
		{
			if (heroLevel < PersistentSingleton<Economies>.Instance.PerkMilestones[i])
			{
				return i;
			}
		}
		return PersistentSingleton<Economies>.Instance.PerkMilestones.Count;
	}

	public BigDouble GetCoinRewardAmount(RewardEnum type, int chunk)
	{
		BigDouble bigDouble = Singleton<EconomyHelpers>.Instance.GetBiomeConfig(chunk).BlockReward;
		switch (type)
		{
		case RewardEnum.AddToCoinsSmall:
			bigDouble *= PersistentSingleton<GameSettings>.Instance.GoldBoosterMultipliers[0];
			bigDouble = BigDouble.Max(bigDouble, PersistentSingleton<GameSettings>.Instance.GoldBoosterMinRewards[0]);
			break;
		case RewardEnum.AddToCoinsMedium:
			bigDouble *= PersistentSingleton<GameSettings>.Instance.GoldBoosterMultipliers[1];
			bigDouble = BigDouble.Max(bigDouble, PersistentSingleton<GameSettings>.Instance.GoldBoosterMinRewards[1]);
			break;
		case RewardEnum.AddToCoinsLarge:
			bigDouble *= PersistentSingleton<GameSettings>.Instance.GoldBoosterMultipliers[2];
			bigDouble = BigDouble.Max(bigDouble, PersistentSingleton<GameSettings>.Instance.GoldBoosterMinRewards[2]);
			break;
		}
		return bigDouble;
	}

	public List<RewardData> GetDrillRewards(int drillLevel)
	{
		List<RewardData> list = new List<RewardData>();
		foreach (DrillConfig drill in PersistentSingleton<Economies>.Instance.Drills)
		{
			if (drill.Level <= drillLevel)
			{
				list.Add(drill.Reward);
			}
		}
		return list;
	}

	public IAPConfig GetSmallestGemPurchaseNeeded(int gemsMissing)
	{
		IAPConfig result = PersistentSingleton<Economies>.Instance.IAPs[0];
		for (int i = 0; i < PersistentSingleton<Economies>.Instance.IAPs.Count; i++)
		{
			if (PersistentSingleton<Economies>.Instance.IAPs[i].Reward.Type == RewardEnum.AddToGems && PersistentSingleton<Economies>.Instance.IAPs[i].Type == IAPType.Consumable)
			{
				result = PersistentSingleton<Economies>.Instance.IAPs[i];
				if (gemsMissing < PersistentSingleton<Economies>.Instance.IAPs[i].Reward.Amount)
				{
					return PersistentSingleton<Economies>.Instance.IAPs[i];
				}
			}
		}
		return result;
	}

	public float GetMilestoneProgress(int heroLevel)
	{
		GetMiniMilestoneLevel(heroLevel, out int current, out int next);
		return (float)(heroLevel - current + 1) / (float)(next - current);
	}

	public void GetPerkMilestoneLevel(int heroLevel, out int current, out int next)
	{
		int num = GetUnlockedPerkCount(heroLevel) - 1;
		current = ((num >= 0) ? PersistentSingleton<Economies>.Instance.PerkMilestones[num] : 0);
		next = ((num >= PersistentSingleton<Economies>.Instance.PerkMilestones.Count - 1) ? int.MaxValue : PersistentSingleton<Economies>.Instance.PerkMilestones[num + 1]);
	}

	public int GetNextSkillUnlockLevel()
	{
		int result = int.MaxValue;
		if (PlayerData.Instance.HeroStates[0].LifetimeLevel.Value < PersistentSingleton<Economies>.Instance.Skills[PersistentSingleton<Economies>.Instance.Skills.Count - 1].LevelReq)
		{
			SkillConfig skillConfig = PersistentSingleton<Economies>.Instance.Skills.First((SkillConfig bd) => bd.LevelReq > PlayerData.Instance.HeroStates[0].LifetimeLevel.Value);
			result = skillConfig.LevelReq;
		}
		return result;
	}

	public void GetMiniMilestoneLevel(int heroLevel, out int current, out int next)
	{
		current = heroLevel / PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep * PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep;
		next = current + PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep;
	}

	public int GetNumBoosters()
	{
		return PersistentSingleton<Economies>.Instance.Boosters.Count;
	}

	public int GetBoosterCost(int boosterIndex, int boostersBought)
	{
		BoosterConfig boosterConfig = PersistentSingleton<Economies>.Instance.Boosters[boosterIndex];
		return boosterConfig.InitialGemCost + boosterConfig.GemCostIncrease * boostersBought;
	}

	public BiomeConfig GetBiomeConfig(int chunk)
	{
		return PersistentSingleton<Economies>.Instance.Biomes.Last((BiomeConfig bd) => bd.Chunk <= chunk);
	}

	public GamblingConfig GetGamblingConfig(int level)
	{
		return PersistentSingleton<Economies>.Instance.Gamblings.Last((GamblingConfig gc) => gc.Level <= level);
	}

	public int GetNextGamblingJackpotLevel(int level)
	{
		int maxGamblingLevel = GetMaxGamblingLevel();
		if (level < maxGamblingLevel)
		{
			return PersistentSingleton<Economies>.Instance.Gamblings.First((GamblingConfig gc) => gc.Level >= level && gc.FailChance == 0f).Level;
		}
		return maxGamblingLevel;
	}

	public int GetMaxGamblingLevel()
	{
		return PersistentSingleton<Economies>.Instance.Gamblings[PersistentSingleton<Economies>.Instance.Gamblings.Count - 1].Level;
	}

	public TournamentConfig GetTournamentPriceBracket(int rank)
	{
		return PersistentSingleton<Economies>.Instance.Tournaments.First((TournamentConfig tc) => tc.Rank >= rank);
	}

	public TournamentConfig GetNextTournamentPriceBracket(int rank)
	{
		return PersistentSingleton<Economies>.Instance.Tournaments.Last((TournamentConfig tc) => tc.Rank < rank);
	}

	public TournamentTierConfig GetTournamentTierConfig(int tier)
	{
		if (!Enum.IsDefined(typeof(TournamentTier), tier))
		{
			return null;
		}
		return PersistentSingleton<Economies>.Instance.TournamentTiers.First((TournamentTierConfig ttc) => ttc.Tier == (TournamentTier)tier);
	}

	public ChunkGeneratingConfig GetChunkGeneratingConfig(int chunk, bool bossFight)
	{
		if (bossFight)
		{
			return PersistentSingleton<Economies>.Instance.BossChunkGeneratings.Last((ChunkGeneratingConfig bd) => bd.Chunk <= chunk);
		}
		return PersistentSingleton<Economies>.Instance.ChunkGeneratings.Last((ChunkGeneratingConfig bd) => bd.Chunk <= chunk);
	}

	public int GetTierBerryDeltaReq(int tier)
	{
		if (tier < 0)
		{
			return 1;
		}
		int num = 0;
		GetTierBerryFullReqMult(tier, out int berryReq, out float _);
		return berryReq - num;
	}

	public float GetTierDamageMult(int tier)
	{
		float damageMult = 0f;
		GetTierBerryFullReqMult(tier, out int _, out damageMult);
		return damageMult;
	}

	private void GetTierBerryFullReqMult(int tier, out int berryReq, out float damageMult)
	{
		tier = Math.Max(tier, 1);
		int num = Math.Min(tier, PersistentSingleton<Economies>.Instance.Tiers.Count);
		int num2 = tier - num;
		berryReq = PersistentSingleton<Economies>.Instance.Tiers[num - 1].BerryReq + num2 * PersistentSingleton<GameSettings>.Instance.TierAdditionalBerryReq;
		damageMult = PersistentSingleton<Economies>.Instance.Tiers[num - 1].DamageMult + (float)(num2 * PersistentSingleton<GameSettings>.Instance.TierAdditionalBerryMult);
	}

	public float GetBerryProgress(int tier, int berries)
	{
		return (float)berries / (float)GetTierBerryDeltaReq(tier);
	}

	public string GetNextMilestoneText(int heroIndex, int heroLevel)
	{
		int num = (heroLevel + PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep) / PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep * PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep;
		BonusMultConfig milestoneOrNull = GetMilestoneOrNull(heroIndex, num);
		return milestoneOrNull.Amount + " " + milestoneOrNull.BonusType + " on level " + num;
	}

	public string GetBerryRequirementText(int tier, int berries)
	{
		return berries + "/" + GetTierBerryDeltaReq(tier);
	}

	public bool IsMilestone(int heroIndex, int heroLevel)
	{
		return heroLevel % PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep == 0;
	}

	public bool IsMiniMilestone(int heroIndex, int heroLevel)
	{
		if (heroLevel % PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep != 0)
		{
			return false;
		}
		return !IsPerkMilestone(heroIndex, heroLevel);
	}

	public bool IsPerkMilestone(int heroIndex, int heroLevel)
	{
		int num = PersistentSingleton<Economies>.Instance.PerkMilestones.FindIndex((int lvl) => lvl == heroLevel);
		if (num == -1)
		{
			return false;
		}
		return PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[heroIndex].Items[num].BonusType != BonusTypeEnum.None;
	}

	public BonusMultConfig GetMilestoneOrNull(int heroIndex, int heroLevel)
	{
		int num = PersistentSingleton<Economies>.Instance.PerkMilestones.FindIndex((int lvl) => lvl == heroLevel);
		if (num != -1 && PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[heroIndex].Items[num].BonusType != BonusTypeEnum.None)
		{
			return PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[heroIndex].Items[num];
		}
		return GetMiniMilestoneOrNull(heroIndex, heroLevel, heroLevel);
	}

	public BonusMultConfig GetMiniMilestoneOrNull(int heroIndex, int startLevel, int endLevel)
	{
		if (startLevel <= 0 && endLevel <= 0)
		{
			return null;
		}
		if (endLevel % PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep != 0 && endLevel - startLevel < PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep - startLevel % PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep)
		{
			return null;
		}
		int num = (startLevel + (PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep - startLevel % PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep)) / PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep - 1;
		int num2 = Mathf.FloorToInt((endLevel - startLevel - 1 - startLevel % PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep) / PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep + 1);
		if (num2 == 1)
		{
			if (IsPerkMilestone(heroIndex, (num + 1) * PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep))
			{
				return null;
			}
			if (heroIndex == 0)
			{
				num %= PersistentSingleton<Economies>.Instance.HeroMiniMilestones.Count;
				return PersistentSingleton<Economies>.Instance.HeroMiniMilestones[num];
			}
			num %= PersistentSingleton<Economies>.Instance.CompanionMiniMilestones.Count;
			return PersistentSingleton<Economies>.Instance.CompanionMiniMilestones[num];
		}
		List<BonusMultConfig> list = new List<BonusMultConfig>();
		for (int i = 0; i < num2; i++)
		{
			if (!IsPerkMilestone(heroIndex, (num + i + 1) * PersistentSingleton<GameSettings>.Instance.MiniMilestoneStep))
			{
				if (heroIndex == 0)
				{
					list.Add(PersistentSingleton<Economies>.Instance.HeroMiniMilestones[(num + i) % PersistentSingleton<Economies>.Instance.HeroMiniMilestones.Count]);
				}
				else
				{
					list.Add(PersistentSingleton<Economies>.Instance.CompanionMiniMilestones[(num + i) % PersistentSingleton<Economies>.Instance.CompanionMiniMilestones.Count]);
				}
			}
		}
		if (list.Count < 1)
		{
			return null;
		}
		float num3 = 1f;
		for (int j = 0; j < list.Count; j++)
		{
			num3 *= list[j].Amount;
		}
		BonusMultConfig bonusMultConfig = new BonusMultConfig();
		bonusMultConfig.BonusType = list[0].BonusType;
		bonusMultConfig.Amount = num3;
		return bonusMultConfig;
	}

	public int GetTutorialGoalAmount()
	{
		return PersistentSingleton<Economies>.Instance.TutorialGoals.Count;
	}

	public int GetNewCreatureInChunkOrNone(int chunkIndex)
	{
		for (int i = 0; i < GetNumHeroes(); i++)
		{
			if (chunkIndex == PersistentSingleton<Economies>.Instance.Heroes[i].UnlockAtChunk - 1)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetNewCreatureInRangeOrNone(int currentIndex, int startIndex, int endIndex)
	{
		for (int i = 0; i < GetNumHeroes(); i++)
		{
			if (endIndex >= currentIndex && startIndex <= PersistentSingleton<Economies>.Instance.Heroes[i].UnlockAtChunk - 1 && endIndex >= PersistentSingleton<Economies>.Instance.Heroes[i].UnlockAtChunk - 1)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetNewGearSetInRangeOrNone(int currentChunk, int startIndex, int endIndex)
	{
		for (int i = 0; i < PersistentSingleton<Economies>.Instance.GearSets.Count; i++)
		{
			if (endIndex >= currentChunk && startIndex <= PersistentSingleton<Economies>.Instance.GearSets[i].ChunkUnlockLevel - 1 && endIndex >= PersistentSingleton<Economies>.Instance.GearSets[i].ChunkUnlockLevel - 1)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetNumUnlockedHeroes(int chunkIndex)
	{
		for (int i = 0; i < GetNumHeroes(); i++)
		{
			if (chunkIndex < PersistentSingleton<Economies>.Instance.Heroes[i].UnlockAtChunk)
			{
				return i;
			}
		}
		return GetNumHeroes();
	}

	public RankEnum GetRank(int tier)
	{
		int index = Mathf.Clamp(tier, 0, PersistentSingleton<Economies>.Instance.Ranks.Count - 1);
		return PersistentSingleton<Economies>.Instance.Ranks[index].Rank;
	}

	public int GetBlockToGems(BlockType type, BigDouble amount)
	{
		return Mathf.CeilToInt(PersistentSingleton<GameSettings>.Instance.BlocksToGems[(int)type] * amount.ToFloat());
	}

	public int GetCraftingGemCost(CraftingRequirement have, CraftingRequirement req)
	{
		int num = 0;
		BigDouble relics = BigDouble.Max(0L, req.Resources[6] - have.Resources[6]);
		num += MissingRelicsToGems.Evaluate(relics);
		for (int i = 0; i < 6; i++)
		{
			long value = Math.Max(0L, req.Resources[i] - have.Resources[i]);
			num += GetBlockToGems((BlockType)i, value);
		}
		return num;
	}

	public BigDouble GetRelicsFromBoss(int relativeChunk)
	{
		int relicsFromBossStartChunk = PersistentSingleton<GameSettings>.Instance.RelicsFromBossStartChunk;
		int relicsFromBossChunkStep = PersistentSingleton<GameSettings>.Instance.RelicsFromBossChunkStep;
		if (relativeChunk < relicsFromBossStartChunk)
		{
			return BigDouble.ZERO;
		}
		int num = (relativeChunk - relicsFromBossStartChunk) / relicsFromBossChunkStep;
		return new BigDouble(num + 1) * new BigDouble(num + 2) / 2.0;
	}

	public CraftingRequirement GetGearUpgradeCost(int id, int level)
	{
		CraftingRequirement craftingRequirement = new CraftingRequirement();
		CraftingMaterialConfig craftingMaterialConfig = PersistentSingleton<Economies>.Instance.CraftingMaterial[id];
		craftingRequirement.Resources = new long[7]
		{
			0L,
			0L,
			0L,
			0L,
			0L,
			0L,
			GetRelicCost(id, level)
		};
		if (craftingMaterialConfig.JellyStartingpoint >= 0 && level == 0)
		{
			craftingRequirement.Resources[6] = PersistentSingleton<Economies>.Instance.Crafting[craftingMaterialConfig.JellyStartingpoint].Relics;
		}
		if (craftingMaterialConfig.Material1 != null)
		{
			craftingRequirement.Resources[(int)craftingMaterialConfig.Material1.Type] = PersistentSingleton<Economies>.Instance.Crafting[level + craftingMaterialConfig.Material1.StartingPoint].Materials[(int)craftingMaterialConfig.Material1.Type];
		}
		if (craftingMaterialConfig.Material2 != null)
		{
			craftingRequirement.Resources[(int)craftingMaterialConfig.Material2.Type] = PersistentSingleton<Economies>.Instance.Crafting[level + craftingMaterialConfig.Material2.StartingPoint].Materials[(int)craftingMaterialConfig.Material2.Type];
		}
		if (craftingMaterialConfig.Material3 != null)
		{
			craftingRequirement.Resources[(int)craftingMaterialConfig.Material3.Type] = PersistentSingleton<Economies>.Instance.Crafting[level + craftingMaterialConfig.Material3.StartingPoint].Materials[(int)craftingMaterialConfig.Material3.Type];
		}
		return craftingRequirement;
	}

	private long GetRelicCost(int id, int level)
	{
		Economies instance = PersistentSingleton<Economies>.Instance;
		if (level <= 0)
		{
			return -1L;
		}
		double a = (double)instance.Gears[id].UpgradeCostA * Math.Pow(instance.Gears[id].UpgradeCostB, level) + (double)instance.Gears[id].UpgradeCostC;
		return (long)Math.Round(a);
	}
}
