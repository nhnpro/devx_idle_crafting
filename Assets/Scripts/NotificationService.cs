using notif;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LocalNotification = notif.LocalNotification;

public class NotificationService : PersistentSingleton<NotificationService>
{
	private const int MaxNotifications = 30;

	public void CancelLocalNotifications()
	{
		for (int i = 0; i < 30; i++)
		{
			long num = long.Parse(PlayerPrefs.GetString("NTS_" + i, "0"));
			if (num > DateTime.Now.Ticks)
			{
				LocalNotification.CancelNotification(i);
			}
			if (num > 0)
			{
				PlayerPrefs.DeleteKey("NTS_" + i);
			}
		}
		LocalNotification.ClearNotifications();
	}

	public void RequestPermissionForNotifications()
	{
		PlayerData.Instance.NotificationDecision.Value = true;
	}

	public void SetLocalNotifications(List<Message> messages)
	{
		if (PlayerData.Instance.NotificationDecision.Value)
		{
			foreach (var item in messages.Select((Message message, int i) => new
			{
				message,
				i
			}))
			{
				PlayerPrefs.SetString("NTS_" + item.i, (item.message.Delay.Ticks + DateTime.Now.Ticks).ToString());
				LocalNotification.SendNotification(item.i, item.message.Delay, "Idle Crafting Empire", item.message.Text, new Color32(0, 0, 0, byte.MaxValue), true, true, true, string.Empty, null, "default");
			}
		}
	}
}
