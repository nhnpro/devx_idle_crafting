using UnityEngine;

public class AttributionID
{
	public static string GetAdId()
	{
		string empty = string.Empty;
		bool flag = false;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
		AndroidJavaObject androidJavaObject = androidJavaClass2.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", new object[1]
		{
			@static
		});
		empty = androidJavaObject.Call<string>("getId", new object[0]).ToString();
		flag = androidJavaObject.Call<bool>("isLimitAdTrackingEnabled", new object[0]);
		return empty;
	}
}
