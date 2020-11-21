public class RewardAction
{
	public RewardData Reward
	{
		get;
		private set;
	}

	public RarityEnum Rarity
	{
		get;
		private set;
	}

	public string FriendId
	{
		get;
		private set;
	}

	public RewardAction(RewardData reward, RarityEnum rarity, string friendId)
	{
		Reward = reward;
		Rarity = rarity;
		FriendId = friendId;
	}

	public virtual void GiveReward()
	{
	}

	public virtual void GiveFakeReward()
	{
	}
}
