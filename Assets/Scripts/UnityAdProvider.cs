using UniRx;
using UnityEngine.Advertisements;

public class UnityAdProvider : AdProvider
{
	private ReactiveProperty<bool> AdReady = new ReactiveProperty<bool>(initialValue: false);

	public AdNetwork NetworkId => AdNetwork.Unity;

	public ReactiveProperty<bool> AdsReady => AdReady;

	public void Init()
	{
		(from _ in TickerService.MasterTicksSlow
			select Advertisement.IsReady()).DistinctUntilChanged().Subscribe(delegate(bool ready)
		{
			AdReady.Value = ready;
		});
	}

	public UniRx.IObservable<AdService.V2PShowResult> Show()
	{
		if (Advertisement.IsReady())
		{
			return Observable.Create(delegate(UniRx.IObserver<AdService.V2PShowResult> subscriber)
			{
				Advertisement.Show(null, new ShowOptions
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
