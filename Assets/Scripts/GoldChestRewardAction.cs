public class GoldChestRewardAction : RewardAction
{
	public GoldChestRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddGoldChests(base.Reward.Amount.ToInt(), "rewards");
	}

	public override void GiveFakeReward()
	{
	}
}
