using Big;
using System.Collections.Generic;
using System.Linq;

public static class BiomeParser
{
	public static List<BiomeConfig> ParseBiomes(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new BiomeConfig
			{
				BiomeIndex = line.asInt(0, line.toError<int>()),
				Chunk = line.asInt(1, line.toError<int>()),
				BlockHP = line.asBigDouble(2, line.toError<BigDouble>()),
				BlockReward = line.asBigDouble(3, line.toError<BigDouble>()),
				MiniBossHP = line.asBigDouble(4, line.toError<BigDouble>()),
				MiniBossReward = line.asBigDouble(5, line.toError<BigDouble>()),
				BossHP = line.asBigDouble(6, line.toError<BigDouble>()),
				BossReward = line.asBigDouble(7, line.toError<BigDouble>())
			}).ToList();
	}
}
