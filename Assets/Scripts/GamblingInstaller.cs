public static class GamblingInstaller
{
	public static void DoAwake()
	{
		Singleton<PropertyManager>.Construct();
		Singleton<PropertyManager>.Instance.AddRootContext(PlayerData.Instance);
		GamblingBindingManager.Construct();
		Singleton<EconomyHelpers>.Construct();
		Singleton<IAPRunner>.Construct();
		Singleton<AdNetworkCachers>.Construct();
		Singleton<IAPItemCollectionRunner>.Construct();
		Singleton<FundRunner>.Construct();
		Singleton<EnableObjectsRunner>.Construct();
		Singleton<NotEnoughGemsRunner>.Construct();
		Singleton<GamblingRunner>.Construct();
	}

	public static void DoStart()
	{
		Singleton<PropertyManager>.Instance.InstallScene();
		Singleton<IAPRunner>.Instance.PostInit();
	}

	public static void ReleaseAll()
	{
		SingletonManager.ReleaseAll();
		GamblingBindingManager.Release();
	}
}
