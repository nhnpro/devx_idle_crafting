using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class GamblingParser
{
	[CompilerGenerated]
	private static Func<string, AmountAndWeight> _003C_003Ef__mg_0024cache0;

	public static List<GamblingConfig> ParseGamblings(string text)
	{
		List<RewardEnum> rewardTypes = ParseRewardTypes(text);
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			GamblingConfig gamblingConfig = new GamblingConfig
			{
				Level = line.asInt(0, line.toError<int>()),
				FailChance = line.asFloat(1, line.toError<float>())
			};
			List<WeightedObject<RewardData>> list = new List<WeightedObject<RewardData>>();
			for (int i = 2; i < line.Length; i++)
			{
				RewardEnum type = rewardTypes[i - 2];
				AmountAndWeight amountAndWeight = line.asCustom(i, AmountAndWeightParser.ParseAmountAndWeight, line.toError<AmountAndWeight>());
				WeightedObject<RewardData> item = new WeightedObject<RewardData>
				{
					Value = new RewardData(type, amountAndWeight.Amount),
					Weight = amountAndWeight.Weight
				};
				list.Add(item);
			}
			gamblingConfig.Rewards = list;
			return gamblingConfig;
		}).ToList();
	}

	private static List<RewardEnum> ParseRewardTypes(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text);
		return source.Take(1).Select(delegate(string[] line)
		{
			List<RewardEnum> list = new List<RewardEnum>();
			for (int i = 2; i < line.Length; i++)
			{
				list.Add(line.asEnum(i, line.toError<RewardEnum>()));
			}
			return list;
		}).First();
	}
}
