using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UniRx;
using UnityEngine;

public class AdMobZoneProvider : LoadingProvider, AdProvider
{
    private IRewardBasedVideoAdClient m_rewardBasedVideo;

    private ReactiveProperty<bool> adReady = new ReactiveProperty<bool>(initialValue: false);

    private Subject<bool> m_adLoaded = new Subject<bool>();

    private Subject<bool> m_adOpened = new Subject<bool>();

    private Subject<bool> m_adFinished = new Subject<bool>();

    private Subject<bool> m_adFinishedMain = new Subject<bool>();

    public ReactiveProperty<bool> AdsReady => adReady;

    public AdNetwork NetworkId => AdNetwork.AdMob;

    public ReactiveProperty<bool> Loading { get; private set; }

    public AdMobZoneProvider()
    {
        Loading = new ReactiveProperty<bool>(initialValue: false);
    }

    public void Init()
    {
        // MobileAds.Initialize("ca-app-pub-7178055090700599~1990908832");
        MobileAds.Initialize(status => { Debug.Log($"Status Of Ads {status}"); });
        m_rewardBasedVideo = MobileAds.GetClientFactory().BuildRewardBasedVideoAdClient();
        m_rewardBasedVideo.CreateRewardBasedVideoAd();
        m_adLoaded.ObserveOnMainThread().Subscribe(delegate(bool l)
        {
            adReady.Value = l;
            Loading.Value = false;
        });
        m_adFinished.ObserveOnMainThread().Subscribe(delegate(bool finished) { m_adFinishedMain.OnNext(finished); });
        m_adOpened.ObserveOnMainThread().Subscribe(delegate { AudioListener.pause = true; });
        (from fin in m_adFinishedMain
            where !fin
            select fin).Subscribe(delegate
        {
            adReady.Value = false;
            AudioListener.pause = false;
        });
        m_rewardBasedVideo.OnAdLoaded += delegate { m_adLoaded.OnNext(value: true); };
        m_rewardBasedVideo.OnAdClosed += delegate { m_adFinished.OnNext(value: false); };
        m_rewardBasedVideo.OnAdFailedToLoad += delegate { m_adLoaded.OnNext(value: false); };
        m_rewardBasedVideo.OnAdRewarded += delegate { m_adFinished.OnNext(value: true); };
        m_rewardBasedVideo.OnAdOpening += delegate { m_adOpened.OnNext(value: true); };
    }

    public void TryToCache()
    {
        throw new Exception("Needs a separate caching logic. Use the RequestRewardedAd(adUnit).");
    }

    public void RequestRewardedAd(string adUnit)
    {
        AdRequest request = new AdRequest.Builder().Build();
        Loading.Value = true;
        m_rewardBasedVideo.LoadAd(request, adUnit);
    }

    public UniRx.IObservable<AdService.V2PShowResult> Show()
    {
        if (m_rewardBasedVideo.IsLoaded())
        {
            return Observable.Create(delegate(UniRx.IObserver<AdService.V2PShowResult> subscriber)
            {
                m_adFinishedMain.Take(1).Subscribe(delegate(bool finished)
                {
                    subscriber.OnNext((!finished) ? AdService.V2PShowResult.Skipped : AdService.V2PShowResult.Finished);
                    subscriber.OnCompleted();
                });
                m_rewardBasedVideo.ShowRewardBasedVideoAd();
                return new BooleanDisposable();
            });
        }

        adReady.Value = false;
        return Observable.Return(AdService.V2PShowResult.Failed);
    }
}