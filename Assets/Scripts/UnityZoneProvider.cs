using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine.Advertisements;

public class UnityZoneProvider : FlooredAdProvider, AdProvider
{
	private List<ZoneData> _data;

	private ReactiveProperty<ZoneData> _current;

	public AdNetwork NetworkId => AdNetwork.Unity;

	public ReactiveProperty<bool> AdsReady
	{
		get;
		private set;
	}

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

	public UnityZoneProvider(List<ZoneData> data)
	{
		_data = data;
		_current = new ReactiveProperty<ZoneData>((ZoneData)null);
		AdsReady = (from d in _current
			select d != null).ToReactiveProperty();
		Zone = (from c in _current
			select (c != null) ? c.Zone : "N/A").ToReadOnlyReactiveProperty();
		FloorValue = (from c in _current
			select c?.Floor ?? 0).ToReadOnlyReactiveProperty();
	}

	public void Init()
	{
		(from _ in TickerService.MasterTicksSlow
			select _data.FirstOrDefault((ZoneData d) => Advertisement.IsReady(d.AdUnit))).DistinctUntilChanged().Subscribe(delegate(ZoneData zoneData)
		{
			_current.Value = zoneData;
		});
	}

	public UniRx.IObservable<AdService.V2PShowResult> Show()
	{
		string placementId = _current.Value.AdUnit;
		if (Advertisement.IsReady(placementId))
		{
			return Observable.Create(delegate(UniRx.IObserver<AdService.V2PShowResult> subscriber)
			{
				Advertisement.Show(placementId, new ShowOptions
				{
					resultCallback = delegate(ShowResult result)
					{
						switch (result)
						{
						case ShowResult.Finished:
							subscriber.OnNext(AdService.V2PShowResult.Finished);
							break;
						case ShowResult.Failed:
							subscriber.OnNext(AdService.V2PShowResult.Failed);
							break;
						case ShowResult.Skipped:
							subscriber.OnNext(AdService.V2PShowResult.Skipped);
							break;
						default:
							subscriber.OnNext(AdService.V2PShowResult.Failed);
							break;
						}
						subscriber.OnCompleted();
					}
				});
				return new BooleanDisposable();
			});
		}
		return Observable.Return(AdService.V2PShowResult.Failed);
	}

	public void TryToCache()
	{
	}
}
