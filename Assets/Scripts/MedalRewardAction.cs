public class MedalRewardAction : RewardAction
{
	public MedalRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddMedals(base.Reward.Amount.ToInt());
	}

	public override void GiveFakeReward()
	{
	}
}
