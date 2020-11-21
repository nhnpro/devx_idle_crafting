using AudienceNetwork;
using System;
using UniRx;
using UnityEngine;

public class FacebookProvider : LoadingProvider, IDisposable, AdProvider
{
	private ReactiveProperty<bool> adReady = new ReactiveProperty<bool>(initialValue: false);

	private Subject<bool> adFinished = new Subject<bool>();

	private RewardedVideoAd ad;

	private GameObject listener;

	public readonly string Zone;

	private CompositeDisposable _disposable = new CompositeDisposable();

	private Subject<bool> _loadingEvents = new Subject<bool>();

	public ReactiveProperty<bool> Loading
	{
		get;
		private set;
	}

	public ReactiveProperty<bool> AdsReady => adReady;

	public AdNetwork NetworkId => AdNetwork.Facebook;

	public FacebookProvider(string zone)
	{
		Zone = zone;
		Loading = new ReactiveProperty<bool>(initialValue: false);
		_loadingEvents.ObserveOnMainThread().Subscribe(delegate(bool success)
		{
			adReady.Value = success;
			Loading.Value = false;
		}).AddTo(_disposable);
	}

	public void Init()
	{
		listener = new GameObject("FacebookAudienceNetworkListener");
		UnityEngine.Object.DontDestroyOnLoad(listener);
	}

	public void Dispose()
	{
		_disposable.Dispose();
	}

	public void TryToCache()
	{
		RequestNewAd();
	}

	private void RequestNewAd()
	{
		if (ad != null)
		{
			ad.Dispose();
		}
		ad = new RewardedVideoAd(Zone);
		ad.Register(listener);
		ad.RewardedVideoAdDidLoad = delegate
		{
			_loadingEvents.OnNext(value: true);
		};
		ad.RewardedVideoAdDidFailWithError = delegate
		{
			_loadingEvents.OnNext(value: false);
		};
		Loading.Value = true;
		adReady.Value = false;
		ad.LoadAd();
	}

	public UniRx.IObservable<AdService.V2PShowResult> Show()
	{
		if (adReady.Value)
		{
			return Observable.Create(delegate(UniRx.IObserver<AdService.V2PShowResult> subscriber)
			{
				FacebookProvider facebookProvider = this;
				CompositeDisposable compositeDisposable = new CompositeDisposable();
				adFinished.ObserveOnMainThread().Take(1).Subscribe(delegate(bool success)
				{
					subscriber.OnNext(success ? AdService.V2PShowResult.Finished : AdService.V2PShowResult.Failed);
					subscriber.OnCompleted();
				})
					.AddTo(compositeDisposable);
				RewardedVideoAd rewardedVideoAd = ad;
				rewardedVideoAd.RewardedVideoAdDidFail = delegate
				{
					facebookProvider.adFinished.OnNext(value: false);
				};
				rewardedVideoAd.RewardedVideoAdComplete = delegate
				{
					facebookProvider.adFinished.OnNext(value: true);
				};
				rewardedVideoAd.RewardedVideoAdDidClose = delegate
				{
					facebookProvider.adFinished.OnNext(value: false);
				};
				rewardedVideoAd.rewardedVideoAdActivityDestroyed = delegate
				{
					facebookProvider.adFinished.OnNext(value: false);
				};
				ad = null;
				adReady.Value = false;
				if (!rewardedVideoAd.Show())
				{
					adFinished.OnNext(value: false);
				}
				return compositeDisposable;
			});
		}
		return Observable.Return(AdService.V2PShowResult.Failed);
	}
}
