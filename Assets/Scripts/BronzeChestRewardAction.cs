public class BronzeChestRewardAction : RewardAction
{
	public BronzeChestRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddNormalChests(base.Reward.Amount.ToInt(), "rewards");
	}

	public override void GiveFakeReward()
	{
	}
}
