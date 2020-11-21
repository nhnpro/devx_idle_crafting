using Big;
using System.Collections.Generic;
using System.Linq;

public static class PlayerGoalParser
{
	public static List<PlayerGoalConfig> ParsePlayerGoals(string file, bool tutorial)
	{
		List<string[]> list = TSVParser.Parse(file).Skip(1).ToList();
		List<PlayerGoalConfig> list2 = new List<PlayerGoalConfig>();
		for (int i = 0; i < list.Count; i++)
		{
			string[] array = list[i];
			PlayerGoalConfig playerGoalConfig = new PlayerGoalConfig();
			playerGoalConfig.ID = array.asString(0, array.toError<string>());
			playerGoalConfig.Index = i;
			playerGoalConfig.AppleID = array.asString(1, array.toError<string>());
			playerGoalConfig.GoogleID = array.asString(2, array.toError<string>());
			playerGoalConfig.Task = array.asEnum(3, array.toError<PlayerGoalTask>());
			playerGoalConfig.Parameter = ParseTaskParameter(array, 4, playerGoalConfig.Task);
			playerGoalConfig.HeroLevelReq = array.asInt(5, array.toError<int>());
			for (int j = 0; j < 5; j++)
			{
				playerGoalConfig.StarReq[j] = array.asBigDouble(6 + j, array.toError<BigDouble>());
			}
			playerGoalConfig.IsTutorialGoal = tutorial;
			list2.Add(playerGoalConfig);
		}
		return list2;
	}

	private static int ParseTaskParameter(string[] line, int index, PlayerGoalTask target)
	{
		switch (target)
		{
		case PlayerGoalTask.HeroLevel:
		case PlayerGoalTask.HeroTier:
			return line.asInt(index, line.toError<int>());
		case PlayerGoalTask.AmountSkillUsed:
			return (int)line.asEnum(index, line.toError<SkillsEnum>());
		default:
			return -1;
		}
	}

	public static List<PlayerGoalRewards> ParseRewards(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new PlayerGoalRewards
			{
				Stars = line.asInt(0, line.toError<int>()),
				GemReward = line.asInt(1, line.toError<int>())
			}).ToList();
	}
}
