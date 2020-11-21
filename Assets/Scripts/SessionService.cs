using System;
using UniRx;

public class SessionService : PersistentSingleton<SessionService>
{
	public const int SecsBetweenSessions = 600;

	public UniRx.IObservable<bool> newUser;

	public UniRx.IObservable<bool> newSession;

	public UniRx.IObservable<bool> appComesForeground;

	public void StartUp()
	{
		if (!base.Inited)
		{
			newSession = from _ in PlayerData.Instance.SessionNumber.Skip(1)
				select true;
			appComesForeground = from q in (from p in Observable.EveryApplicationPause()
					where !p
					select p).Merge(Observable.Return(value: false)).DelayFrame(1)
				select !q;
			newUser = from sessionNumber in PlayerData.Instance.SessionNumber.Skip(1)
				select sessionNumber == 0;
			(from _ in appComesForeground
				select new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.LastSaved).TotalSeconds into awaySecs
				where awaySecs >= 600.0 || PlayerData.Instance.SessionNumber.Value == -1
				select awaySecs).Subscribe(delegate
			{
				SessionWillStart();
			});
			base.Inited = true;
		}
	}

	private void SessionWillStart()
	{
		if (PlayerData.Instance.SessionNumber.Value == -1)
		{
			SendNewUserEvents();
		}
		PlayerData instance = PlayerData.Instance;
		long num = ServerTimeService.NowTicks();
		int num2 = (int)new TimeSpan(num - instance.InstallTime).TotalDays;
		int num3 = (int)new TimeSpan(num - instance.LastSaved).TotalDays;
		int hours = new TimeSpan(num - instance.LastSaved).Hours;
		int num4 = Math.Max(0, (int)new TimeSpan(instance.LastSaved - instance.LastSessionStart.Value).TotalMinutes);
		instance.MinutesInGame += num4;
		instance.LastSessionStart.Value = num;
		instance.SumOfPreviousSessionTimes.Value += Math.Max(0L, new TimeSpan(instance.LastSaved - instance.LastSessionStart.Value).Ticks);
		if (num2 > instance.DaysRetained.Value)
		{
			instance.DaysRetained.Value = num2;
			SendAppleWatchEventIfOwned();
		}
		instance.SessionNumber.Value++;
	}

	private void SendAppleWatchEventIfOwned()
	{
	}

	private void SendNewUserEvents()
	{
	}

	private string AttributionId()
	{
		string text = null;
		try
		{
			text = AttributionID.GetAdId();
		}
		catch (Exception)
		{
		}
		return (text == null) ? "not found" : text;
	}

	public long TicksInCurrentSession()
	{
		return new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.LastSessionStart.Value).Ticks;
	}

	public long TicksSinceInstall()
	{
		return new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.InstallTime).Ticks;
	}

	public long TicksSinceLastSave()
	{
		if (PlayerData.Instance.LastSaved > 0)
		{
			return new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.LastSaved).Ticks;
		}
		return 0L;
	}

	public long TicksPlayedInLifetime()
	{
		return PlayerData.Instance.SumOfPreviousSessionTimes.Value + TicksInCurrentSession();
	}
}
