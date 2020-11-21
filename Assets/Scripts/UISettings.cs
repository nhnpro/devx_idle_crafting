using UnityEngine;

public class UISettings : MonoBehaviour
{
	public void OnShowAchievements()
	{
		PersistentSingleton<AchievementsService>.Instance.ShowAchievementsUI();
	}

	public void OnChangeQuality()
	{
		Singleton<QualitySettingsRunner>.Instance.ChangeQuality();
	}

	public void OnChangeFPS()
	{
		Singleton<QualitySettingsRunner>.Instance.ChangeFPS();
	}

	public void OnShowFAQ()
	{
		Singleton<ReviewAppRunner>.Instance.GotoFAQPage();
	}

	public void OnShowPrivacyPolicy()
	{
		Singleton<ReviewAppRunner>.Instance.GoToPrivacyPolicyPage();
	}

	public void OnFacebook()
	{
		if (!Singleton<FacebookRunner>.Instance.IsLoggedToFacebook.Value)
		{
			Singleton<FacebookRunner>.Instance.Login();
		}
		else
		{
			Singleton<FacebookRunner>.Instance.Logout();
		}
	}
}
