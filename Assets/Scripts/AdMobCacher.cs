using System;
using System.Linq;
using UniRx;

public class AdMobCacher : SingleNetworkCacher
{
	public override void Init()
	{
		AdMobFlooredProvider adMobFlooredProvider = (AdMobFlooredProvider)base._flooredProviders.FirstOrDefault((FlooredAdProvider p) => p is AdMobFlooredProvider);
		if (adMobFlooredProvider != null)
		{
			bindAdMobLoading(adMobFlooredProvider);
		}
	}

	private void bindAdMobLoading(AdMobFlooredProvider adMob)
	{
		if (adMob._data.Count != 0)
		{
			UniRx.IObservable<bool> nobodyIsLoading = from l in adMob._provider.Loading
				where !l
				select l;
			UniRx.IObservable<bool> adsBecomesReady = from r in adMob.AdsReady
				where r
				select r;
			UniRx.IObservable<long> source = beginningsOfLoadingCycle(adMob.AdsReady);
			(from data in source.Select((Func<long, UniRx.IObservable<ZoneData>>)delegate
				{
					int index = 0;
					return from i in (from loading in nobodyIsLoading
							select ++index).Take(adMob._data.Count).TakeUntil(adsBecomesReady)
						select adMob._data[i];
				}).Switch()
				select data.Zone).Subscribe(delegate(string zone)
			{
				adMob.SetZone(zone);
				preload(adMob);
			}).AddTo(_disposable);
		}
	}
}
