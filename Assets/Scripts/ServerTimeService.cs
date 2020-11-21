using System;
using UniRx;
using UnityEngine;

public static class ServerTimeService
{
	private struct ServerTime
	{
		public long utcTime;

		public long localOffset;
	}

	public const int SyncCycleSeconds = 30;

	public static ReactiveProperty<string> TimeLabelForTweakable = new ReactiveProperty<string>();

	public static ReactiveProperty<TimeSyncStatus> Status = new ReactiveProperty<TimeSyncStatus>(TimeSyncStatus.NotSynced);

	public static UniRx.IObservable<bool> IsSynced;

	public static UniRx.IObservable<long> Syncs;

	private static ReadOnlyReactiveProperty<ServerTime> ServerTimes;

	public static bool Inited
	{
		get;
		private set;
	}

	public static void StartUp()
	{
		if (!Inited)
		{
			UniRx.IObservable<long> notSyncedTriggers = (from a in ConnectivityService.InternetConnectionAvailable
				where a
				select a into _
				select Observable.Interval(TimeSpan.FromSeconds(30.0)).StartWith(0L).TakeUntil(from a in ConnectivityService.InternetConnectionAvailable
					where !a
					select a)).Switch().TakeUntil(from s in Status
				where s == TimeSyncStatus.Synced
				select s);
			UniRx.IObservable<long> source = (from s in Status
				where s == TimeSyncStatus.NotSynced
				select s).SelectMany((TimeSyncStatus _) => notSyncedTriggers);
			ServerTimes = (from _ in source
				select CreateTimeObservable().CatchIgnore()).Switch().StartWith(DateTime.UtcNow.Ticks).Select(delegate(long st)
			{
				ServerTime result = default(ServerTime);
				result.utcTime = st;
				result.localOffset = st - DateTime.UtcNow.Ticks;
				return result;
			})
				.ToReadOnlyReactiveProperty();
			ServerTimes.Skip(1).Subscribe(delegate
			{
				Status.Value = TimeSyncStatus.Synced;
			});
			IsSynced = from s in Status
				select s == TimeSyncStatus.Synced;
			Syncs = from s in IsSynced
				where s
				select s into _
				select NowTicks();
			(from p in Observable.EveryApplicationPause()
				where !p
				select p).Subscribe(delegate
			{
				Status.Value = TimeSyncStatus.NotSynced;
			});
			Inited = true;
		}
	}

	public static void ResetDebug()
	{
	}

	public static void DebugAddSeconds(int secs)
	{
	}

	private static UniRx.IObservable<long> CreateTimeObservable()
	{
		return from www in ObservableWWW.GetWWW("https://fp-cloudsync.herokuapp.com/time?t=" + DateTime.UtcNow.Ticks).Timeout(TimeSpan.FromSeconds(5.0))
			select Convert.ToInt64(www.text);
	}

	public static long NowTicks()
	{
		float num = 0f;
		ServerTime value = ServerTimes.Value;
		return DateTime.UtcNow.Ticks + value.localOffset + (long)(1E+07f * num);
	}

	public static DateTime UtcNow()
	{
		float num = 0f;
		return DateTime.UtcNow + TimeSpan.FromSeconds(num);
	}
}
