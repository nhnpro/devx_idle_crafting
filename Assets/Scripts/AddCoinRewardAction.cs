public class AddCoinRewardAction : RewardAction
{
	public AddCoinRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddCoins(base.Reward.Amount);
	}

	public override void GiveFakeReward()
	{
		Singleton<FakeFundRunner>.Instance.AddCoins(base.Reward.Amount);
	}
}
