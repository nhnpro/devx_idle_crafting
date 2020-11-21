using UniRx;

public interface AdProvider
{
	ReactiveProperty<bool> AdsReady
	{
		get;
	}

	AdNetwork NetworkId
	{
		get;
	}

	void Init();

	void TryToCache();

	IObservable<AdService.V2PShowResult> Show();
}
