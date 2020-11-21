using System.Collections.Generic;
using System.Linq;

public static class DrillParser
{
	public static List<DrillConfig> ParseDrills(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new DrillConfig
			{
				Level = line.asInt(0, line.toError<int>()),
				Reward = ParseRewardOrNull(line[1])
			}).ToList();
	}

	private static RewardData ParseRewardOrNull(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		return RewardParser.ParseReward(str);
	}
}
