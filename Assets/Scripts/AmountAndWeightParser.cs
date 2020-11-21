using Big;

public class AmountAndWeightParser
{
	public static AmountAndWeight ParseAmountAndWeight(string raw)
	{
		string[] row = raw.Split("X".ToCharArray());
		BigDouble amount = row.asBigDouble(0, row.toError<BigDouble>());
		float weight = row.asFloat(1, row.toError<float>());
		return new AmountAndWeight(amount, weight);
	}
}
