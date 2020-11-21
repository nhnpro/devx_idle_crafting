using UnityEngine;

public class AndroidDeviceID
{
	public static string GetID()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
		return androidJavaClass2.CallStatic<string>("getString", new object[2]
		{
			androidJavaObject,
			"android_id"
		});
	}
}
