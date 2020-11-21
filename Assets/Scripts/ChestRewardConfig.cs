public struct ChestRewardConfig
{
	public RewardData Reward;

	public RarityEnum Rarity;

	public ChestRewardConfig(RewardData reward, RarityEnum rarity)
	{
		Reward = reward;
		Rarity = rarity;
	}
}
