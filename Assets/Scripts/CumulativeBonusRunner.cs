using Big;
using System;
using UniRx;

public class CumulativeBonusRunner : Singleton<CumulativeBonusRunner>
{
	public ReactiveProperty<BigDouble>[] BonusMult = new ReactiveProperty<BigDouble>[25];

	public CumulativeBonusRunner()
	{
		for (int i = 0; i < 25; i++)
		{
			BonusMult[i] = new ReactiveProperty<BigDouble>(BonusTypeHelper.GetOrigin((BonusTypeEnum)i));
		}
	}

	public void PostInit()
	{
		for (int i = 0; i < 25; i++)
		{
			CreateSubscribe(i);
		}
	}

	private void CreateSubscribe(int typeIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		CreateCombinedObservable(typeIndex).Subscribe(delegate(BigDouble mult)
		{
			BonusMult[typeIndex].Value = mult;
		}).AddTo(instance);
	}

	private UniRx.IObservable<BigDouble> CreateCombinedObservable(int typeIndex)
	{
		Func<BigDouble, float, BigDouble> func = BonusTypeHelper.CreateBigDoubleFunction((BonusTypeEnum)typeIndex);
		return from bonus in Singleton<PerkMilestoneRunner>.Instance.BonusMult[typeIndex].CombineLatest(Singleton<GearBoostRunner>.Instance.BonusMult[typeIndex], (BigDouble bonus, float mult) => func(bonus, mult)).CombineLatest(Singleton<GearSetCollectionRunner>.Instance.BonusMult[typeIndex], (BigDouble bonus, float mult) => func(bonus, mult))
			select (!(bonus >= BigDouble.ZERO)) ? BigDouble.ZERO : bonus;
	}
}
