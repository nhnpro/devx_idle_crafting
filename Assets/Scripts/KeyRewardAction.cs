public class KeyRewardAction : RewardAction
{
	public KeyRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddKeys(base.Reward.Amount.ToInt(), base.Reward.Type.ToString());
	}

	public override void GiveFakeReward()
	{
		Singleton<FakeFundRunner>.Instance.AddKeys(base.Reward.Amount.ToInt());
	}
}
