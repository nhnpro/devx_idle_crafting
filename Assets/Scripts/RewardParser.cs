using Big;

public class RewardParser
{
	public static RewardData ParseReward(string raw)
	{
		string[] row = raw.Split("X".ToCharArray());
		RewardEnum type = row.asEnum(0, row.toError<RewardEnum>());
		BigDouble amount = row.asBigDouble(1, row.toError<BigDouble>());
		return new RewardData(type, amount);
	}
}
