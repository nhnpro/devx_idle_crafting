using Big;
using System.Collections.Generic;
using System.Linq;

public static class HeroParser
{
	public static List<HeroConfig> ParseHeros(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new HeroConfig
			{
				HeroIndex = line.asInt(0, line.toError<int>()),
				Category = line.asEnum(1, line.toError<HeroCategory>()),
				UnlockAtChunk = line.asInt(2, line.toError<int>()),
				InitialCost = line.asBigDouble(3, line.toError<BigDouble>()),
				CostMultiplier = line.asFloat(4, line.toError<float>()),
				MilestoneMultiplier = line.asFloat(5, line.toError<float>()),
				DamageMultiplier = line.asFloat(6, line.toError<float>()),
				InitialDamage = line.asBigDouble(7, line.toError<BigDouble>())
			}).ToList();
	}
}
