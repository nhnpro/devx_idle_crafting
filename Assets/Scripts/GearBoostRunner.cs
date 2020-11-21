using System.Collections.Generic;
using System.Linq;
using UniRx;

public class GearBoostRunner : Singleton<GearBoostRunner>
{
	public ReactiveProperty<float>[] BonusMult = new ReactiveProperty<float>[25];

	public GearBoostRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		for (int i = 0; i < 25; i++)
		{
			BonusMult[i] = CreateObservableForAll((BonusTypeEnum)i).TakeUntilDestroy(instance).ToReactiveProperty();
		}
	}

	private UniRx.IObservable<float> CreateObservableForAll(BonusTypeEnum bonusType)
	{
		List<UniRx.IObservable<float>> mult = (from config in PersistentSingleton<Economies>.Instance.Gears
			where config.Boost1.Mult.BonusType == bonusType
			select Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(config.GearIndex).Boost1Amount.AsObservable()).ToList();
		List<UniRx.IObservable<float>> mult2 = (from config in PersistentSingleton<Economies>.Instance.Gears
			where config.Boost2.Mult.BonusType == bonusType
			select Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(config.GearIndex).Boost2Amount.AsObservable()).ToList();
		return BonusTypeHelper.CreateCombineStacking(bonusType, mult, mult2);
	}
}
