using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class NotificationRunner : PersistentSingleton<NotificationRunner>
{
	private List<Message> BundleNotifications = new List<Message>();

	private List<Message> WelcomeBackNotifications = new List<Message>();

	public void InitializeNotifications()
	{
		if (!base.Inited)
		{
			(from p in Observable.EveryApplicationPause()
				where !p
				select p).Subscribe(delegate
			{
				CancelLocalNotifications();
			});
			(from p in Observable.EveryApplicationPause()
				where p
				select p).Subscribe(delegate
			{
				SetLocalNotifications();
			});
			CancelLocalNotifications();
			base.Inited = true;
			if (!PlayerData.Instance.NotificationDecision.Value)
			{
				PlayerData.Instance.NotificationDecision.Value = true;
			}
		}
	}

	private void CancelLocalNotifications()
	{
		PersistentSingleton<NotificationService>.Instance.CancelLocalNotifications();
	}

	private void SetLocalNotifications()
	{
		CancelLocalNotifications();
		List<Message> localNotifications = GenerateLocalNotifications();
		PersistentSingleton<NotificationService>.Instance.SetLocalNotifications(localNotifications);
	}

	public void RequestPermissionForNotifications()
	{
		PersistentSingleton<NotificationService>.Instance.RequestPermissionForNotifications();
	}

	private List<Message> GenerateLocalNotifications()
	{
		List<Message> list = new List<Message>();
		List<Message> bundleNotifications = BundleNotifications;
		foreach (Message item in bundleNotifications)
		{
			TimeSpan timeSpan = item.Delay - TimeSpan.FromTicks(ServerTimeService.NowTicks());
			if (timeSpan > TimeSpan.FromMinutes(10.0))
			{
				list.Add(new Message(timeSpan, item.Text));
			}
		}
		list.AddRange(WelcomeBackNotifications);
		List<Message> collection = CreateDrillNotifications();
		list.AddRange(collection);
		List<Message> collection2 = CreateFortunePodsNotification();
		list.AddRange(collection2);
		List<Message> collection3 = CreateTournamentNotifications();
		list.AddRange(collection3);
		List<Message> collection4 = CreateRetentionNotifications();
		list.AddRange(collection4);
		return list;
	}

	private List<Message> CreateRetentionNotifications()
	{
		List<Message> list = new List<Message>();
		List<string> list2 = new List<string>();
		for (int i = 1; i <= 20; i++)
		{
			list2.Add(PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification.Retention.Notif" + i));
		}
		int num = 0;
		for (int j = 0; j < 4; j++)
		{
			if (list2.Count > 0)
			{
				num = UnityEngine.Random.Range(0, list2.Count);
				int num2 = 1;
				switch (j)
				{
				case 0:
					num2 = 1;
					break;
				case 1:
					num2 = 3;
					break;
				case 2:
					num2 = 7;
					break;
				case 3:
					num2 = 30;
					break;
				}
				list.Add(new Message(TimeSpan.FromDays(num2), list2[num]));
				list2.Remove(list2[num]);
			}
		}
		return list;
	}

	private List<Message> CreateDrillNotifications()
	{
		List<Message> list = new List<Message>();
		List<string> list2 = new List<string>();
		TimeSpan delay = TimeSpan.FromSeconds(0.0);
		if (PlayerData.Instance.DrillTimeStamp.Value > 0)
		{
			delay = TimeSpan.FromTicks(PlayerData.Instance.DrillTimeStamp.Value - ServerTimeService.NowTicks());
		}
		for (int i = 1; i <= 4; i++)
		{
			list2.Add(PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification.Drill.Notif" + i));
		}
		if (list2.Count != 0 && delay.TotalSeconds > 1.0)
		{
			list.Add(new Message(delay, list2[UnityEngine.Random.Range(0, list2.Count)]));
		}
		return list;
	}

	public void CreateBundleNotification(bool deleteOld, TimeSpan notificationTimestamp, IAPProductEnum bundle)
	{
		if (deleteOld)
		{
			BundleNotifications.Clear();
		}
		List<string> list = new List<string>();
		for (int i = 1; i <= 2; i++)
		{
			list.Add(PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification." + bundle + ".Notif" + i));
		}
		if (list.Count > 0)
		{
			BundleNotifications.Add(new Message(notificationTimestamp, list[UnityEngine.Random.Range(0, list.Count)]));
		}
	}

	public void CreateWelcomeBackNotification(TimeSpan delay)
	{
		WelcomeBackNotifications.Clear();
		List<string> list = new List<string>();
		for (int i = 1; i <= 2; i++)
		{
			list.Add(PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification.DailyDouble.Notif" + i));
		}
		if (list.Count != 0)
		{
			WelcomeBackNotifications.Add(new Message(delay, list[UnityEngine.Random.Range(0, list.Count)]));
		}
	}

	private List<Message> CreateFortunePodsNotification()
	{
		List<Message> list = new List<Message>();
		List<string> list2 = new List<string>();
		TimeSpan delay = TimeSpan.FromSeconds(Mathf.Max((float)PersistentSingleton<GameSettings>.Instance.GamblingCooldown - (float)new TimeSpan(ServerTimeService.NowTicks() - PlayerData.Instance.GamblingTimeStamp.Value).TotalSeconds, 0f));
		for (int i = 1; i <= 3; i++)
		{
			list2.Add(PersistentSingleton<LocalizationService>.Instance.Text("TimeMachine.Notif." + i));
		}
		if (list2.Count != 0 && delay.TotalSeconds > 1.0)
		{
			list.Add(new Message(delay, list2[UnityEngine.Random.Range(0, list2.Count)]));
		}
		return list;
	}

	private List<Message> CreateTournamentNotifications()
	{
		List<Message> list = new List<Message>();
		if (Singleton<TournamentRunner>.Instance == null)
		{
			return list;
		}
		if (!Singleton<TournamentRunner>.Instance.CurrentlyInTournament.Value && Singleton<TournamentRunner>.Instance.TimeTillTournament.Value > 300)
		{
			list.Add(new Message(TimeSpan.FromSeconds(Singleton<TournamentRunner>.Instance.TimeTillTournament.Value), PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification.Tournaments.Starting")));
		}
		if (Singleton<TournamentRunner>.Instance.CurrentlyInTournament.Value)
		{
			if (Singleton<TournamentRunner>.Instance.TimeTillTournamentEnd.Value > 1200)
			{
				list.Add(new Message(TimeSpan.FromSeconds(Singleton<TournamentRunner>.Instance.TimeTillTournamentEnd.Value - 900), PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification.Tournaments.Ending")));
			}
			list.Add(new Message(TimeSpan.FromSeconds(Mathf.Max(Singleton<TournamentRunner>.Instance.TimeTillTournamentEnd.Value, 300f)), PersistentSingleton<LocalizationService>.Instance.Text("LocalNotification.Tournaments.Finished")));
		}
		return list;
	}
}
