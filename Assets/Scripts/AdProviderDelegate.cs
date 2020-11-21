using UniRx;

public class AdProviderDelegate : FlooredAdProvider, AdProvider
{
	public readonly AdProvider _provider;

	public readonly int MaxFloor;

	public AdNetwork NetworkId
	{
		get;
		private set;
	}

	public ReactiveProperty<bool> AdsReady => _provider.AdsReady;

	public ReadOnlyReactiveProperty<int> FloorValue
	{
		get;
		private set;
	}

	public ReadOnlyReactiveProperty<string> Zone
	{
		get;
		private set;
	}

	public AdProviderDelegate(AdProvider provider, string zone, int floor)
	{
		_provider = provider;
		NetworkId = _provider.NetworkId;
		Zone = Observable.Return(zone).ToReadOnlyReactiveProperty();
		FloorValue = (from r in AdsReady
			select r ? floor : 0).ToReadOnlyReactiveProperty();
		MaxFloor = floor;
	}

	public void Init()
	{
		_provider.Init();
	}

	public void TryToCache()
	{
		_provider.TryToCache();
	}

	IObservable<AdService.V2PShowResult> AdProvider.Show()
	{
		return _provider.Show();
	}
}
