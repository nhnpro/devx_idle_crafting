using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class GearParser
{
	[CompilerGenerated]
	private static Func<string, BonusMultConfig> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<string, BonusMultConfig> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<string, BonusMultConfig> _003C_003Ef__mg_0024cache2;

	public static List<GearConfig> ParseGears(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new GearConfig
			{
				GearIndex = line.asInt(0, line.toError<int>()),
				Category = line.asEnum(2, line.toError<GearCategory>()),
				UpgradeCostA = line.asFloat(4, line.toError<float>()),
				UpgradeCostB = line.asFloat(5, line.toError<float>()),
				UpgradeCostC = line.asFloat(6, line.toError<float>()),
				MaxLevel = line.asInt(7, line.toError<int>()),
				Boost1 = new GearBoostStruct
				{
					Mult = line.asCustom(8, BonusMultParser.ParseBonusMultConfig, line.toError<BonusMultConfig>()),
					Operator = line.asEnum(9, line.toError<BoostOperatorEnum>()),
					LevelUpAmount = line.asFloat(10, line.toError<float>())
				},
				Boost2 = new GearBoostStruct
				{
					Mult = line.asCustom(11, BonusMultParser.ParseBonusMultConfig, line.toError<BonusMultConfig>()),
					Operator = line.asEnum(12, line.toError<BoostOperatorEnum>()),
					LevelUpAmount = line.asFloat(13, line.toError<float>())
				}
			}).ToList();
	}

	public static List<GearSet> ParseGearSets(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new GearSet
			{
				SetIndex = line.asInt(0, line.toError<int>()),
				ChunkUnlockLevel = line.asInt(1, line.toError<int>()),
				Bonus = line.asCustom(2, BonusMultParser.ParseBonusMultConfig, line.toError<BonusMultConfig>())
			}).ToList();
	}
}
