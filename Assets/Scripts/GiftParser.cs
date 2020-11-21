using System.Collections.Generic;
using System.Linq;

public static class GiftParser
{
	public static List<WeightedObject<RewardData>> ParseGiftRewards(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new WeightedObject<RewardData>
			{
				Value = RewardParser.ParseReward(line[0]),
				Weight = line.asInt(1, line.toError<int>())
			}).ToList();
	}
}
