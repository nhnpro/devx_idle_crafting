public class GemRewardAction : RewardAction
{
	public GemRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddGems(base.Reward.Amount.ToInt(), base.Reward.Type.ToString(), "rewards");
	}

	public override void GiveFakeReward()
	{
		Singleton<FakeFundRunner>.Instance.AddGems(base.Reward.Amount.ToInt());
	}
}
