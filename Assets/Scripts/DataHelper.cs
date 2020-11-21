using Big;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public static class DataHelper
{
	public static JSONObject CollectionToJSON(ReactiveCollection<string> listString)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		foreach (string item in listString)
		{
			jSONObject.Add(JSONObject.CreateStringObject(item));
		}
		return jSONObject;
	}

	public static JSONObject FBPlayersToJSON(FacebookAPIService service)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		foreach (KeyValuePair<string, FBPlayer> fBPlayer in service.FBPlayers)
		{
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject2.AddField("Id", fBPlayer.Value.Id);
			jSONObject2.AddField("Name", fBPlayer.Value.Name);
			jSONObject2.AddField("Playing", fBPlayer.Value.Playing);
			jSONObject2.AddField("Invited", fBPlayer.Value.Invited);
			jSONObject2.AddField("Highscore", fBPlayer.Value.Highscore);
			jSONObject2.AddField("GiftTimeStamp", fBPlayer.Value.GiftTimeStamp);
			jSONObject.AddField(fBPlayer.Key, jSONObject2);
		}
		return jSONObject;
	}

	public static JSONObject GamblingStateToJSON(GamblingState state)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		for (int i = 0; i < 31; i++)
		{
			jSONObject.AddField("Rewards_" + (RewardEnum)i, state.Rewards.Value[i].ToString());
		}
		jSONObject.AddField("FailCost", state.FailCost.Value);
		jSONObject.AddField("FailPaid", state.FailPaid.Value);
		jSONObject.AddField("FailAmount", state.FailAmount.Value);
		jSONObject.AddField("ButtonOneState", state.ButtonOneState.Value);
		jSONObject.AddField("ButtonTwoState", state.ButtonTwoState.Value);
		jSONObject.AddField("ButtonThreeState", state.ButtonThreeState.Value);
		jSONObject.AddField("ButtonFourState", state.ButtonFourState.Value);
		jSONObject.AddField("CurrentGamblingLevel", state.CurrentGamblingLevel.Value);
		for (int j = 0; j < 4; j++)
		{
			jSONObject.AddField("RewardCardType_" + j, (int)state.RewardCards.Value[j].Type);
			jSONObject.AddField("RewardCardAmount_" + j, state.RewardCards.Value[j].Amount.ToString());
		}
		return jSONObject;
	}

	public static JSONObject SkillStateToJSON(SkillState state)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		jSONObject.AddField("Amount", state.Amount.Value);
		jSONObject.AddField("CooldownTimeStamp", state.CooldownTimeStamp.Value);
		jSONObject.AddField("ElapsedTime", state.ElapsedTime.Value);
		jSONObject.AddField("LifetimeUsed", state.LifetimeUsed.Value);
		return jSONObject;
	}

	public static JSONObject HeroStateToJSON(HeroState state)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		jSONObject.AddField("Level", state.Level.Value);
		jSONObject.AddField("LifetimeLevel", state.LifetimeLevel.Value);
		jSONObject.AddField("Tier", state.Tier.Value);
		jSONObject.AddField("Berries", state.Berries.Value);
		jSONObject.AddField("UnusedBerries", state.UnusedBerries.Value);
		return jSONObject;
	}

	public static JSONObject GearStateToJSON(GearState state)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		jSONObject.AddField("Level", state.Level.Value);
		return jSONObject;
	}

	public static JSONObject PlayerGoalStateToJSON(PlayerGoalState state)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		jSONObject.AddField("ID", state.ID);
		jSONObject.AddField("CompletedStars", state.CompletedStars.Value);
		jSONObject.AddField("ClaimedStars", state.ClaimedStars.Value);
		return jSONObject;
	}

	public static JSONObject ListToJSON<T>(List<T> list, Func<T, JSONObject> ToJSON)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		foreach (T item in list)
		{
			jSONObject.Add(ToJSON(item));
		}
		return jSONObject;
	}

	public static Dictionary<string, FBPlayer> JSONToFBPlayers(JSONObject obj)
	{
		Dictionary<string, FBPlayer> dictionary = new Dictionary<string, FBPlayer>();
		foreach (string key in obj.keys)
		{
			dictionary.Add(key, JSONToFBPlayer(obj[key]));
		}
		return dictionary;
	}

	private static FBPlayer JSONToFBPlayer(JSONObject player)
	{
		string id = player.asString("Id", () => string.Empty);
		string name = player.asString("Name", () => string.Empty);
		bool playing = player.asBool("Playing", () => false);
		bool invited = player.asBool("Invited", () => false);
		int highscore = player.asInt("Highscore", () => 0);
		long gitTimeStamp = player.asLong("GiftTimeStamp", () => 0L);
		return new FBPlayer(id, name, playing, invited, highscore, gitTimeStamp);
	}

	public static GamblingState JSONToGamlingState(JSONObject jobj)
	{
		GamblingState gamblingState = new GamblingState();
		gamblingState.Rewards = new ReactiveProperty<BigDouble[]>(new BigDouble[31]);
		for (int i = 0; i < 31; i++)
		{
			gamblingState.Rewards.Value[i] = jobj.asBigDouble("Rewards_" + (RewardEnum)i, () => BigDouble.ZERO);
		}
		gamblingState.FailCost.Value = jobj.asInt("FailCost", () => 25);
		gamblingState.FailPaid.Value = jobj.asBool("FailPaid", () => true);
		gamblingState.FailAmount.Value = jobj.asInt("FailAmount", () => 0);
		gamblingState.ButtonOneState.Value = jobj.asInt("ButtonOneState", () => 0);
		gamblingState.ButtonTwoState.Value = jobj.asInt("ButtonTwoState", () => 0);
		gamblingState.ButtonThreeState.Value = jobj.asInt("ButtonThreeState", () => 0);
		gamblingState.ButtonFourState.Value = jobj.asInt("ButtonFourState", () => 0);
		gamblingState.CurrentGamblingLevel.Value = jobj.asInt("CurrentGamblingLevel", () => -1);
		gamblingState.RewardCards = new ReactiveProperty<RewardData[]>(new RewardData[4]);
		for (int j = 0; j < 4; j++)
		{
			int type = jobj.asInt("RewardCardType_" + j, () => 0);
			BigDouble amount = jobj.asBigDouble("RewardCardAmount_" + j, () => -1L);
			RewardData rewardData = new RewardData((RewardEnum)type, amount);
			gamblingState.RewardCards.Value[j] = rewardData;
		}
		return gamblingState;
	}

	public static SkillState JSONToSkillState(JSONObject jobj)
	{
		SkillState skillState = new SkillState();
		skillState.Amount.Value = jobj.asInt("Amount", () => 0);
		skillState.CooldownTimeStamp.Value = jobj.asLong("CooldownTimeStamp", () => 0L);
		skillState.ElapsedTime.Value = jobj.asLong("ElapsedTime", () => 0L);
		skillState.LifetimeUsed.Value = jobj.asInt("LifetimeUsed", () => 0);
		return skillState;
	}

	public static HeroState JSONToHeroState(JSONObject jobj)
	{
		HeroState heroState = new HeroState();
		heroState.Level.Value = jobj.asInt("Level", () => 0);
		heroState.LifetimeLevel.Value = jobj.asInt("LifetimeLevel", () => 0);
		heroState.Tier.Value = jobj.asInt("Tier", () => 0);
		heroState.Berries.Value = jobj.asInt("Berries", () => 0);
		heroState.UnusedBerries.Value = jobj.asInt("UnusedBerries", () => 0);
		return heroState;
	}

	public static GearState JSONToGearState(JSONObject jobj)
	{
		GearState gearState = new GearState();
		gearState.Level.Value = jobj.asInt("Level", () => 0);
		return gearState;
	}

	public static PlayerGoalState JSONToPlayerGoalState(JSONObject jobj)
	{
		PlayerGoalState playerGoalState = new PlayerGoalState();
		playerGoalState.ID = jobj.asString("ID", () => "Invalid");
		playerGoalState.CompletedStars.Value = jobj.asInt("CompletedStars", () => 0);
		playerGoalState.ClaimedStars.Value = jobj.asInt("ClaimedStars", () => 0);
		return playerGoalState;
	}

	public static List<T> JSONToList<T>(JSONObject jobj, Func<JSONObject, T> ToState)
	{
		List<T> list = new List<T>();
		if (jobj == null)
		{
			return list;
		}
		for (int i = 0; i < jobj.Count; i++)
		{
			list.Add(ToState(jobj[i]));
		}
		return list;
	}

	public static List<string> JSONToStringList(JSONObject jobj)
	{
		List<string> list = new List<string>();
		if (jobj == null)
		{
			return list;
		}
		for (int i = 0; i < jobj.Count; i++)
		{
			list.Add(jobj[i].str);
		}
		return list;
	}

	public static JSONObject AdHistoryToJSON(BaseData baseData)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.ARRAY);
		long cutOff = AdHistory.OldestThatCounts();
		IEnumerable<AdHistory> enumerable = from h in baseData.AdsWatched
			where h.TimeStamp >= cutOff
			select h;
		foreach (AdHistory item in enumerable)
		{
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject2.AddField("Time", item.TimeStamp.ToString());
			jSONObject2.AddField("Placement", item.Placement.ToString());
			jSONObject.Add(jSONObject2);
		}
		return jSONObject;
	}

	public static string ConvertInt16ArrayToString(short[] intArray)
	{
		byte[] array = new byte[intArray.Length * 2];
		for (int i = 0; i < intArray.Length; i++)
		{
			array[i * 2] = BitConverter.GetBytes(intArray[i])[0];
			array[i * 2 + 1] = BitConverter.GetBytes(intArray[i])[1];
		}
		return Convert.ToBase64String(array);
	}

	public static short[] ConvertStringToInt16Array(string run)
	{
		byte[] array = Convert.FromBase64String(run);
		short[] array2 = new short[array.Length / 2];
		for (int i = 0; i < array2.Length; i++)
		{
			byte[] bytes = new byte[2]
			{
				array[i * 2],
				array[i * 2 + 1]
			};
			array2[i] = ByteArrayToInt16(bytes);
		}
		return array2;
	}

	private static short ByteArrayToInt16(byte[] bytes)
	{
		return BitConverter.ToInt16(bytes, 0);
	}
}
