using System;
using System.Collections.Generic;

public class GameSettings : PersistentSingleton<GameSettings>
{
	public const int DATA_VERSION = 3;

	public const int SAVE_FILE_VERSION = 4;

	public const bool MANAGE_ANDROID_PERSISTENT_DATA_PATH = false;

	public const string MY_XPROMO_ID = "CA";

	public const string GooglePlayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjAYZVERoP4E4HwFAtzZt3JbsfNsiJ+MihJ9bSMjStq0zFo6gXUMc0Ubl4TCgw69UYMt7pR+2ghbTdnR0UvJ/CKhLYpqHhZMp9nQUmGZS+L9Yyl6gAn7Z4xN3kdoVh0fIg8nu0rn3/7hQoUO+7yjZ8usAWubnxRbg4cLL1r5UFwPR+lE7r2rFPdbP3YdFodUfxGdRQVZNFZtZHb4dAW7bZP6sK/UncD49hYkfonkER9cYwJkFpEkXMIl/mxN8I8Vrdro11PSTfhNO9PxqabjZZwPICxSO1pYYPpFehONESezhIf8XPvJ/qMn3PlF/J14IsGOAFOzN3ul3xf67euRMHQIDAQAB";

	public const string UnityAdsAppId = "1124326";

	public const string AdColonyZoneId = "vzc2600c4251094143b0";

	public const string AdColonyAppId = "appfe9ad9a3c19e49f38f";

	public const string AdMobAdsAppId = "ca-app-pub-7178055090700599~1990908832";

	public const string AdMobAdUnitId = "ca-app-pub-7178055090700599/6756860794";

	public const string VungleIOSAppId = "59477cc539f5f72906000426";

	public const string VungleAndroidAppId = "59477f9768ad644006000588";

	public const string AppsFlyerDevKey = "nr8SibwpFjcKGBQNpDdttd";

	public const string AppsFlyerAppId = "com.futureplay.minecraft";

	public const string VintraAppId = "TODO";

	public const string TimeServiceUrl = "https://fp-cloudsync.herokuapp.com/time";

	public const string AppLovinId = "6xdT87E5seTXRNXVn5VWNfTSlcl5MfsSbT0kbEpRtM-JkWqUbJ0--UQk5AqRqfvP8w8k9xA0I8K0f9VGTWER_e";

	public const string FBAudienceNetworkID = "1384750198279004_1440791502674873";

	public const string TournamentDummy = "D1E872B9C9DA0648";

	public bool FirebaseAnalyticsEnabled = true;

	public int DataVersion = 3;

	public string MinimumSupportedVersionIos = "0.1";

	public string MinimumSupportedVersionAndroid = "0.1";

	public int MinimumAcceptedVersion = 1;

	public string IosUpdateUrl = "itms-apps://itunes.apple.com/app/id1153498642";

	public string AndroidUpdateUrl = "market://details?id=com.futureplay.minecraft";

	public string FacebookPage = "https://www.facebook.com/craftawaygame/";

	public string FacebookAppId = "1384750198279004";

	public string FacebookPageId = "471159106569616";

	public string FacebookOGObjectId = "513057892380309";

	public string FAQPage = "https://futureplaygames.zendesk.com/hc/en-us/categories/360000030191-Craft-Away-";

	public string PrivacyPolicyPage = "http://www.futureplaygames.com/privacypolicy";

	public string TermsOfServicePage = "http://www.futureplaygames.com/termsofservice";

	public float BlockHpSoftest = 1f;

	public float BlockHpSoft = 3f;

	public float BlockHpMedium = 5f;

	public float BlockHpHard = 7f;

	public float BlockHpHardest = 15f;

	public float BlockRewardSoftest = 1f;

	public float BlockRewardSoft = 1f;

	public float BlockRewardMedium = 2f;

	public float BlockRewardHard = 2f;

	public float BlockRewardHardest = 50f;

	public int BossDurationSeconds = 30;

	public int MaxDamageBoosterPurchaseAmount = 300;

	public int MaxBoosterPurchaseAmount = 20;

	public int GoldenHammerInitialDuration = 300;

	public int GoldenHammerMultiplier = 2;

	public float[] GoldBoosterMultipliers = new float[3]
	{
		500f,
		1000f,
		5000f
	};

	public int[] GoldBoosterPrices = new int[3]
	{
		100,
		150,
		400
	};

	public int[] GoldBoosterMinRewards = new int[3]
	{
		15000,
		35000,
		150000
	};

	public int BundleActiveTime = 43200;

	public int BundleDeactiveTime = 86400;

	public int BundleTransitionTime = 21600;

	public float TntDamageMultiplier = 21000f;

	public float AutoMinePerSecond = 15f;

	public float AutoMineMultiplier = 20f;

	public float GoldFingerMultiplier = 2f;

	public float TapBoostDynamiteChance = 0.07f;

	public float TapBoostDynamiteDamageMultiplier = 100f;

	public float TeamBoostMultiplier = 5f;

	public int[] SkillPurchaseCosts = new int[5]
	{
		10,
		15,
		20,
		25,
		5
	};

	public int SkillPurchaseAmount = 1;

	public float CompanionMilestoneCostMultiplier = 6f;

	public int MiniMilestoneStep = 10;

	public float FreeLevelSkipPercent = 0.1f;

	public int FreeSkipMinimum = 30;

	public float ChunksMoneysAfterSkip = 6f;

	public float ResourceMultAfterSkip = 0.1f;

	public int SmallLevelSkipEffect = 30;

	public int MediumLevelSkipEffect = 60;

	public int LargeLevelSkipEffect = 100;

	public int FreeLevelSkipCost = 50;

	public int SmallLevelSkipCost = 50;

	public int MediumLevelSkipCost = 75;

	public int LargeLevelSkipCost = 100;

	public int LevelSkipAmount = 1;

	public int MediumSkipUnlockLevel = 200;

	public int LargeSkipUnlockLevel = 400;

	public int InitialGamblingFailCost = 25;

	public int MaximumGamblingFailCost = 1000;

	public float GamblingFailCostMultiplier = 2f;

	public float GamblingRewardVariation = 0.5f;

	public int GamblingCooldown = 43200;

	public int SavePointsPerTournament = 90;

	public int TournamentDurationSeconds = 86400;

	public int TournamentVariationSeconds = 300;

	public List<string> TournamentDays = new List<string>
	{
		"Wednesday",
		"Saturday"
	};

	public int TournamentBiggestLegitValue = 1200;

	public float ChestRelicMultiplier = 0.02f;

	public float HoldTapInterval = 0.25f;

	public float BossSwipeDamageMult = 0.2f;

	public int TierAdditionalBerryReq = 500;

	public int TierAdditionalBerryMult = 2;

	public int GearMilestoneCount = 5;

	public string TutorialStepReqForBerries = "Tutorial.UnlockSocial";

	public float BerryChanceFromSwipe = 0.00025f;

	public string TutorialStepReqForKeys = "Tutorial.UnlockSocial";

	public float KeyChanceFromSwipe = 0.0005f;

	public float KeyChanceFromAd = 0.5f;

	public int NormalChestKeyCost = 20;

	public int NormalChestGemCost = 80;

	public int SilverChestGemCost = 150;

	public int GoldChestGemCost = 200;

	public int NormalChestItemCount = 3;

	public int SilverChestItemCount = 6;

	public int GoldChestItemCount = 9;

	public float ChestRewardCoinBiomeMult = 1000f;

	public int GiftSendCooldown = 86400;

	public float WelcomeBackBlocksPerHero = 1f;

	public float WelcomeBackRewardMultiplier = 1f;

	public float DrillCoinMultiplier = 2000f;

	public int DrillCoinMinimumConfig = 20;

	public float DrillResourceMultiplier = 10f;

	public int DrillResourceMinimumConfig = 20;

	public int DrillDuration = 10800;

	public float CriticalTapMultBase = 50f;

	public float CriticalTapChanceBase = 0.005f;

	public int RelicsFromBossStartChunk = 64;

	public int RelicsFromBossChunkStep = 20;

	public int DisplayedLeaderboardEntries = 12;

	public int[] PrestigeRequirements = new int[5]
	{
		70,
		70,
		70,
		70,
		70
	};

	public int OverflowDamageMaxJumpDistance = 2;

	public int OverflowDamageTickAmount = 5;

	public long[] BlocksCollectStep = new long[7]
	{
		1L,
		1L,
		1L,
		1L,
		1L,
		1L,
		1L
	};

	public float[] BlocksToGems = new float[6]
	{
		0.01f,
		0.02f,
		0.04f,
		0.05f,
		71f / (339f * (float)Math.PI),
		0.2f
	};

	public int GemBlockReward = 1;

	public List<string> Segments = new List<string>();

	public int DrJellySpawnAfterMin = 10;

	public int DrJellySpawnAfterMax = 15;

	public float DrJellyRewardMult = 2f;

	public float DrJellyHpMult = 2f;

	public bool IntroSequenceOn = true;

	public bool MapJellyOn = true;

	public List<string> XPromotedApps = new List<string>
	{
		"FA",
		"SA",
		"BA",
		"FB"
	};

	public int XPromoDuration = 60;

	public string PlayFabTitleId => "A42";

	public string PlayFabUrl => "https://" + PlayFabTitleId + ".playfabapi.com/Client/";

	public string MinimumSupportedVersion => MinimumSupportedVersionAndroid;

	public static void JSONToSettings(JSONObject j, GameSettings settings)
	{
		GameSettings defaults = new GameSettings();
		settings.FirebaseAnalyticsEnabled = j.asBool("FirebaseAnalyticsEnabled", () => defaults.FirebaseAnalyticsEnabled);
		settings.DataVersion = j.asInt("DataVersion", () => 3);
		settings.MinimumSupportedVersionIos = j.asString("MinimumSupportedVersionIos", () => defaults.MinimumSupportedVersionIos);
		settings.MinimumSupportedVersionAndroid = j.asString("MinimumSupportedVersionAndroid", () => defaults.MinimumSupportedVersionAndroid);
		settings.MinimumAcceptedVersion = j.asInt("MinimumAcceptedVersion", () => defaults.MinimumAcceptedVersion);
		settings.IosUpdateUrl = j.asString("IosUpdateUrl", () => defaults.IosUpdateUrl);
		settings.AndroidUpdateUrl = j.asString("AndroidUpdateUrl", () => defaults.AndroidUpdateUrl);
		settings.FacebookPage = j.asString("FacebookPage", () => defaults.FacebookPage);
		settings.FacebookAppId = j.asString("FacebookAppId", () => defaults.FacebookAppId);
		settings.FacebookPageId = j.asString("FacebookPageId", () => defaults.FacebookPageId);
		settings.FAQPage = j.asString("FAQPage", () => defaults.FAQPage);
		settings.PrivacyPolicyPage = j.asString("PrivacyPolicyPage", () => defaults.PrivacyPolicyPage);
		settings.TermsOfServicePage = j.asString("TermsOfServicePage", () => defaults.TermsOfServicePage);
		settings.BlockHpSoftest = j.asFloat("BlockHpSoftest", () => defaults.BlockHpSoftest);
		settings.BlockHpSoft = j.asFloat("BlockHpSoft", () => defaults.BlockHpSoft);
		settings.BlockHpMedium = j.asFloat("BlockHpMedium", () => defaults.BlockHpMedium);
		settings.BlockHpHard = j.asFloat("BlockHpHard", () => defaults.BlockHpHard);
		settings.BlockHpHardest = j.asFloat("BlockHpHardest", () => defaults.BlockHpHardest);
		settings.BlockRewardSoftest = j.asFloat("BlockRewardSoftest", () => defaults.BlockRewardSoftest);
		settings.BlockRewardSoft = j.asFloat("BlockRewardSoft", () => defaults.BlockRewardSoft);
		settings.BlockRewardMedium = j.asFloat("BlockRewardMedium", () => defaults.BlockRewardMedium);
		settings.BlockRewardHard = j.asFloat("BlockRewardHard", () => defaults.BlockRewardHard);
		settings.BlockRewardHardest = j.asFloat("BlockRewardHardest", () => defaults.BlockRewardHardest);
		settings.BossDurationSeconds = j.asInt("BossDurationSeconds", () => defaults.BossDurationSeconds);
		settings.MaxDamageBoosterPurchaseAmount = j.asInt("MaxDamageBoosterPurchaseAmount", () => defaults.MaxDamageBoosterPurchaseAmount);
		settings.MaxBoosterPurchaseAmount = j.asInt("MaxBoosterPurchaseAmount", () => defaults.MaxBoosterPurchaseAmount);
		settings.GoldenHammerInitialDuration = j.asInt("GoldenHammerInitialDuration", () => defaults.GoldenHammerInitialDuration);
		settings.GoldenHammerMultiplier = j.asInt("GoldenHammerMultiplier", () => defaults.GoldenHammerMultiplier);
		int i;
		for (i = 0; i < 3; i++)
		{
			settings.GoldBoosterMultipliers[i] = j.asFloat("GoldBoosterMultipliers_" + (GoldBoosterEnum)i, () => defaults.GoldBoosterMultipliers[i]);
		}
		int k;
		for (k = 0; k < 3; k++)
		{
			settings.GoldBoosterPrices[k] = j.asInt("GoldBoosterPrices_" + (GoldBoosterEnum)k, () => defaults.GoldBoosterPrices[k]);
		}
		int l;
		for (l = 0; l < 3; l++)
		{
			settings.GoldBoosterMinRewards[l] = j.asInt("GoldBoosterMinRewards_" + (GoldBoosterEnum)l, () => defaults.GoldBoosterMinRewards[l]);
		}
		settings.BundleActiveTime = j.asInt("BundleActiveTime", () => defaults.BundleActiveTime);
		settings.BundleDeactiveTime = j.asInt("BundleDeactiveTime", () => defaults.BundleDeactiveTime);
		settings.BundleTransitionTime = j.asInt("BundleTransitionTime", () => defaults.BundleTransitionTime);
		settings.TntDamageMultiplier = j.asFloat("TntDamageMultiplier", () => defaults.TntDamageMultiplier);
		settings.AutoMinePerSecond = j.asFloat("AutoMinePerSecond", () => defaults.AutoMinePerSecond);
		settings.AutoMineMultiplier = j.asFloat("AutoMineMultiplier", () => defaults.AutoMineMultiplier);
		settings.GoldFingerMultiplier = j.asFloat("GoldFingerMultiplier", () => defaults.GoldFingerMultiplier);
		settings.TapBoostDynamiteChance = j.asFloat("TapBoostDynamiteChance", () => defaults.TapBoostDynamiteChance);
		settings.TapBoostDynamiteDamageMultiplier = j.asFloat("TapBoostDynamiteDamageMultiplier", () => defaults.TapBoostDynamiteDamageMultiplier);
		settings.TeamBoostMultiplier = j.asFloat("TeamBoostMultiplier", () => defaults.TeamBoostMultiplier);
		int m;
		for (m = 0; m < 5; m++)
		{
			settings.SkillPurchaseCosts[m] = j.asInt("SkillPurchaseCost_" + (SkillsEnum)m, () => defaults.SkillPurchaseCosts[m]);
		}
		settings.SkillPurchaseAmount = j.asInt("SkillPurchaseAmount", () => defaults.SkillPurchaseAmount);
		settings.CompanionMilestoneCostMultiplier = j.asFloat("CompanionMilestoneCostMultiplier", () => defaults.CompanionMilestoneCostMultiplier);
		settings.MiniMilestoneStep = j.asInt("MiniMilestoneStep", () => defaults.MiniMilestoneStep);
		settings.HoldTapInterval = j.asFloat("HoldTapInterval", () => defaults.HoldTapInterval);
		settings.ChestRelicMultiplier = j.asFloat("ChestRelicMultiplier", () => defaults.ChestRelicMultiplier);
		settings.BossSwipeDamageMult = j.asFloat("BossSwipeDamageMult", () => defaults.BossSwipeDamageMult);
		settings.FreeLevelSkipPercent = j.asFloat("FreeLevelSkipPercent", () => defaults.FreeLevelSkipPercent);
		settings.FreeSkipMinimum = j.asInt("FreeSkipMinimum", () => defaults.FreeSkipMinimum);
		settings.ChunksMoneysAfterSkip = j.asFloat("ChunksMoneysAfterSkip", () => defaults.ChunksMoneysAfterSkip);
		settings.ResourceMultAfterSkip = j.asFloat("ResourceMultAfterSkip", () => defaults.ResourceMultAfterSkip);
		settings.SmallLevelSkipEffect = j.asInt("SmallLevelSkipEffect", () => defaults.SmallLevelSkipEffect);
		settings.MediumLevelSkipEffect = j.asInt("MediumLevelSkipEffect", () => defaults.MediumLevelSkipEffect);
		settings.LargeLevelSkipEffect = j.asInt("LargeLevelSkipEffect", () => defaults.LargeLevelSkipEffect);
		settings.FreeLevelSkipCost = j.asInt("FreeLevelSkipCost", () => defaults.FreeLevelSkipCost);
		settings.SmallLevelSkipCost = j.asInt("SmallLevelSkipCost", () => defaults.SmallLevelSkipCost);
		settings.MediumLevelSkipCost = j.asInt("MediumLevelSkipCost", () => defaults.MediumLevelSkipCost);
		settings.LargeLevelSkipCost = j.asInt("LargeLevelSkipCost", () => defaults.LargeLevelSkipCost);
		settings.LevelSkipAmount = j.asInt("SmallLevelSkipEffect", () => defaults.LevelSkipAmount);
		settings.MediumSkipUnlockLevel = j.asInt("MediumSkipUnlockLevel", () => defaults.MediumSkipUnlockLevel);
		settings.LargeSkipUnlockLevel = j.asInt("LargeSkipUnlockLevel", () => defaults.LargeSkipUnlockLevel);
		settings.InitialGamblingFailCost = j.asInt("InitialGamblingFailCost", () => defaults.InitialGamblingFailCost);
		settings.MaximumGamblingFailCost = j.asInt("MaximumGamblingFailCost", () => defaults.MaximumGamblingFailCost);
		settings.GamblingFailCostMultiplier = j.asFloat("GamblingFailCostMultiplier", () => defaults.GamblingFailCostMultiplier);
		settings.GamblingRewardVariation = j.asFloat("GamblingRewardVariation", () => defaults.GamblingRewardVariation);
		settings.GamblingCooldown = j.asInt("GamblingCooldown", () => defaults.GamblingCooldown);
		settings.SavePointsPerTournament = j.asInt("SavePointsPerTournament", () => defaults.SavePointsPerTournament);
		settings.TournamentDurationSeconds = j.asInt("TournamentDurationSeconds", () => defaults.TournamentDurationSeconds);
		settings.TournamentVariationSeconds = j.asInt("TournamentVariationSeconds", () => defaults.TournamentVariationSeconds);
		settings.TournamentDays = j.asCustom("TournamentDays", (JSONObject list) => list.keys, () => defaults.TournamentDays);
		settings.TournamentBiggestLegitValue = j.asInt("TournamentBiggestLegitValue", () => defaults.TournamentBiggestLegitValue);
		settings.TierAdditionalBerryReq = j.asInt("TierAdditionalBottleReq", () => defaults.TierAdditionalBerryReq);
		settings.TierAdditionalBerryMult = j.asInt("TierAdditionalBottleMult", () => defaults.TierAdditionalBerryMult);
		settings.GearMilestoneCount = j.asInt("GearMilestoneCount", () => defaults.GearMilestoneCount);
		settings.TutorialStepReqForBerries = j.asString("TutorialStepReqForBerries", () => defaults.TutorialStepReqForBerries);
		settings.BerryChanceFromSwipe = j.asFloat("BerryChanceFromSwipe", () => defaults.BerryChanceFromSwipe);
		settings.TutorialStepReqForKeys = j.asString("TutorialStepReqForKeys", () => defaults.TutorialStepReqForKeys);
		settings.KeyChanceFromSwipe = j.asFloat("KeyChanceFromSwipe", () => defaults.KeyChanceFromSwipe);
		settings.KeyChanceFromAd = j.asFloat("KeyChanceFromAd", () => defaults.KeyChanceFromAd);
		settings.NormalChestKeyCost = j.asInt("NormalChestKeyCost", () => defaults.NormalChestKeyCost);
		settings.NormalChestGemCost = j.asInt("NormalChestGemCost", () => defaults.NormalChestGemCost);
		settings.SilverChestGemCost = j.asInt("SilverChestGemCost", () => defaults.SilverChestGemCost);
		settings.GoldChestGemCost = j.asInt("GoldChestGemCost", () => defaults.GoldChestGemCost);
		settings.NormalChestItemCount = j.asInt("NormalChestItemCount", () => defaults.NormalChestItemCount);
		settings.SilverChestItemCount = j.asInt("SilverChestItemCount", () => defaults.SilverChestItemCount);
		settings.GoldChestItemCount = j.asInt("GoldChestItemCount", () => defaults.GoldChestItemCount);
		settings.ChestRewardCoinBiomeMult = j.asFloat("ChestRewardCoinBiomeMult", () => defaults.ChestRewardCoinBiomeMult);
		settings.GiftSendCooldown = j.asInt("GiftSendCooldown", () => defaults.GiftSendCooldown);
		settings.WelcomeBackBlocksPerHero = j.asFloat("WelcomeBackBlocksPerHero", () => defaults.WelcomeBackBlocksPerHero);
		settings.WelcomeBackRewardMultiplier = j.asFloat("WelcomeBackRewardMultiplier", () => defaults.WelcomeBackRewardMultiplier);
		settings.DrillCoinMultiplier = j.asFloat("DrillCoinMultiplier", () => defaults.DrillCoinMultiplier);
		settings.DrillCoinMinimumConfig = j.asInt("DrillCoinMinimumConfig", () => defaults.DrillCoinMinimumConfig);
		settings.DrillResourceMultiplier = j.asFloat("DrillResourceMultiplier", () => defaults.DrillCoinMinimumConfig);
		settings.DrillResourceMinimumConfig = j.asInt("DrillResourceMinimumConfig", () => defaults.DrillResourceMinimumConfig);
		settings.DrillDuration = j.asInt("DrillDuration", () => defaults.DrillDuration);
		settings.CriticalTapMultBase = j.asFloat("CriticalTapMultBase", () => defaults.CriticalTapMultBase);
		settings.CriticalTapChanceBase = j.asFloat("CriticalTapChanceBase", () => defaults.CriticalTapChanceBase);
		settings.RelicsFromBossStartChunk = j.asInt("RelicsFromBossStartChunk", () => defaults.RelicsFromBossStartChunk);
		settings.RelicsFromBossChunkStep = j.asInt("RelicsFromBossChunkStep", () => defaults.RelicsFromBossChunkStep);
		settings.DisplayedLeaderboardEntries = j.asInt("DisplayedLeaderboardEntries", () => defaults.DisplayedLeaderboardEntries);
		int n;
		for (n = 0; n < defaults.PrestigeRequirements.Length; n++)
		{
			settings.PrestigeRequirements[n] = j.asInt("PrestigeRequirement_" + n, () => defaults.PrestigeRequirements[n]);
		}
		settings.OverflowDamageMaxJumpDistance = j.asInt("OverflowDamageMaxJumpDistance", () => defaults.OverflowDamageMaxJumpDistance);
		settings.OverflowDamageTickAmount = j.asInt("OverflowDamageTickAmount", () => defaults.OverflowDamageTickAmount);
		int i2;
		for (i2 = 0; i2 < 7; i2++)
		{
			settings.BlocksCollectStep[i2] = j.asLong("BlocksCollectStep_" + (BlockType)i2, () => defaults.BlocksCollectStep[i2]);
		}
		int i3;
		for (i3 = 0; i3 < 6; i3++)
		{
			settings.BlocksToGems[i3] = j.asFloat("BlocksToGems_" + (BlockType)i3, () => defaults.BlocksToGems[i3]);
		}
		settings.GemBlockReward = j.asInt("GemBlockReward", () => defaults.GemBlockReward);
		settings.Segments = j.asCustom("Segments", (JSONObject list) => list.keys, () => defaults.Segments);
		settings.DrJellySpawnAfterMin = j.asInt("DrJellySpawnAfterMin", () => defaults.DrJellySpawnAfterMin);
		settings.DrJellySpawnAfterMax = j.asInt("DrJellySpawnAfterMax", () => defaults.DrJellySpawnAfterMax);
		settings.DrJellyRewardMult = j.asFloat("DrJellyRewardMult", () => defaults.DrJellyRewardMult);
		settings.DrJellyHpMult = j.asFloat("DrJellyHpMult", () => defaults.DrJellyHpMult);
		settings.IntroSequenceOn = j.asBool("IntroSequenceOn", () => defaults.IntroSequenceOn);
		settings.MapJellyOn = j.asBool("MapJellyOn", () => defaults.MapJellyOn);
		settings.XPromotedApps = j.asCustom("XPromotedApps", (JSONObject list) => list.keys, () => defaults.XPromotedApps);
		settings.XPromoDuration = j.asInt("XPromoDuration", () => defaults.XPromoDuration);
	}

	public void DumpJson()
	{
	}
}
