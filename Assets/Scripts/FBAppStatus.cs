using System;
using UnityEngine;

public class FBAppStatus
{
	public static bool isFBAppInstalled()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]);
		AndroidJavaObject androidJavaObject2 = null;
		try
		{
			androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[1]
			{
				"com.facebook.katana"
			});
		}
		catch (Exception)
		{
			return false;
		}
		if (androidJavaObject2 == null)
		{
			return false;
		}
		return true;
	}
}
