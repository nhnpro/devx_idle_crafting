using AdColony;
using UniRx;
using UnityEngine;

public class AdColonyProvider : PersistentSingleton<AdColonyProvider>, AdProvider
{
	private ReactiveProperty<InterstitialAd> myAd = new ReactiveProperty<InterstitialAd>();

	private BoolReactiveProperty Configured = new BoolReactiveProperty(initialValue: false);

	private Subject<bool> AdFinished = new Subject<bool>();

	public ReactiveProperty<bool> AdsReady
	{
		get;
		private set;
	}

	public AdNetwork NetworkId => AdNetwork.AdColony;

	public AdColonyProvider()
	{
		AdsReady = (from a in myAd
			select a != null).ToReactiveProperty();
	}

	private void ConfigureAds()
	{
		AppOptions appOptions = new AppOptions();
		appOptions.UserId = PlayerData.Instance.PlayerId;
		appOptions.AdOrientation = AdOrientationType.AdColonyOrientationPortrait;
		string[] zoneIds = new string[1]
		{
			"vzc2600c4251094143b0"
		};
		Ads.Configure("appfe9ad9a3c19e49f38f", appOptions, zoneIds);
	}

	public void Init()
	{
	}

	public void LateInitialize()
	{
		Ads.OnConfigurationCompleted += delegate
		{
			Configured.Value = true;
		};
		Ads.OnRequestInterstitial += delegate(InterstitialAd ad_)
		{
			myAd.Value = ad_;
		};
		Ads.OnRequestInterstitialFailed += delegate
		{
			myAd.Value = null;
		};
		Ads.OnOpened += delegate
		{
			Time.timeScale = 0f;
			AudioListener.pause = true;
			myAd.Value = null;
		};
		Ads.OnClosed += delegate
		{
			Time.timeScale = 1f;
			AudioListener.pause = false;
			AdFinished.OnNext(value: true);
			myAd.Value = null;
		};
		Ads.OnExpiring += delegate
		{
			myAd.Value = null;
		};
		ConfigureAds();
	}

	public void TryToCache()
	{
		(from c in Configured
			where c
			select c).Take(1).Subscribe(delegate
		{
			RequestInterstitial();
		}).AddTo(SceneLoader.Instance);
	}

	private void RequestInterstitial()
	{
		AdOptions adOptions = new AdOptions();
		Ads.RequestInterstitialAd("vzc2600c4251094143b0", adOptions);
	}

	public UniRx.IObservable<AdService.V2PShowResult> Show()
	{
		if (myAd.Value != null && !myAd.Value.Expired)
		{
			return Observable.Create(delegate(UniRx.IObserver<AdService.V2PShowResult> subscriber)
			{
				AdFinished.Take(1).Subscribe(delegate
				{
					subscriber.OnNext(AdService.V2PShowResult.Finished);
					subscriber.OnCompleted();
				});
				Ads.ShowAd(myAd.Value);
				myAd.Value = null;
				return new BooleanDisposable();
			});
		}
		myAd.Value = null;
		return Observable.Return(AdService.V2PShowResult.Failed);
	}
}
