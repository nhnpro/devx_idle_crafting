using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AchievementsService : PersistentSingleton<AchievementsService>
{
	public void LateInitialize()
	{
		if (!base.Inited)
		{
			base.Inited = true;
		}
	}

	private bool AndroidShouldAuth()
	{
		return PlayerData.Instance.GPGSPermission || !PlayerData.Instance.GPGSPermissionAsked;
	}

	private void CheckAndroidAuth()
	{
		if (PlayerData.Instance.GPGSPermissionAsked)
		{
			PlayerData.Instance.GPGSPermission = Social.localUser.authenticated;
		}
	}

	private void ProcessAuthentication(bool success)
	{
		PlayerData.Instance.GPGSPermissionAsked = true;
		if (success)
		{
			PlayerData.Instance.GPGSPermission = true;
			Social.LoadAchievements(ProcessLoadedAchievements);
		}
		else
		{
			PlayerData.Instance.GPGSPermission = false;
		}
	}

	private void ProcessLoadedAchievements(IAchievement[] achievements)
	{
		if (achievements.Length != 0)
		{
		}
	}

	public void ReportProgress(string id, float progress)
	{
	}

	private void ProcessReportProgress(bool success)
	{
	}

	public void ResetAchievements()
	{
	}

	public void ShowAchievementsUI()
	{
	}
}
