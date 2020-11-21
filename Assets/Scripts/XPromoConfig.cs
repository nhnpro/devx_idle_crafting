public class XPromoConfig
{
	public string ID
	{
		get;
		private set;
	}

	public string AppName
	{
		get;
		private set;
	}

	public string iOSAppId
	{
		get;
		private set;
	}

	public string PackageId
	{
		get;
		private set;
	}

	public string LaunchUrl
	{
		get;
		private set;
	}

	public string WebUrl
	{
		get;
		private set;
	}

	public string AppsFlyerUrl
	{
		get;
		private set;
	}

	public XPromoConfig(string id, string appName, string iosApp, string pkgId, string launchUrl, string webUrl, string appsFlyerUrl)
	{
		ID = id;
		AppName = appName;
		iOSAppId = iosApp;
		PackageId = pkgId;
		LaunchUrl = launchUrl;
		WebUrl = webUrl;
		AppsFlyerUrl = appsFlyerUrl;
	}
}
