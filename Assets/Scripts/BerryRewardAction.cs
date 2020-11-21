public class BerryRewardAction : RewardAction
{
	public int HeroIndex;

	public BerryRewardAction(RewardData reward, RarityEnum rarity, int heroIndex, string friendId)
		: base(reward, rarity, friendId)
	{
		HeroIndex = heroIndex;
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddBerry(base.Reward.Amount.ToInt(), HeroIndex, "rewards");
	}

	public override void GiveFakeReward()
	{
	}
}
