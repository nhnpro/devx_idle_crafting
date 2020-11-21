using System.Collections.Generic;
using System.Linq;

public static class TierParser
{
	public static List<TierConfig> ParseTiers(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new TierConfig
			{
				Tier = line.asInt(0, line.toError<int>()),
				BerryReq = line.asInt(1, line.toError<int>()),
				DamageMult = line.asFloat(2, line.toError<float>())
			}).ToList();
	}

	public static List<RankConfig> ParseRanks(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new RankConfig
			{
				Tier = line.asInt(0, line.toError<int>()),
				Rank = line.asEnum(1, line.toError<RankEnum>())
			}).ToList();
	}
}
