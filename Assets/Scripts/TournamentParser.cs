using Big;
using System.Collections.Generic;
using System.Linq;

public static class TournamentParser
{
	public static List<TournamentConfig> ParseTournaments(string text)
	{
		List<RewardEnum> rewardTypes = ParseRewardTypes(text);
		IEnumerable<string[]> source = TSVParser.Parse(text + "\t").Skip(1);
		return source.Select(delegate(string[] line)
		{
			TournamentConfig tournamentConfig = new TournamentConfig
			{
				Rank = line.asInt(0, line.toError<int>())
			};
			tournamentConfig.Rewards[0] = new RewardData(rewardTypes[0], line.asBigDouble(1, line.toError<BigDouble>()));
			tournamentConfig.Rewards[1] = new RewardData(rewardTypes[1], line.asBigDouble(2, line.toError<BigDouble>()));
			tournamentConfig.Rewards[2] = new RewardData(rewardTypes[2], line.asBigDouble(3, line.toError<BigDouble>()));
			return tournamentConfig;
		}).ToList();
	}

	private static List<RewardEnum> ParseRewardTypes(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text);
		return source.Take(1).Select(delegate(string[] line)
		{
			List<RewardEnum> list = new List<RewardEnum>();
			for (int i = 1; i < line.Length; i++)
			{
				list.Add(line.asEnum(i, line.toError<RewardEnum>()));
			}
			return list;
		}).First();
	}

	public static List<TournamentTierConfig> ParseTournamentTiers(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text + "\t").Skip(1);
		return (from line in source
			select new TournamentTierConfig
			{
				Tier = line.asEnum(0, line.toError<TournamentTier>()),
				Bonus = BonusMultParser.ParseBonusMultConfig(line[1]),
				Requirement = line.asInt(2, line.toError<int>())
			}).ToList();
	}
}
