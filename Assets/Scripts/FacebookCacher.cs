using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class FacebookCacher : SingleNetworkCacher
{
	public override void Init()
	{
		List<LoadingProvider> providers = (from p in base._flooredProviders
			where p.NetworkId == AdNetwork.Facebook
			select p as AdProviderDelegate into p
			where p != null
			select p._provider as LoadingProvider into p
			where p != null
			select p).ToList();
		bindFacebookLoading(providers);
	}

	private void bindFacebookLoading(List<LoadingProvider> providers)
	{
		if (providers.Count != 0)
		{
			UniRx.IObservable<bool> nobodyIsLoading = from l in (from p in (IEnumerable<LoadingProvider>)providers
					select p.Loading).CombineLatest(seed: false, (bool a, bool b) => a || b)
				where !l
				select l;
			UniRx.IObservable<bool> observable = (from p in (IEnumerable<LoadingProvider>)providers
				select p.AdsReady).CombineLatest(seed: false, (bool a, bool b) => a || b);
			UniRx.IObservable<bool> adsBecomesReady = from r in observable
				where r
				select r;
			UniRx.IObservable<long> source = beginningsOfLoadingCycle(observable);
			source.Select((Func<long, UniRx.IObservable<LoadingProvider>>)delegate
			{
				int index = 0;
				return from i in (from loading in nobodyIsLoading
						select ++index).Take(providers.Count).TakeUntil(adsBecomesReady)
					select providers[i];
			}).Switch().Subscribe(delegate(LoadingProvider p)
			{
				preload(p);
			})
				.AddTo(_disposable);
		}
	}
}
