using UnityEngine;

public class UIFacebookRelink : MonoBehaviour
{
	public void OnLogout()
	{
		Singleton<FacebookRunner>.Instance.Logout();
	}

	public void OnForceRelink()
	{
	}
}
