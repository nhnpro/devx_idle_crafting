using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;

[PropertyClass]
public class GearSetRunner
{
	[PropertyInt]
	public ReactiveProperty<int> MaxAllLevel;

	public ReactiveProperty<int> MaxAnyLevel;

	[PropertyString]
	public ReactiveProperty<string> SetBoostText;

	[CompilerGenerated]
	private static Func<int, int, int> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<int, int, int> _003C_003Ef__mg_0024cache1;

	public List<GearRunner> GearRunners
	{
		get;
		private set;
	}

	public GearSetRunner(int setIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		GearRunners = (from gear in Singleton<GearCollectionRunner>.Instance.Gears()
			where gear.SetIndex == setIndex
			select gear).ToList();
		UniRx.IObservable<int> observable = Observable.Return(int.MaxValue);
		foreach (GearRunner gearRunner in GearRunners)
		{
			observable = observable.CombineLatest(gearRunner.Level, Mathf.Min);
		}
		UniRx.IObservable<int> observable2 = Observable.Return(0);
		foreach (GearRunner gearRunner2 in GearRunners)
		{
			observable2 = observable2.CombineLatest(gearRunner2.Level, Mathf.Max);
		}
		MaxAllLevel = observable.TakeUntilDestroy(instance).ToReactiveProperty();
		MaxAnyLevel = observable2.TakeUntilDestroy(instance).ToReactiveProperty();
		GearSet gearSet = PersistentSingleton<Economies>.Instance.GearSets[setIndex];
		SetBoostText = new ReactiveProperty<string>(BonusTypeHelper.GetAttributeText(gearSet.Bonus.BonusType, gearSet.Bonus.Amount));
	}
}
