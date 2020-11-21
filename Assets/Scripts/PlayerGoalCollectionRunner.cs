using System.Collections.Generic;
using System.Linq;
using UniRx;

[PropertyClass]
public class PlayerGoalCollectionRunner : Singleton<PlayerGoalCollectionRunner>
{
	public List<PlayerGoalRunner> PlayerGoalRunners = new List<PlayerGoalRunner>();

	[PropertyBool]
	public ReactiveProperty<bool> NewGoalAvailable = new ReactiveProperty<bool>();

	public PlayerGoalCollectionRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SetupPlayerGoals();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		UniRx.IObservable<bool> observable = Observable.Return<bool>(value: false);
		foreach (PlayerGoalRunner playerGoalRunner in PlayerGoalRunners)
		{
			UniRx.IObservable<bool> observable2 = from pair in playerGoalRunner.Unlocked.Pairwise()
				where !pair.Previous && pair.Current
				select pair into _
				select true;
			observable = observable.Merge(observable2);
		}
		observable.Subscribe(delegate(bool trigger)
		{
			NewGoalAvailable.Value = trigger;
		}).AddTo(instance);
	}

	private void SetupPlayerGoals()
	{
		foreach (PlayerGoalConfig playerGoal in PersistentSingleton<Economies>.Instance.PlayerGoals)
		{
			GetOrCreatePlayerGoalRunner(playerGoal.ID);
		}
	}

	public PlayerGoalRunner GetOrCreatePlayerGoalRunner(string id)
	{
		PlayerGoalRunner playerGoalRunner = PlayerGoalRunners.ToList().Find((PlayerGoalRunner pgr) => pgr.GoalConfig.ID == id);
		if (playerGoalRunner == null)
		{
			PlayerGoalConfig config = PersistentSingleton<Economies>.Instance.PlayerGoals.Find((PlayerGoalConfig cfg) => cfg.ID == id);
			playerGoalRunner = new PlayerGoalRunner(config);
			PlayerGoalRunners.Add(playerGoalRunner);
		}
		return playerGoalRunner;
	}

	public void SeeAllPlayerGoals()
	{
		NewGoalAvailable.Value = false;
		PlayerData.Instance.PlayerGoalsSeen.Clear();
		foreach (PlayerGoalRunner playerGoalRunner in PlayerGoalRunners)
		{
			PlayerData.Instance.PlayerGoalsSeen.Add(playerGoalRunner.GoalConfig.ID);
		}
	}
}
