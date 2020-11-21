using Big;

public class MultiplyCoinRewardAction : RewardAction
{
	private BigDouble m_coinAmount;

	public BigDouble CoinAmount => m_coinAmount;

	public MultiplyCoinRewardAction(RewardData reward, RarityEnum rarity, string friendId)
		: base(reward, rarity, friendId)
	{
		m_coinAmount = Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockReward * PersistentSingleton<GameSettings>.Instance.ChestRewardCoinBiomeMult * base.Reward.Amount;
	}

	public override void GiveReward()
	{
		Singleton<FundRunner>.Instance.AddCoins(CoinAmount);
	}

	public override void GiveFakeReward()
	{
		Singleton<FakeFundRunner>.Instance.AddCoins(CoinAmount);
	}
}
