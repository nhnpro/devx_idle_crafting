using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class BoosterParser
{
	[CompilerGenerated]
	private static Func<string, float> _003C_003Ef__mg_0024cache0;

	public static List<BoosterConfig> ParseBoosters(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new BoosterConfig
			{
				BoosterID = line.asString(0, line.toError<string>()),
				InitialGemCost = line.asInt(1, line.toError<int>()),
				GemCostIncrease = line.asInt(2, line.toError<int>()),
				RewardAmount = line.asCustom(3, BoosterRewardAmountParser.ParseBoosterRewardAmount, line.toError<float>())
			}).ToList();
	}
}
