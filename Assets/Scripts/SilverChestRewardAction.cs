public class SilverChestRewardAction : RewardAction
{
	public SilverChestRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddSilverChests(base.Reward.Amount.ToInt(), "rewards");
	}

	public override void GiveFakeReward()
	{
	}
}
