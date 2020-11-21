using System;
using UniRx;

public abstract class AbstractEmptyAdProvider : AdProvider
{
	public float minWait;

	public float maxWait = 20f;

	private ReactiveProperty<bool> adReady = new ReactiveProperty<bool>(initialValue: false);

	private static readonly Random random = new Random();

	public ReactiveProperty<bool> AdsReady => adReady;

	public abstract AdNetwork NetworkId
	{
		get;
	}

	public UniRx.IObservable<AdService.V2PShowResult> Show()
	{
		adReady.Value = false;
		return Observable.Return(AdService.V2PShowResult.Finished);
	}

	public void Init()
	{
		adReady.Skip(1).Subscribe(delegate(bool r)
		{
			this.debug("Ad Ready: " + r + " (" + NetworkId + ")");
		});
	}

	public void TryToCache()
	{
		double num = (double)minWait + random.NextDouble() * (double)(maxWait - minWait);
		this.debug("Trying to cache ads for " + NetworkId + ". Will succeed doing so after: " + num + "s");
		Observable.Timer(TimeSpan.FromSeconds(num)).Subscribe(delegate
		{
			adReady.Value = true;
		});
	}
}
