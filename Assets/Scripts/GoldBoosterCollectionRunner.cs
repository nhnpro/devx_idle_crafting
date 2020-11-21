using System.Collections.Generic;
using UniRx;

public class GoldBoosterCollectionRunner : Singleton<GoldBoosterCollectionRunner>
{
	private List<GoldBoosterRunner> m_goldBoosterRunners = new List<GoldBoosterRunner>();

	public ReactiveProperty<int> ActiveBooster = new ReactiveProperty<int>(-1);

	public GoldBoosterRunner GetOrCreateGoldBoosterRunner(int booster)
	{
		m_goldBoosterRunners.EnsureSize(booster, (int count) => new GoldBoosterRunner(count));
		return m_goldBoosterRunners[booster];
	}

	public void SetActiveBooster(int booster)
	{
		ActiveBooster.SetValueAndForceNotify(booster);
		Singleton<BoosterCollectionRunner>.Instance.ActiveBooster.SetValueAndForceNotify(-1);
	}
}
