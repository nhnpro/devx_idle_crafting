using UniRx;

[PropertyClass]
public class HeroNotificaitonRunner : Singleton<HeroNotificaitonRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> HeroUpgradeAvailable;

	[PropertyBool]
	public ReactiveProperty<bool> CompanionUpgradeAvailable;

	public HeroNotificaitonRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		HeroCostRunner orCreateHeroCostRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroCostRunner(0);
		SceneLoader instance = SceneLoader.Instance;
		HeroUpgradeAvailable = orCreateHeroCostRunner.UpgradeAvailable;
		UniRx.IObservable<bool> observable = Observable.Return<bool>(value: false);
		foreach (HeroCostRunner item in Singleton<HeroTeamRunner>.Instance.Costs())
		{
			observable = observable.CombineLatest(item.UpgradeAvailable, (bool combined, bool available) => combined || available);
		}
		CompanionUpgradeAvailable = observable.TakeUntilDestroy(instance).ToReactiveProperty();
	}
}
