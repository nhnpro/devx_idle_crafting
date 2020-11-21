using System.Collections.Generic;

public class Economies : PersistentSingleton<Economies>
{
	public List<BiomeConfig> Biomes
	{
		get;
		private set;
	}

	public List<ChunkGeneratingConfig> BossChunkGeneratings
	{
		get;
		private set;
	}

	public List<ChunkGeneratingConfig> ChunkGeneratings
	{
		get;
		private set;
	}

	public List<ChunkMapConfig> ChunkMaps
	{
		get;
		private set;
	}

	public List<SkillConfig> Skills
	{
		get;
		private set;
	}

	public List<HeroConfig> Heroes
	{
		get;
		private set;
	}

	public List<BoosterConfig> Boosters
	{
		get;
		private set;
	}

	public List<int> PerkMilestones
	{
		get;
		private set;
	}

	public List<PerkMilestoneConfig> PerkMilestoneConfigs
	{
		get;
		private set;
	}

	public List<BonusMultConfig> CompanionMiniMilestones
	{
		get;
		private set;
	}

	public List<BonusMultConfig> HeroMiniMilestones
	{
		get;
		private set;
	}

	public List<TierConfig> Tiers
	{
		get;
		private set;
	}

	public List<RankConfig> Ranks
	{
		get;
		private set;
	}

	public List<GearConfig> Gears
	{
		get;
		private set;
	}

	public List<PlayerGoalConfig> TutorialGoals
	{
		get;
		private set;
	}

	public List<PlayerGoalConfig> PlayerGoals
	{
		get;
		private set;
	}

	public List<PlayerGoalRewards> PlayerGoalRewards
	{
		get;
		private set;
	}

	public List<GearSet> GearSets
	{
		get;
		private set;
	}

	public List<WeightedObject<ChestRewardConfig>> Chests
	{
		get;
		private set;
	}

	public List<WeightedObject<ChestRewardConfig>> KeyChest
	{
		get;
		private set;
	}

	public List<WeightedObject<ChestRewardConfig>> BronzeChest
	{
		get;
		private set;
	}

	public List<WeightedObject<ChestRewardConfig>> SilverChest
	{
		get;
		private set;
	}

	public List<WeightedObject<ChestRewardConfig>> GoldChest
	{
		get;
		private set;
	}

	public List<CraftingConfig> Crafting
	{
		get;
		private set;
	}

	public List<CraftingMaterialConfig> CraftingMaterial
	{
		get;
		private set;
	}

	public List<IAPConfig> IAPs
	{
		get;
		private set;
	}

	public List<BundleTierConfig> BundleTiers
	{
		get;
		private set;
	}

	public List<AdFrequencyConfig> AdFrequencies
	{
		get;
		private set;
	}

	public List<DrillConfig> Drills
	{
		get;
		private set;
	}

	public List<WeightedObject<RewardData>> Gifts
	{
		get;
		private set;
	}

	public List<GpuPerfData> GpuPerf
	{
		get;
		private set;
	}

	public List<XPromoConfig> XPromo
	{
		get;
		private set;
	}

	public List<GamblingConfig> Gamblings
	{
		get;
		private set;
	}

	public List<TournamentConfig> Tournaments
	{
		get;
		private set;
	}

	public List<TournamentTierConfig> TournamentTiers
	{
		get;
		private set;
	}

	public List<ZoneData> AdMobZoneData
	{
		get;
		set;
	}

	public List<ZoneData> UnityZoneData
	{
		get;
		set;
	}

	public List<ZoneAndFloor> FacebookFloors
	{
		get;
		private set;
	}

	public List<ZoneAndFloor> AdColonyFloors
	{
		get;
		private set;
	}

	public List<ZoneMapping> AdMobZoneMappings
	{
		get;
		private set;
	}

	public List<ZoneMapping> FacebookZoneMappings
	{
		get;
		private set;
	}

	public List<ZoneMapping> UnityZoneMappings
	{
		get;
		private set;
	}

	public void LoadConfigs()
	{
		StringCache instance = PersistentSingleton<StringCache>.Instance;
		Biomes = BiomeParser.ParseBiomes(instance.Get("biomes"));
		BossChunkGeneratings = ChunkGeneratingParser.ParseChunkGeneratings(instance.Get("boss_chunk_generating"));
		ChunkMaps = new List<ChunkMapConfig>();
		for (int i = 0; i <= 4; i++)
		{
			ChunkMaps.Add(ChunkMapParser.ParseChunkMap(instance.Get("ChunkMaps/chunk_map_" + i)));
		}
		ChunkGeneratings = ChunkGeneratingParser.ParseChunkGeneratings(instance.Get("chunk_generating"));
		Skills = SkillParser.ParseSkills(instance.Get("skills"));
		Heroes = HeroParser.ParseHeros(instance.Get("heros"));
		Boosters = BoosterParser.ParseBoosters(instance.Get("boosters"));
		string text = instance.Get("perk_milestones");
		PerkMilestones = PerkMilestoneParser.ParsePerkMilestonesLevels(text);
		PerkMilestoneConfigs = PerkMilestoneParser.ParsePerkMilestones(text);
		CompanionMiniMilestones = PerkMilestoneParser.ParseMiniMilestones(instance.Get("companion_mini_milestones"));
		HeroMiniMilestones = PerkMilestoneParser.ParseMiniMilestones(instance.Get("hero_mini_milestones"));
		Tiers = TierParser.ParseTiers(instance.Get("tiers"));
		Ranks = TierParser.ParseRanks(instance.Get("ranks"));
		Gears = GearParser.ParseGears(instance.Get("gears"));
		TutorialGoals = PlayerGoalParser.ParsePlayerGoals(instance.Get("tutorial_goals"), tutorial: true);
		PlayerGoals = PlayerGoalParser.ParsePlayerGoals(instance.Get("player_goals"), tutorial: false);
		PlayerGoalRewards = PlayerGoalParser.ParseRewards(instance.Get("goal_rewards"));
		GearSets = GearParser.ParseGearSets(instance.Get("gear_set"));
		KeyChest = ChestParser.ParseChests(instance.Get("chests"), ChestEnum.KeyChest);
		BronzeChest = ChestParser.ParseChests(instance.Get("chests"), ChestEnum.BronzeChest);
		SilverChest = ChestParser.ParseChests(instance.Get("chests"), ChestEnum.SilverChest);
		GoldChest = ChestParser.ParseChests(instance.Get("chests"), ChestEnum.GoldChest);
		Crafting = CraftingParser.ParseCrafting(instance.Get("crafting"));
		CraftingMaterial = CraftingMaterialParser.ParseCraftingMaterial(instance.Get("crafting_materials"));
		IAPs = IAPParser.ParseIAPs(instance.Get("iaps"));
		BundleTiers = BundleTierParser.ParseBundleTiers(instance.Get("bundle_tiers"));
		AdFrequencies = AdFrequencyParser.ParseAdFrequencies(instance.Get("Ads/frequencies"));
		Drills = DrillParser.ParseDrills(instance.Get("drills"));
		Gifts = GiftParser.ParseGiftRewards(instance.Get("gifts"));
		GpuPerf = GpuPerfParser.ParseGpuPerf(instance.Get("gpuperf"));
		XPromo = XPromoParser.ParseXpromo(instance.Get("xpromo"));
		Gamblings = GamblingParser.ParseGamblings(instance.Get("gambling"));
		Tournaments = TournamentParser.ParseTournaments(instance.Get("tournaments"));
		TournamentTiers = TournamentParser.ParseTournamentTiers(instance.Get("tournamentTiers"));
		AdMobZoneData = NetworkConfigParser.ParseAdMobZoneData();
		UnityZoneData = NetworkConfigParser.ParseUnityZoneData();
		FacebookFloors = NetworkConfigParser.ParseFacebookFloors();
		AdColonyFloors = NetworkConfigParser.ParseAdColonyFloors();
		AdMobZoneMappings = NetworkConfigParser.ParseAdMobZoneMappings();
		FacebookZoneMappings = NetworkConfigParser.ParseFacebookZoneMappings();
		UnityZoneMappings = NetworkConfigParser.ParseUnityZoneMappings();
	}
}
