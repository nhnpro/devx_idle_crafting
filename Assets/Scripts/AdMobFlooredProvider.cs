using System.Collections.Generic;
using System.Linq;
using UniRx;

public class AdMobFlooredProvider : FlooredAdProvider, AdProvider
{
	public readonly AdMobZoneProvider _provider;

	private ReactiveProperty<ZoneData> _current;

	public readonly List<ZoneData> _data;

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

	public AdMobFlooredProvider(AdMobZoneProvider provider, List<ZoneData> data)
	{
		_provider = provider;
		NetworkId = _provider.NetworkId;
		_data = data;
		_current = new ReactiveProperty<ZoneData>(data.First());
		Zone = (from c in _current
			select c.Zone).ToReadOnlyReactiveProperty();
		FloorValue = (from r in AdsReady
			select r ? (from z in _current
				select _data.First((ZoneData d) => d == z).Floor) : Observable.Return(0)).Switch().ToReadOnlyReactiveProperty();
	}

	public void Init()
	{
		_provider.Init();
	}

	public void SetZone(string zone)
	{
		_current.Value = _data.First((ZoneData d) => d.Zone == zone);
	}

	public void TryToCache()
	{
		_provider.RequestRewardedAd(_current.Value.AdUnit);
	}

	IObservable<AdService.V2PShowResult> AdProvider.Show()
	{
		return _provider.Show();
	}
}
