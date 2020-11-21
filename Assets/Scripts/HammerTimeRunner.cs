using UniRx;
using UnityEngine;

[PropertyClass]
public class HammerTimeRunner : Singleton<HammerTimeRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> Active;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AdAvailable;

	private ReactiveProperty<int> AdTime;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> AdTimeLeft;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DurationLeft;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DurationString;

	[PropertyBool]
	public ReactiveProperty<bool> HammerTimeEnded;

	private bool HammerTimeActive;

	public HammerTimeRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		ReactiveProperty<int> reactiveProperty = (from dur in PlayerData.Instance.BoostersEffect[5]
			select (int)dur + PersistentSingleton<GameSettings>.Instance.GoldenHammerInitialDuration).TakeUntilDestroy(instance).ToReactiveProperty();
		ReactiveProperty<int> right = reactiveProperty.CombineLatest(PlayerData.Instance.HammerTimeBonusDuration, (int dura, int bonus) => dura + bonus).TakeUntilDestroy(instance).ToReactiveProperty();
		TickerService.MasterTicks.Subscribe(delegate(long ticks)
		{
			if (PlayerData.Instance.HammerTimeElapsedTime.Value < 1728000000000L)
			{
				PlayerData.Instance.HammerTimeElapsedTime.Value += ticks;
			}
		}).AddTo(instance);
		UniRx.IObservable<int> source = PlayerData.Instance.HammerTimeElapsedTime.CombineLatest(right, (long ticks, int dur) => dur - (int)(ticks / 10000000));
		Active = (from secs in source
			select secs > 0).TakeUntilDestroy(instance).ToReactiveProperty();
		DurationLeft = (from seconds in source
			select TextUtils.FormatSecondsShort(seconds)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DurationString = (from dur in reactiveProperty
			select PersistentSingleton<LocalizationService>.Instance.Text("Attribute.Duration") + ": " + BoosterCollectionRunner.FormatSecondsForBoosters(dur)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		HammerTimeEnded = (from act in Active
			select act && (HammerTimeActive = true) into sit
			select (!sit && HammerTimeActive) ? true : false).TakeUntilDestroy(instance).ToReactiveProperty();
		(from elapsed in PlayerData.Instance.HammerTimeElapsedTime
			where elapsed < 0
			select elapsed).CombineLatest(right, (long cheat, int dur) => dur).Subscribe(delegate(int dur)
		{
			PlayerData.Instance.HammerTimeElapsedTime.Value = dur;
		}).AddTo(instance);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		AdTime = (from _ in TickerService.MasterTicks
			select (float)Singleton<AdRunner>.Instance.GetNextTimeToShowAd(AdPlacement.HammerTime).TotalSeconds into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		AdAvailable = (from time in AdTime
			select time <= 0).CombineLatest(Singleton<AdRunner>.Instance.AdReady, (bool time, bool ad) => time && ad).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		AdTimeLeft = AdTime.Select(delegate(int seconds)
		{
			string empty = string.Empty;
			return (seconds > 0) ? TextUtils.FormatSecondsShort(seconds) : PersistentSingleton<LocalizationService>.Instance.Text("AD.Placement.NotAvailable");
		}).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFinished
			where ad == AdPlacement.HammerTime
			select ad).Subscribe(delegate
		{
			HammerTimeAdFinished();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementSkipped
			where ad == AdPlacement.HammerTime
			select ad).Subscribe(delegate
		{
			HammerTimeAdSkipped();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFailed
			where ad == AdPlacement.HammerTime
			select ad).Subscribe(delegate
		{
			HammerTimeAdSkipped();
		}).AddTo(instance);
	}

	public void ActivateHammerTime(int bonus)
	{
		PlayerData.Instance.HammerTimeBonusDuration.Value = bonus;
		PlayerData.Instance.HammerTimeElapsedTime.Value = 0L;
	}

	private void HammerTimeAdFinished()
	{
		ActivateHammerTime(0);
		BindingManager.Instance.GoldenHammerSuccessParent.ShowInfo();
	}

	private void HammerTimeAdSkipped()
	{
		BindingManager.Instance.GoldenHammerEntryParent.ShowInfo();
	}
}
