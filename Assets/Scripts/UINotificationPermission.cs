using UnityEngine;

public class UINotificationPermission : MonoBehaviour
{
	public void OnRequestForPermission()
	{
		PersistentSingleton<NotificationRunner>.Instance.RequestPermissionForNotifications();
	}
}
