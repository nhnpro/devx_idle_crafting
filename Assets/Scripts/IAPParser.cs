using System.Collections.Generic;
using System.Linq;

public static class IAPParser
{
	public const int NumExtraRewards = 2;

	public static List<IAPConfig> ParseIAPs(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			IAPConfig iAPConfig = new IAPConfig
			{
				AppleID = line.asString(0, line.toError<string>()),
				GoogleID = line.asString(1, line.toError<string>()),
				Type = line.asEnum(2, line.toError<IAPType>()),
				ProductEnum = line.asEnum(3, line.toError<IAPProductEnum>()),
				Reward = ParseRewardOrNull(line[4])
			};
			iAPConfig.ExtraRewards[0] = ParseRewardOrNull(line[5]);
			iAPConfig.ExtraRewards[1] = ParseRewardOrNull(line[6]);
			iAPConfig.SaleEnum = line.asEnum(7, line.toError<IAPProductEnum>());
			return iAPConfig;
		}).ToList();
	}

	private static RewardData ParseRewardOrNull(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		return RewardParser.ParseReward(str);
	}
}
