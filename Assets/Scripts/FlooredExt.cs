public static class FlooredExt
{
	public static string ProviderInfo(this AdProvider provider)
	{
		AdProviderDelegate adProviderDelegate = provider as AdProviderDelegate;
		if (adProviderDelegate != null)
		{
			return adProviderDelegate.DelegateProviderInfo();
		}
		FlooredAdProvider flooredAdProvider = provider as FlooredAdProvider;
		if (flooredAdProvider != null)
		{
			return flooredAdProvider.FlooredProviderInfo();
		}
		return provider.NetworkId + "/DEFAULT";
	}

	public static string FlooredProviderInfo(this FlooredAdProvider provider)
	{
		return provider.NetworkId + "/" + provider.Zone.Value + ": " + provider.FloorValue.Value;
	}

	public static string DelegateProviderInfo(this AdProviderDelegate provider)
	{
		return provider.NetworkId + "/" + provider.Zone.Value + ": " + provider.FloorValue.Value + "/" + provider.MaxFloor;
	}
}
