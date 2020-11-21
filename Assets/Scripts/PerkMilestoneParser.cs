using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class PerkMilestoneParser
{
	[CompilerGenerated]
	private static Func<string, BonusMultConfig> _003C_003Ef__mg_0024cache0;

	public static List<int> ParsePerkMilestonesLevels(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text);
		return source.Take(1).Select(delegate(string[] line)
		{
			List<int> list = new List<int>();
			for (int i = 1; i < line.Length; i++)
			{
				list.Add(line.asInt(i, line.toError<int>()));
			}
			return list;
		}).First();
	}

	public static List<PerkMilestoneConfig> ParsePerkMilestones(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			PerkMilestoneConfig perkMilestoneConfig = new PerkMilestoneConfig
			{
				Hero = line.asInt(0, line.toError<int>())
			};
			for (int i = 1; i < line.Length; i++)
			{
				perkMilestoneConfig.Items.Add(line.asCustom(i, BonusMultParser.ParseBonusMultConfig, line.toError<BonusMultConfig>()));
			}
			return perkMilestoneConfig;
		}).ToList();
	}

	public static List<BonusMultConfig> ParseMiniMilestones(string filename)
	{
		IEnumerable<string[]> source = TSVParser.Parse(filename).Skip(1);
		return (from line in source
			select BonusMultParser.ParseBonusMultConfig(line[0])).ToList();
	}
}
