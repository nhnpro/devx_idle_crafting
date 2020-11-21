using UniRx;
using UnityEngine;

[PropertyClass]
public class GearMilestoneRunner : Singleton<GearMilestoneRunner>
{
	[PropertyInt]
	public ReadOnlyReactiveProperty<int> Gears;

	[PropertyFloat]
	public ReadOnlyReactiveProperty<float> Progress;

	[PropertyBool]
	public ReactiveProperty<bool> AnimationTriggered = new ReactiveProperty<bool>();

	[PropertyString]
	public ReadOnlyReactiveProperty<string> ProgressText;

	private UniRx.IObservable<int> GearChests;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> GearChestsToCollect;

	public GearMilestoneRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		GearChests = CreateGearChestsObservable();
		Gears = (from gears in Singleton<GearCollectionRunner>.Instance.GearLevels
			select gears % PersistentSingleton<GameSettings>.Instance.GearMilestoneCount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		Progress = (from gears in Gears
			select (float)gears / (float)PersistentSingleton<GameSettings>.Instance.GearMilestoneCount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		ProgressText = (from gears in Gears
			select gears + "/" + PersistentSingleton<GameSettings>.Instance.GearMilestoneCount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		GearChestsToCollect = (from chests in GearChests
			select (chests > 0) ? true : false).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public UniRx.IObservable<int> CreateGearChestsObservable()
	{
		UniRx.IObservable<int> observable = Observable.Return(0);
		foreach (ReactiveProperty<int> item in PlayerData.Instance.GearChestsToCollect)
		{
			observable = observable.CombineLatest(item, (int org, int num) => org + num);
		}
		return observable;
	}

	public void CollectGearChests(Transform start)
	{
		int num = 0;
		for (int i = 0; i < PlayerData.Instance.GearChestsToCollect.Count; i++)
		{
			int value = PlayerData.Instance.GearChestsToCollect[i].Value;
			num += value;
			switch (i)
			{
			case 1:
				Singleton<FundRunner>.Instance.AddNormalChests(value, "gearChests");
				break;
			case 2:
				Singleton<FundRunner>.Instance.AddSilverChests(value, "gearChests");
				break;
			case 3:
				Singleton<FundRunner>.Instance.AddGoldChests(value, "gearChests");
				break;
			}
			PlayerData.Instance.GearChestsToCollect[i].Value = 0;
		}
		for (int j = 0; j < num; j++)
		{
			BindingManager.Instance.UIChestsTarget.SlerpFromHud(start.position);
		}
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from pair in Gears.Pairwise()
			where pair.Current == 0 && pair.Previous > 0
			select pair).Subscribe(delegate
		{
			PlayerData.Instance.GearChestsToCollect[1].Value++;
			AnimationTriggered.Value = true;
		}).AddTo(instance);
	}

	public void AnimationEnded()
	{
		AnimationTriggered.Value = false;
	}
}
