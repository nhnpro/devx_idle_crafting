public class AdColonyCacher : SingleNetworkCacher
{
	public override void Init()
	{
		bindSinglePreloader(PersistentSingleton<AdColonyProvider>.Instance);
	}
}
