using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRx;

public class PlayerDataLoader : BaseDataConverterImpl
{
	private static PlayerDataLoader m_instance;

	[CompilerGenerated]
	private static Func<JSONObject, GamblingState> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<JSONObject, SkillState> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<JSONObject, HeroState> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<JSONObject, GearState> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Func<JSONObject, PlayerGoalState> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Func<SkillState, JSONObject> _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static Func<HeroState, JSONObject> _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static Func<GearState, JSONObject> _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static Func<PlayerGoalState, JSONObject> _003C_003Ef__mg_0024cache8;

	private PlayerDataLoader()
	{
	}

	public static PlayerDataLoader Instance()
	{
		if (m_instance == null)
		{
			m_instance = new PlayerDataLoader();
		}
		return m_instance;
	}

	public override void FillBaseData(JSONObject json, BaseData baseData)
	{
		base.FillBaseData(json, baseData);
		PlayerData playerData = (PlayerData)baseData;
		playerData.Coins.Value = json.asBigDouble("Coins", () => 0L);
		playerData.WelcomebackCoins.Value = json.asBigDouble("WelcomebackCoins", () => 0L);
		playerData.LifetimeCoins.Value = json.asBigDouble("LifetimeCoins", () => 0L);
		playerData.LifetimeRelics.Value = json.asLong("LifetimeRelics", () => 0L);
		playerData.LifetimeBerries.Value = json.asLong("LifetimeBerries", () => 0L);
		playerData.LifetimePrestiges.Value = json.asInt("LifetimePrestiges", () => 0);
		playerData.RetryLevelNumber.Value = json.asInt("RetryLevelNumber", () => 0);
		playerData.Gems.Value = json.asInt("Gems", () => 100);
		playerData.LifetimeGems.Value = json.asInt("LifetimeGems", () => 100);
		playerData.Keys.Value = json.asInt("Keys", () => 0);
		playerData.LifetimeKeys.Value = json.asInt("LifetimeKeys", () => 0);
		playerData.Medals.Value = json.asInt("Medals", () => 0);
		playerData.LifetimeMedals.Value = json.asInt("LifetimeMedals", () => 0);
		playerData.Trophies.Value = json.asInt("Trophies", () => 0);
		playerData.LifetimeGamblings.Value = json.asInt("LifetimeGamblings", () => 0);
		playerData.BoostersBought = new List<ReactiveProperty<int>>();
		playerData.BoostersEffect = new List<ReactiveProperty<float>>();
		for (int i = 0; i < 7; i++)
		{
			ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>();
			reactiveProperty.Value = json.asInt("BoostersBought_" + (BoosterEnum)i, () => 0);
			playerData.BoostersBought.Add(reactiveProperty);
			ReactiveProperty<float> reactiveProperty2 = new ReactiveProperty<float>();
			if (BoosterCollectionRunner.IsBoosterMultiplier((BoosterEnum)i))
			{
				if (i == 3)
				{
					reactiveProperty2.Value = json.asFloat("BoostersEffect_" + (BoosterEnum)i, () => 2f);
				}
				else
				{
					reactiveProperty2.Value = json.asFloat("BoostersEffect_" + (BoosterEnum)i, () => 1f);
				}
			}
			else
			{
				reactiveProperty2.Value = json.asFloat("BoostersEffect_" + (BoosterEnum)i, () => 0f);
			}
			playerData.BoostersEffect.Add(reactiveProperty2);
		}
		playerData.HammerTimeElapsedTime.Value = json.asLong("HammerTimeElapsedTime", () => 85536000000000L);
		playerData.HammerTimeBonusDuration.Value = json.asInt("HammerTimeBonusDuration", () => 0);
		playerData.CurrentBundleEnum.Value = json.asInt("CurrentBundleEnum", () => int.MinValue);
		playerData.CurrentBundleGearIndex.Value = json.asInt("CurrentBundleGearIndex", () => 0);
		playerData.BundleTimeStamp.Value = json.asLong("BundleTimeStamp", () => 0L);
		playerData.LifetimeBundles.Value = json.asInt("LifetimeBundles", () => 0);
		playerData.LifetimeBlocksTaps.Value = json.asLong("LifetimeBlocksTaps", () => 0L);
		playerData.LifetimeCreatures.Value = json.asInt("LifetimeCreatures", () => 0);
		playerData.LifetimeGears.Value = json.asInt("LifetimeGears", () => 0);
		playerData.LifetimeGearLevels.Value = json.asInt("LifetimeGearLevels", () => 0);
		playerData.LifetimeBlocksDestroyed = new List<ReactiveProperty<long>>();
		playerData.BlocksInBackpack = new List<ReactiveProperty<long>>();
		playerData.BlocksCollected = new List<ReactiveProperty<long>>();
		for (int j = 0; j < 7; j++)
		{
			ReactiveProperty<long> reactiveProperty3 = new ReactiveProperty<long>();
			reactiveProperty3.Value = json.asLong("LifetimeBlocksDestroyed_" + (BlockType)j, () => 0L);
			playerData.LifetimeBlocksDestroyed.Add(reactiveProperty3);
			ReactiveProperty<long> reactiveProperty4 = new ReactiveProperty<long>();
			reactiveProperty4.Value = json.asLong("BlocksInBackpack_" + (BlockType)j, () => 0L);
			playerData.BlocksInBackpack.Add(reactiveProperty4);
			ReactiveProperty<long> reactiveProperty5 = new ReactiveProperty<long>();
			reactiveProperty5.Value = json.asLong("BlocksCollected_" + (BlockType)j, () => 0L);
			playerData.BlocksCollected.Add(reactiveProperty5);
		}
		playerData.GearChestsToCollect = new List<ReactiveProperty<int>>();
		for (int k = 0; k < 4; k++)
		{
			ReactiveProperty<int> reactiveProperty6 = new ReactiveProperty<int>();
			reactiveProperty6.Value = json.asInt("GearChestsToCollect_" + (ChestEnum)k, () => 0);
			playerData.GearChestsToCollect.Add(reactiveProperty6);
		}
		playerData.NormalChests.Value = json.asInt("NormalChests", () => 0);
		playerData.SilverChests.Value = json.asInt("SilverChests", () => 0);
		playerData.GoldChests.Value = json.asInt("GoldChests", () => 0);
		playerData.LifetimeAllOpenedChests.Value = json.asInt("LifetimeAllOpenedChests", () => 0);
		playerData.PlayerGoalsSeen = json.asCustom("PlayerGoalsSeen", (JSONObject jobj) => new ReactiveCollection<string>(DataHelper.JSONToStringList(jobj)), () => new ReactiveCollection<string>());
		playerData.TutorialStep.Value = json.asInt("TutorialStep", () => 0);
		playerData.ARTutorialStep.Value = json.asInt("ARTutorialStep", () => 0);
		playerData.MainChunk.Value = json.asInt("MainChunk", () => 0);
		playerData.BonusChunk.Value = json.asInt("BonusChunk", () => -1);
		playerData.LifetimeChunk.Value = json.asInt("LifetimeChunk", () => 0);
		playerData.BiomeStarted.Value = json.asBool("BiomeStarted", () => true);
		playerData.LevelSkipsBought = new List<ReactiveProperty<int>>();
		for (int l = 0; l < 3; l++)
		{
			ReactiveProperty<int> reactiveProperty7 = new ReactiveProperty<int>();
			reactiveProperty7.Value = json.asInt("LevelSkipsBought_" + (LevelSkipEnum)l, () => 0);
			playerData.LevelSkipsBought.Add(reactiveProperty7);
		}
		playerData.GamblingTimeStamp.Value = json.asLong("GamblingTimeStamp", () => 0L);
		playerData.Gambling.Value = json.asCustom("Gambling", DataHelper.JSONToGamlingState, () => new GamblingState());
		playerData.SkillStates = json.asCustom("SkillStates", (JSONObject jobj) => DataHelper.JSONToList(jobj, DataHelper.JSONToSkillState), () => new List<SkillState>());
		playerData.HeroStates = json.asCustom("HeroStates", (JSONObject jobj) => DataHelper.JSONToList(jobj, DataHelper.JSONToHeroState), () => new List<HeroState>());
		playerData.GearStates = json.asCustom("GearStates", (JSONObject jobj) => DataHelper.JSONToList(jobj, DataHelper.JSONToGearState), () => new List<GearState>());
		playerData.PlayerGoalStates = json.asCustom("PlayerGoalStates", (JSONObject jobj) => DataHelper.JSONToList(jobj, DataHelper.JSONToPlayerGoalState), () => new List<PlayerGoalState>());
		playerData.BossFailedLastTime.Value = json.asBool("BossFailedLastTime", () => false);
		playerData.WelcomebackTimeStamp.Value = json.asLong("WelcomebackTimeStamp", () => 0L);
		playerData.DrillTimeStamp.Value = json.asLong("DrillTimeStamp", () => 0L);
		playerData.DrillLevel.Value = json.asInt("DrillLevel", () => 0);
		playerData.DrJellyLevel.Value = json.asInt("DrJellyLevel", () => -1);
		playerData.DrJellySpawningLevel.Value = json.asInt("DrJellySpawningLevel", () => -1);
		playerData.TournamentTimeStamp.Value = json.asLong("TournamentTimeStamp", () => 0L);
		playerData.TournamentIdCurrent.Value = json.asInt("TournamentIdCurrent", () => -1);
		playerData.TournamentLastPointOnline.Value = json.asLong("TournamentLastPointOnline", () => 0L);
		playerData.TournamentRun.Value = DataHelper.ConvertStringToInt16Array(json.asString("TournamentRun", () => DataHelper.ConvertInt16ArrayToString(new short[90])));
		playerData.TournamentEnteringDevice.Value = json.asString("TournamentEnteringDevice", () => string.Empty);
		playerData.DisplayName.Value = json.asString("DisplayName", () => string.Empty);
		playerData.AREditorChosen.Value = json.asBool("AREditorChosen", () => false);
		playerData.AndroidEarlyAccess.Value = json.asBool("AndroidEarlyAccess", () => false);
	}

	public override void FillJson(JSONObject json, BaseData baseData)
	{
		base.FillJson(json, baseData);
		PlayerData playerData = (PlayerData)baseData;
		json.AddField("Coins", playerData.Coins.Value.ToString());
		json.AddField("WelcomebackCoins", playerData.WelcomebackCoins.Value.ToString());
		json.AddField("LifetimeCoins", playerData.LifetimeCoins.Value.ToString());
		json.AddField("LifetimeRelics", playerData.LifetimeRelics.Value);
		json.AddField("LifetimeBerries", playerData.LifetimeBerries.Value);
		json.AddField("LifetimePrestiges", playerData.LifetimePrestiges.Value);
		json.AddField("RetryLevelNumber", playerData.RetryLevelNumber.Value);
		json.AddField("Gems", playerData.Gems.Value);
		json.AddField("LifetimeGems", playerData.LifetimeGems.Value);
		json.AddField("Keys", playerData.Keys.Value);
		json.AddField("LifetimeKeys", playerData.LifetimeKeys.Value);
		json.AddField("Medals", playerData.Medals.Value);
		json.AddField("LifetimeMedals", playerData.LifetimeMedals.Value);
		json.AddField("Trophies", playerData.Trophies.Value);
		json.AddField("LifetimeGamblings", playerData.LifetimeGamblings.Value);
		for (int i = 0; i < 7; i++)
		{
			json.AddField("BoostersBought_" + (BoosterEnum)i, playerData.BoostersBought[i].Value);
			json.AddField("BoostersEffect_" + (BoosterEnum)i, playerData.BoostersEffect[i].Value);
		}
		json.AddField("HammerTimeElapsedTime", playerData.HammerTimeElapsedTime.Value);
		json.AddField("HammerTimeBonusDuration", playerData.HammerTimeBonusDuration.Value);
		json.AddField("CurrentBundleEnum", playerData.CurrentBundleEnum.Value);
		json.AddField("CurrentBundleGearIndex", playerData.CurrentBundleGearIndex.Value);
		json.AddField("BundleTimeStamp", playerData.BundleTimeStamp.Value);
		json.AddField("LifetimeBundles", playerData.LifetimeBundles.Value);
		json.AddField("LifetimeBlocksTaps", playerData.LifetimeBlocksTaps.Value);
		json.AddField("LifetimeCreatures", playerData.LifetimeCreatures.Value);
		json.AddField("LifetimeGears", playerData.LifetimeGears.Value);
		json.AddField("LifetimeGearLevels", playerData.LifetimeGearLevels.Value);
		for (int j = 0; j < 7; j++)
		{
			json.AddField("LifetimeBlocksDestroyed_" + (BlockType)j, playerData.LifetimeBlocksDestroyed[j].Value);
			json.AddField("BlocksInBackpack_" + (BlockType)j, playerData.BlocksInBackpack[j].Value);
			json.AddField("BlocksCollected_" + (BlockType)j, playerData.BlocksCollected[j].Value);
		}
		for (int k = 0; k < 4; k++)
		{
			json.AddField("GearChestsToCollect_" + (ChestEnum)k, playerData.GearChestsToCollect[k].Value);
		}
		json.AddField("NormalChests", playerData.NormalChests.Value);
		json.AddField("SilverChests", playerData.SilverChests.Value);
		json.AddField("GoldChests", playerData.GoldChests.Value);
		json.AddField("LifetimeAllOpenedChests", playerData.LifetimeAllOpenedChests.Value);
		json.AddField("PlayerGoalsSeen", DataHelper.CollectionToJSON(playerData.PlayerGoalsSeen));
		json.AddField("TutorialStep", playerData.TutorialStep.Value);
		json.AddField("ARTutorialStep", playerData.ARTutorialStep.Value);
		json.AddField("MainChunk", playerData.MainChunk.Value);
		json.AddField("BonusChunk", playerData.BonusChunk.Value);
		json.AddField("LifetimeChunk", playerData.LifetimeChunk.Value);
		json.AddField("BiomeStarted", playerData.BiomeStarted.Value);
		for (int l = 0; l < 3; l++)
		{
			json.AddField("LevelSkipsBought_" + (LevelSkipEnum)l, playerData.LevelSkipsBought[l].Value);
		}
		json.AddField("GamblingTimeStamp", playerData.GamblingTimeStamp.Value);
		json.AddField("Gambling", DataHelper.GamblingStateToJSON(playerData.Gambling.Value));
		json.AddField("SkillStates", DataHelper.ListToJSON(playerData.SkillStates, DataHelper.SkillStateToJSON));
		json.AddField("HeroStates", DataHelper.ListToJSON(playerData.HeroStates, DataHelper.HeroStateToJSON));
		json.AddField("GearStates", DataHelper.ListToJSON(playerData.GearStates, DataHelper.GearStateToJSON));
		json.AddField("PlayerGoalStates", DataHelper.ListToJSON(playerData.PlayerGoalStates, DataHelper.PlayerGoalStateToJSON));
		json.AddField("BossFailedLastTime", playerData.BossFailedLastTime.Value);
		json.AddField("WelcomebackTimeStamp", playerData.WelcomebackTimeStamp.Value);
		json.AddField("DrillTimeStamp", playerData.DrillTimeStamp.Value);
		json.AddField("DrillLevel", playerData.DrillLevel.Value);
		json.AddField("DrJellyLevel", playerData.DrJellyLevel.Value);
		json.AddField("DrJellySpawningLevel", playerData.DrJellySpawningLevel.Value);
		json.AddField("TournamentTimeStamp", playerData.TournamentTimeStamp.Value);
		json.AddField("TournamentIdCurrent", playerData.TournamentIdCurrent.Value);
		json.AddField("TournamentLastPointOnline", playerData.TournamentLastPointOnline.Value);
		json.AddField("TournamentRun", DataHelper.ConvertInt16ArrayToString(playerData.TournamentRun.Value));
		json.AddField("TournamentEnteringDevice", playerData.TournamentEnteringDevice.Value);
		json.AddField("DisplayName", playerData.DisplayName.Value);
		json.AddField("AREditorChosen", playerData.AREditorChosen.Value);
		json.AddField("AndroidEarlyAccess", playerData.AndroidEarlyAccess.Value);
	}
}
