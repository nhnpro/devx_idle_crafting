using UnityEngine;

public class UIFacebook : MonoBehaviour
{
	public void OnSyncLogin()
	{
		Singleton<FacebookRunner>.Instance.Login();
	}

	public void OnLogin()
	{
		Singleton<FacebookRunner>.Instance.Login();
	}

	public void OnLogout()
	{
		Singleton<FacebookRunner>.Instance.Logout();
	}

	public void OnAppRequest()
	{
		Singleton<FacebookRunner>.Instance.SendAppRequestFriendSelector(PersistentSingleton<LocalizationService>.Instance.Text("FacebookInvite.Send.Message"), PersistentSingleton<LocalizationService>.Instance.Text("FacebookInvite.Send.Title"));
	}
}
