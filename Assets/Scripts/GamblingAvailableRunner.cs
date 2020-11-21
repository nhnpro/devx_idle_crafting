using System;
using UniRx;
using UnityEngine;

[PropertyClass]
public class GamblingAvailableRunner : Singleton<GamblingAvailableRunner>
{
	public ReactiveProperty<float> GamblingTime;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> GamblingAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> GamblingTimerAvailable;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> GamblingTimeLeft;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> SyncedOrNotFinished;

	public GamblingAvailableRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		GamblingTime = (from _ in (UniRx.IObservable<long>)TickerService.MasterTicksSlow
			select Mathf.Max((float)PersistentSingleton<GameSettings>.Instance.GamblingCooldown - (float)new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.GamblingTimeStamp.Value).TotalSeconds, 0f)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		GamblingAvailable = (from avail in GamblingTime.CombineLatest(PlayerData.Instance.Gambling, (float time, GamblingState state) => time <= 0.0 || state.CurrentGamblingLevel.Value >= 0)
			select (avail)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		GamblingTimerAvailable = (from synced in ServerTimeService.IsSynced
			select (synced)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		GamblingTimeLeft = (from seconds in GamblingTime
			select TextUtils.FormatSecondsShort((long)seconds)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SyncedOrNotFinished = (from synced in ServerTimeService.IsSynced.CombineLatest(PlayerData.Instance.Gambling, (bool synced, GamblingState state) => synced || state.CurrentGamblingLevel.Value >= 0)
			select (synced)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}
}
