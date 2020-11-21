using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class TutorialGoalCollectionRunner : Singleton<TutorialGoalCollectionRunner>
{
	private const float GoalDisplayDelay = 3f;

	public List<PlayerGoalRunner> TutorialGoalRunners = new List<PlayerGoalRunner>();

	public ReadOnlyReactiveProperty<PlayerGoalRunner> DelayedCurrentGoal;

	public ReadOnlyReactiveProperty<PlayerGoalRunner> CurrentGoal;

	public ReadOnlyReactiveProperty<bool> TutorialsDone;

	public TutorialGoalCollectionRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		CurrentGoal = (from step in PlayerData.Instance.TutorialStep
			select GetOrCreatePlayerGoalRunner(step)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DelayedCurrentGoal = CurrentGoal.Delay(TimeSpan.FromSeconds(3.0)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TutorialsDone = (from step in PlayerData.Instance.TutorialStep
			select step >= PersistentSingleton<Economies>.Instance.TutorialGoals.Count - 1).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SetupTutorialGoals();
	}

	private void SetupTutorialGoals()
	{
		foreach (PlayerGoalConfig tutorialGoal in PersistentSingleton<Economies>.Instance.TutorialGoals)
		{
			if (tutorialGoal.GetShowInUI())
			{
				GetOrCreatePlayerGoalRunner(tutorialGoal.ID);
			}
		}
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		UniRx.IObservable<int> source = from i in (from runner in DelayedCurrentGoal
				select runner.GoalAction.CompletedStars).Switch()
			where i > 0
			select i;
		source.Subscribe(delegate
		{
			NextTutorialGoal();
		}).AddTo(instance);
	}

	public PlayerGoalRunner GetOrCreatePlayerGoalRunner(int index)
	{
		if (index < 0 || index >= PersistentSingleton<Economies>.Instance.TutorialGoals.Count)
		{
			index = PersistentSingleton<Economies>.Instance.TutorialGoals.Count - 1;
		}
		string iD = PersistentSingleton<Economies>.Instance.TutorialGoals[index].ID;
		return GetOrCreatePlayerGoalRunner(iD);
	}

	private PlayerGoalRunner GetOrCreatePlayerGoalRunner(string id)
	{
		PlayerGoalRunner playerGoalRunner = TutorialGoalRunners.ToList().Find((PlayerGoalRunner pgr) => pgr.GoalConfig.ID == id);
		if (playerGoalRunner == null)
		{
			PlayerGoalConfig config = PersistentSingleton<Economies>.Instance.TutorialGoals.Find((PlayerGoalConfig cfg) => cfg.ID == id);
			playerGoalRunner = new PlayerGoalRunner(config);
			TutorialGoalRunners.Add(playerGoalRunner);
		}
		return playerGoalRunner;
	}

	public void NextTutorialGoal()
	{
		PlayerData.Instance.TutorialStep.Value++;
	}

	public void SkipAllTutorials()
	{
		PlayerData.Instance.TutorialStep.Value = PersistentSingleton<Economies>.Instance.TutorialGoals.Count - 1;
	}
}
