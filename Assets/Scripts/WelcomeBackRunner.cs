using Big;
using System;
using UniRx;
using UnityEngine;

[PropertyClass]
public class WelcomeBackRunner : Singleton<WelcomeBackRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> WelcomeBackAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DoubleAdAvailable;

	private ReactiveProperty<int> DoubleAdTime;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DoubleAdTimeLeft;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> Coins;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> CollectedCoins = new ReactiveProperty<BigDouble>(0L);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> WelcomeBackMultiplier;

	private int m_stepIndex;

	public WelcomeBackRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		m_stepIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == "Tutorial.ReachChunk");
		WelcomeBackAvailable = (from avail in PlayerData.Instance.WelcomebackCoins.CombineLatest(ServerTimeService.IsSynced, (BigDouble coins, bool sync) => coins > 100L && sync)
			select (avail)).TakeUntilDestroy(instance).ToReactiveProperty();
		Coins = PlayerData.Instance.WelcomebackCoins.TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		if (PlayerData.Instance.WelcomebackTimeStamp.Value == 0)
		{
			PlayerData.Instance.WelcomebackTimeStamp.Value = ServerTimeService.NowTicks();
		}
		UniRx.IObservable<bool> source = from p in Observable.EveryApplicationPause().StartWith(value: false)
			select (p);
		(from _ in (from pause in source
				where !pause
				select pause into _
				select (from sync in ServerTimeService.IsSynced
					where sync
					select sync).Take(1)).Switch()
			select new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.WelcomebackTimeStamp.Value).TotalSeconds into awaySecs
			where awaySecs >= 90.0 && PlayerData.Instance.TutorialStep.Value >= m_stepIndex
			select awaySecs into _
			select ServerTimeService.NowTicks() - PlayerData.Instance.WelcomebackTimeStamp.Value into deltaTicks
			select GetWelcomeBackCoins(deltaTicks)).Subscribe(delegate(BigDouble coins)
		{
			PlayerData.Instance.WelcomebackTimeStamp.Value = ServerTimeService.NowTicks();
			PlayerData.Instance.WelcomebackCoins.Value += coins;
		}).AddTo(instance);
		(from _ in PersistentSingleton<MainSaver>.Instance.Saving
			select (from sync in ServerTimeService.IsSynced
				where sync
				select sync).Take(1)).Switch().Subscribe(delegate
		{
			PlayerData.Instance.WelcomebackTimeStamp.Value = ServerTimeService.NowTicks();
		}).AddTo(instance);
		(from prestige in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where prestige == PrestigeOrder.PrestigeInit
			select prestige).Subscribe(delegate
		{
			PlayerData.Instance.WelcomebackCoins.Value = 0L;
		}).AddTo(instance);
		DoubleAdTime = (from _ in TickerService.MasterTicksSlow
			select (float)Singleton<AdRunner>.Instance.GetNextTimeToShowAd(AdPlacement.DailyDouble).TotalSeconds into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		DoubleAdAvailable = (from time in DoubleAdTime
			select time <= 0).CombineLatest(Singleton<AdRunner>.Instance.AdReady, (bool time, bool ad) => time && ad).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DoubleAdTimeLeft = DoubleAdTime.Select(delegate(int seconds)
		{
			string empty = string.Empty;
			return (seconds > 0) ? TextUtils.FormatSecondsShort(seconds) : PersistentSingleton<LocalizationService>.Instance.Text("AD.Placement.NotAvailable");
		}).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFinished
			where ad == AdPlacement.DailyDouble
			select ad).Subscribe(delegate
		{
			DailyDoubleAdFinished();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementSkipped
			where ad == AdPlacement.DailyDouble
			select ad).Subscribe(delegate
		{
			DailyDoubleAdSkipped();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFailed
			where ad == AdPlacement.DailyDouble
			select ad).Subscribe(delegate
		{
			DailyDoubleAdSkipped();
		}).AddTo(instance);
		WelcomeBackMultiplier = (from _ in PlayerData.Instance.BoostersEffect[3].AsObservable()
			select BoosterCollectionRunner.GetBoosterBonusString(BoosterEnum.DailyDoubleBoost)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	private BigDouble GetWelcomeBackCoins(long deltaTicks)
	{
		float num = 1f;
		if (PlayerData.Instance.LifetimePrestiges.Value > 0 || PlayerData.Instance.MainChunk.Value > 70)
		{
			num = 0.5f;
		}
		BigDouble right = BigDouble.Min(Singleton<HeroTeamRunner>.Instance.GetTeamDamage() / Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockHP * num, new BigDouble(PersistentSingleton<GameSettings>.Instance.WelcomeBackBlocksPerHero) * Singleton<HeroUnlockRunner>.Instance.NumUnlockedHeroes.Value);
		double value = Math.Min(new TimeSpan(deltaTicks).TotalSeconds, TimeSpan.FromDays(2.0).TotalSeconds);
		return Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockReward * (value * right) * PersistentSingleton<GameSettings>.Instance.WelcomeBackRewardMultiplier;
	}

	public void AddWelcomeBackCoins(double seconds)
	{
		float num = 1f;
		if (PlayerData.Instance.LifetimePrestiges.Value > 0 || PlayerData.Instance.MainChunk.Value > 70)
		{
			num = 0.5f;
		}
		BigDouble right = BigDouble.Min(Singleton<HeroTeamRunner>.Instance.GetTeamDamage() / Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockHP * num, new BigDouble(PersistentSingleton<GameSettings>.Instance.WelcomeBackBlocksPerHero) * Singleton<HeroUnlockRunner>.Instance.NumUnlockedHeroes.Value);
		double value = Math.Min(seconds, TimeSpan.FromDays(2.0).TotalSeconds);
		PlayerData.Instance.WelcomebackCoins.Value += Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockReward * (value * right) * PersistentSingleton<GameSettings>.Instance.WelcomeBackRewardMultiplier;
	}

	public void PerformWelcomeBack()
	{
		Singleton<FundRunner>.Instance.AddCoins(Coins.Value);
		SlerpTarget target = BindingManager.Instance.CoinsTarget;
		UIPopupManager popup = BindingManager.Instance.WelcomeBackParent;
		for (int i = 0; i < 10; i++)
		{
			Observable.Return(value: true).Delay(TimeSpan.FromSeconds((double)i * 0.1)).Take(1)
				.Subscribe(delegate
				{
					target.SlerpFromHud(popup.transform.position);
				});
		}
		CollectedCoins.Value = Coins.Value;
		PlayerData.Instance.WelcomebackCoins.Value = 0L;
	}

	private void DailyDoubleWelcomeBack()
	{
		PlayerData.Instance.WelcomebackCoins.Value *= (BigDouble)PlayerData.Instance.BoostersEffect[3].Value;
	}

	private void DailyDoubleAdFinished()
	{
		DailyDoubleWelcomeBack();
		PerformWelcomeBack();
		BindingManager.Instance.WelcomeBackSuccessParent.ShowInfo();
	}

	private void DailyDoubleAdSkipped()
	{
		BindingManager.Instance.WelcomeBackParent.ShowInfo();
	}

	private double GetWelcomeBackTime()
	{
		if (Singleton<HeroUnlockRunner>.Instance.NumUnlockedHeroes.Value > 1)
		{
			if (GetWelcomeBackCoins(6000000000L) > 100L)
			{
				return 600.0;
			}
			return (100L / (Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockReward * 0.10000000149011612)).ToDouble();
		}
		return -1.0;
	}
}
