public class RelicRewardAction : RewardAction
{
	public RelicRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddRelicsToBackpak(base.Reward.Amount);
	}

	public override void GiveFakeReward()
	{
	}
}
