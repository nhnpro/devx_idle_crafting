using Big;

public class RewardData
{
	public RewardEnum Type
	{
		get;
		private set;
	}

	public BigDouble Amount
	{
		get;
		private set;
	}

	public RewardData(RewardEnum type, BigDouble amount)
	{
		Type = type;
		Amount = amount;
	}
}
