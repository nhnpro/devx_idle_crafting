public class BoosterRewardAmountParser
{
	public static float ParseBoosterRewardAmount(string raw)
	{
		string[] row = raw.Split("X".ToCharArray());
		return row.asFloat(1, row.toError<float>());
	}
}
