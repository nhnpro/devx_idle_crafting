using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class AdService : PersistentSingleton<AdService>, IDisposable
{
	public enum V2PShowResult
	{
		Failed,
		Skipped,
		Finished,
		None
	}

	private List<FlooredAdProvider> _adSources;

	private CompositeDisposable _disposable;

	public ReadOnlyReactiveProperty<FlooredAdProvider> CurrentProvider;

	private static readonly Random random = new Random();

	public ReactiveProperty<bool> AdReady
	{
		get;
		private set;
	}

	public ReactiveProperty<AdWatched> AdStarted
	{
		get;
		private set;
	}

	public ReactiveProperty<AdWatched> AdResults
	{
		get;
		private set;
	}

	public UniRx.IObservable<AdWatched> AdsWatched
	{
		get;
		private set;
	}

	public UniRx.IObservable<AdWatched> AdsFailed
	{
		get;
		private set;
	}

	public UniRx.IObservable<AdLoadRequest> AdLoadRequests
	{
		get;
		private set;
	}

	public AdService()
	{
		AdStarted = Observable.Never<AdWatched>().ToReactiveProperty();
		AdResults = new ReactiveProperty<AdWatched>(new AdWatched
		{
			result = V2PShowResult.None,
			network = AdNetwork.None,
			placement = AdPlacement.None
		});
		AdsWatched = from r in AdResults
			where r.result == V2PShowResult.Finished
			select r;
		AdsFailed = from r in AdResults
			where r.result == V2PShowResult.Failed || r.result == V2PShowResult.Skipped
			select r;
		AdLoadRequests = Observable.Never<AdLoadRequest>();
	}

	public void Init()
	{
		if (!base.Inited)
		{
			_disposable = new CompositeDisposable();
			_adSources = PersistentSingleton<FlooredProvidersFactory>.Instance.FlooredAdProviders;
			CurrentProvider = (from _ in (from p in (IEnumerable<FlooredAdProvider>)_adSources
					select p.FloorValue).Merge()
				select chooseSource()).ToReadOnlyReactiveProperty();
			bindAdReady();
			AdStarted.Subscribe(delegate(AdWatched s)
			{
				PersistentSingleton<MainSaver>.Instance.PleaseSave("ads_" + s.network + "_" + s.placement);
			}).AddTo(_disposable);
			AdResults.Subscribe(delegate(AdWatched s)
			{
				PersistentSingleton<MainSaver>.Instance.PleaseSave("ads_" + s.network + "_" + s.placement + "_result;" + s.result);
			}).AddTo(_disposable);
			base.Inited = true;
		}
	}

	public void Dispose()
	{
		_disposable.Dispose();
	}

	private void bindAdReady()
	{
		this.debugTMP("Ad sources: " + _adSources.Count);
		AdReady = (from p in (IEnumerable<FlooredAdProvider>)_adSources
			select p.AdsReady).CombineLatest(seed: false, (bool p1Ready, bool p2Ready) => p1Ready || p2Ready).ToReactiveProperty();
	}

	public void ShowAd(AdPlacement placement)
	{
		TryToShow(placement).ObserveOnMainThread().Subscribe(delegate(AdWatched result)
		{
			AdResults.Value = result;
		}).AddTo(_disposable);
	}

	public UniRx.IObservable<AdWatched> TryToShow(AdPlacement placement)
	{
		return TryToShow(placement, CurrentProvider.Value);
	}

	public UniRx.IObservable<AdWatched> TryToShow(AdPlacement placement, FlooredAdProvider provider)
	{
		if (provider == null || !provider.AdsReady.Value)
		{
			AdWatched adWatched = new AdWatched();
			adWatched.network = AdNetwork.None;
			adWatched.placement = placement;
			adWatched.result = V2PShowResult.Failed;
			return Observable.Return(adWatched);
		}
		string zone = provider.Zone.Value;
		int floor = provider.FloorValue.Value;
		AdStarted.Value = new AdWatched
		{
			network = provider.NetworkId,
			result = V2PShowResult.None,
			placement = placement,
			zone = zone,
			floorValue = floor
		};
		return from result in provider.Show()
			select new AdWatched
			{
				result = result,
				network = provider.NetworkId,
				placement = placement,
				zone = zone,
				floorValue = floor
			};
	}

	private FlooredAdProvider chooseSource()
	{
		int maxVal = _adSources.Max((FlooredAdProvider s) => s.FloorValue.Value);
		FlooredAdProvider[] array = (from s in _adSources
			where s.FloorValue.Value == maxVal
			select s).ToArray();
		if (array.Length == 0)
		{
			return null;
		}
		int num = (int)(random.NextDouble() * (double)array.Length);
		return array[num];
	}
}
