using System.Collections.Generic;
using System.Linq;

public static class AdFrequencyParser
{
	public static List<AdFrequencyConfig> ParseAdFrequencies(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new AdFrequencyConfig
			{
				Placement = line.asEnum(0, line.toError<AdPlacement>()),
				DailyCap = line.asInt(1, line.toError<int>()),
				Cooldown = line.asInt(2, line.toError<int>())
			}).ToList();
	}
}
