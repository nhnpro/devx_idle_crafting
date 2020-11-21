using UniRx;

[PropertyClass]
public class PlayerGoalClaimRunner : Singleton<PlayerGoalClaimRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> ClaimAvailable;

	public PlayerGoalClaimRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		UniRx.IObservable<bool> left = Observable.Return<bool>(value: false);
		foreach (PlayerGoalRunner playerGoalRunner in Singleton<PlayerGoalCollectionRunner>.Instance.PlayerGoalRunners)
		{
			left = left.CombineLatest(playerGoalRunner.ClaimAvailable, (bool acc, bool claim) => acc || claim);
		}
		UniRx.IObservable<bool> observable = Observable.Return<bool>(value: false);
		foreach (PlayerGoalRunner tutorialGoalRunner in Singleton<TutorialGoalCollectionRunner>.Instance.TutorialGoalRunners)
		{
			observable = observable.CombineLatest(tutorialGoalRunner.ClaimAvailable, (bool acc, bool claim) => acc || claim);
		}
		ClaimAvailable = left.CombineLatest(observable, (bool a, bool b) => a || b).TakeUntilDestroy(instance).ToReactiveProperty();
	}
}
