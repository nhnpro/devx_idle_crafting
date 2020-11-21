/*
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;

public static class XPromoPlugin
{
	public static ReactiveProperty<XPromoAction> XPromoActions = Observable.Never<XPromoAction>().ToReactiveProperty();

	[CompilerGenerated]
	private static Func<XPromoConfig, bool> _003C_003Ef__mg_0024cache0;

	public static bool CheckForExternalApp(XPromoConfig cfg)
	{
		return GetLaunchIntentForPackage(cfg.PackageId) != null;
	}

	private static AndroidJavaObject GetLaunchIntentForPackage(string androidPackage)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]);
		AndroidJavaObject androidJavaObject2 = null;
		try
		{
			return androidJavaObject.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[1]
			{
				androidPackage
			});
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Exception hoover in XPromoPlugin " + ex.Message);
			return null;
		}
	}

	public static string InstalledApps()
	{
		return (from cfg in PersistentSingleton<Economies>.Instance.XPromo.Where(CheckForExternalApp)
			select cfg.ID).Aggregate(string.Empty, (string a, string b) => a + b + "_");
	}

	public static void OpenAppOnDevice(XPromoConfig cfg)
	{
		if (!CheckForExternalApp(cfg))
		{
			OpenAppPage(cfg);
			return;
		}
		XPromoActions.Value = new XPromoAction("OpenAppOnDevice", cfg.ID);
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject launchIntentForPackage = GetLaunchIntentForPackage(cfg.PackageId);
			if (launchIntentForPackage == null)
			{
				OpenAppPage(cfg);
			}
			else if (!string.IsNullOrEmpty(cfg.LaunchUrl))
			{
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.content.Intent");
				AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("android.net.Uri");
				AndroidJavaObject androidJavaObject = androidJavaClass3.CallStatic<AndroidJavaObject>("parse", new object[1]
				{
					cfg.LaunchUrl
				});
				AndroidJavaObject static2 = androidJavaClass2.GetStatic<AndroidJavaObject>("ACTION_VIEW");
				AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.content.Intent", static2, androidJavaObject);
				@static.Call("startActivity", androidJavaObject2);
			}
			else
			{
				@static.Call("startActivity", launchIntentForPackage);
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Exception hoover in XPromoPlugin.OpenAppOnDevice" + ex.Message);
			OpenAppPage(cfg);
		}
	}

	public static void OpenReview(XPromoConfig cfg)
	{
		XPromoActions.Value = new XPromoAction("OpenAppInStore", cfg.ID);
		FpOpenUrl(InternalStoreUrl(cfg));
	}

	public static void OpenAppPage(XPromoConfig cfg)
	{
		if (!string.IsNullOrEmpty(cfg.WebUrl))
		{
			FpOpenUrl(cfg.WebUrl);
		}
		else
		{
			OpenAttributedAppStore(cfg);
		}
	}

	private static void OpenAttributedAppStore(XPromoConfig cfg)
	{
		XPromoActions.Value = new XPromoAction("OpenAppInStoreAttributed", cfg.ID);
		FpOpenUrl(AttributedStoreUrl(cfg));
	}

	private static string InternalStoreUrl(XPromoConfig cfg)
	{
		return "market://details?id=" + cfg.PackageId;
	}

	private static string AttributedStoreUrl(XPromoConfig cfg)
	{
		return cfg.AppsFlyerUrl + Application.identifier;
	}

	public static void FpOpenUrl(string url)
	{
		Application.OpenURL(url);
	}

	public static void OpenURL(string url)
	{
		XPromoActions.Value = new XPromoAction("OpenURL", url);
		FpOpenUrl(url);
	}
}
*/
