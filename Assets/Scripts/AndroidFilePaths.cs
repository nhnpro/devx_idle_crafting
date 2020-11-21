using System;
using UnityEngine;

public static class AndroidFilePaths
{
	private static string androidExternalPath;

	private static string androidInternalPath;

	public static string GetExternalSavePath()
	{
		if (androidExternalPath == null)
		{
			androidExternalPath = Application.persistentDataPath;
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getApplicationContext", new object[0]))
						{
							using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getExternalFilesDir", new object[1]
							{
								string.Empty
							}))
							{
								androidExternalPath = androidJavaObject3.Call<string>("getAbsolutePath", new object[0]);
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
		return androidExternalPath;
	}

	public static string GetInternalSavePath()
	{
		if (androidInternalPath == null)
		{
			androidInternalPath = Application.persistentDataPath;
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getApplicationContext", new object[0]))
						{
							using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getFilesDir", new object[0]))
							{
								androidInternalPath = androidJavaObject3.Call<string>("getAbsolutePath", new object[0]);
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
		return androidInternalPath;
	}
}
