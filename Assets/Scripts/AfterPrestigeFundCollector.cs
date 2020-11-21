using Big;
using UniRx;

public class AfterPrestigeFundCollector : Singleton<AfterPrestigeFundCollector>
{
	public ReactiveProperty<long>[] FundsAfterPrestige = new ReactiveProperty<long>[7];

	public AfterPrestigeFundCollector()
	{
		SceneLoader instance = SceneLoader.Instance;
		for (int i = 0; i < FundsAfterPrestige.Length; i++)
		{
			FundsAfterPrestige[i] = CreateFundsObservable(i).TakeUntilDestroy(instance).ToReactiveProperty();
		}
	}

	private UniRx.IObservable<long> CreateFundsObservable(int typeIndex)
	{
		UniRx.IObservable<long> right = PlayerData.Instance.BlocksInBackpack[typeIndex].CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[13 + typeIndex], (long back, BigDouble mult) => back * mult.ToLong());
		return PlayerData.Instance.BlocksCollected[typeIndex].CombineLatest(right, (long collected, long prestige) => collected + prestige);
	}
}
