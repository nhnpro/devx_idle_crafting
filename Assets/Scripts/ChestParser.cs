using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class ChestParser
{
	[CompilerGenerated]
	private static Func<string, RewardData> _003C_003Ef__mg_0024cache0;

	public static List<WeightedObject<ChestRewardConfig>> ParseChests(string text, ChestEnum chest)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new WeightedObject<ChestRewardConfig>
			{
				Value = new ChestRewardConfig(line.asCustom(0, RewardParser.ParseReward, line.toError<RewardData>()), line.asEnum(1, line.toError<RarityEnum>())),
				Weight = line.asFloat((int)(chest + 2), line.toError<float>())
			}).ToList();
	}
}
