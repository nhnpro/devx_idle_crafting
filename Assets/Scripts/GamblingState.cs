using Big;
using UniRx;

public class GamblingState
{
	public ReactiveProperty<BigDouble[]> Rewards = new ReactiveProperty<BigDouble[]>(new BigDouble[31]);

	public ReactiveProperty<int> FailCost = new ReactiveProperty<int>(PersistentSingleton<GameSettings>.Instance.InitialGamblingFailCost);

	public ReactiveProperty<bool> FailPaid = new ReactiveProperty<bool>(initialValue: true);

	public ReactiveProperty<int> FailAmount = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> ButtonOneState = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> ButtonTwoState = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> ButtonThreeState = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> ButtonFourState = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> CurrentGamblingLevel = new ReactiveProperty<int>(-1);

	public ReactiveProperty<RewardData[]> RewardCards = new ReactiveProperty<RewardData[]>(new RewardData[4]);

	public GamblingState()
	{
		for (int i = 0; i < Rewards.Value.Length; i++)
		{
			Rewards.Value[i] = BigDouble.ZERO;
		}
		for (int j = 0; j < RewardCards.Value.Length; j++)
		{
			RewardCards.Value[j] = new RewardData(RewardEnum.Invalid, BigDouble.ZERO);
		}
	}
}
