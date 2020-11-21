using Big;
using UniRx;
using UnityEngine;

[PropertyClass]
public class PlayerGoalRunner
{
	public PlayerGoalAction GoalAction;

	[PropertyString]
	public ReactiveProperty<string> GoalName;

	[PropertyBool]
	public ReactiveProperty<bool> ClaimAvailable;

	[PropertyInt]
	public ReactiveProperty<int> CompletedStars;

	[PropertyInt]
	public ReactiveProperty<int> ClaimedStars;

	[PropertyBool]
	public ReactiveProperty<bool> FullyCompleted;

	[PropertyFloat]
	public ReactiveProperty<float> Progress;

	[PropertyString]
	public ReactiveProperty<string> ProgressText;

	[PropertyInt]
	public ReactiveProperty<int> GemReward;

	[PropertyBool]
	public ReactiveProperty<bool> Unlocked;

	public PlayerGoalState GoalState
	{
		get;
		private set;
	}

	public PlayerGoalConfig GoalConfig
	{
		get;
		private set;
	}

	public PlayerGoalRunner(PlayerGoalConfig config)
	{
		SceneLoader instance = SceneLoader.Instance;
		UIIngameNotifications IngameNotifications = BindingManager.Instance.IngameNotifications;
		GoalConfig = config;
		GoalState = PlayerGoalStateFactory.GetOrCreatePlayerGoalState(config.ID);
		GoalAction = PlayerGoalActionFactory.Create(GoalConfig, GoalState);
		GoalName = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("PlayerGoal.Title." + config.ID));
		ClaimedStars = GoalState.ClaimedStars;
		GoalAction.CompletedStars.Subscribe(delegate(int val)
		{
			GoalState.CompletedStars.Value = val;
		}).AddTo(instance);
		if (GoalConfig.IsTutorialGoal)
		{
			Unlocked = (from step in PlayerData.Instance.TutorialStep
				select step >= GoalConfig.Index).TakeUntilDestroy(instance).ToReactiveProperty();
		}
		else
		{
			Unlocked = (from lvl in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(0).LifetimeLevel
				select lvl >= GoalConfig.HeroLevelReq).TakeUntilDestroy(instance).ToReactiveProperty();
		}
		ClaimAvailable = GoalState.CompletedStars.CombineLatest(GoalState.ClaimedStars, (int comp, int claim) => comp > claim).CombineLatest(Unlocked, (bool claimAvailable, bool unlocked) => claimAvailable && unlocked).TakeUntilDestroy(instance)
			.ToReactiveProperty();
		GoalState.CompletedStars.Skip(1).Subscribe(delegate
		{
			IngameNotifications.InstantiatePlayerGoalNotification(this);
		}).AddTo(instance);
		CompletedStars = GoalAction.CompletedStars.TakeUntilDestroy(instance).ToReactiveProperty();
		CompletedStars.Skip(1).Subscribe(delegate
		{
			if (PersistentSingleton<GameAnalytics>.Instance != null)
			{
				PersistentSingleton<GameAnalytics>.Instance.GoalCompleted.Value = this;
			}
		}).AddTo(instance);
		FullyCompleted = (from max in GoalAction.ProgressMax
			select max < BigDouble.ZERO).TakeUntilDestroy(instance).ToReactiveProperty();
		(from full in FullyCompleted
			where full
			select full).Take(1).Subscribe(delegate
		{
			ReportAchievement(GoalConfig);
		}).AddTo(instance);
		Progress = GoalAction.Progress.TakeUntilDestroy(instance).ToReactiveProperty();
		ProgressText = GoalAction.ProgressCurrent.CombineLatest(GoalAction.ProgressMax, (BigDouble cur, BigDouble max) => BigString.ToString(cur) + "/" + BigString.ToString(max)).TakeUntilDestroy(instance).ToReactiveProperty();
		GemReward = (from stars in GoalState.ClaimedStars
			select Mathf.Min(stars, PersistentSingleton<Economies>.Instance.PlayerGoalRewards.Count - 1) into stars
			select PersistentSingleton<Economies>.Instance.PlayerGoalRewards[stars].GemReward).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public void Claim(Vector3 spawnPoint)
	{
		if (ClaimAvailable.Value)
		{
			for (int i = 0; i < GemReward.Value; i++)
			{
				BindingManager.Instance.UIGemsTarget.SlerpFromHud(spawnPoint);
			}
			Singleton<FundRunner>.Instance.AddGems(GemReward.Value, "goalReward", "rewards");
			GoalState.ClaimedStars.Value++;
		}
	}

	private void ReportAchievement(PlayerGoalConfig cfg)
	{
		if (!string.IsNullOrEmpty(cfg.GoogleID))
		{
			PersistentSingleton<AchievementsService>.Instance.ReportProgress(cfg.GoogleID, 100f);
		}
	}
}
