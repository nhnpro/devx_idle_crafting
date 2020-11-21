using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

[PropertyClass]
public class AdRunner : Singleton<AdRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> AdReady;

	[PropertyBool]
	public ReactiveProperty<bool> ShowPending = new ReactiveProperty<bool>();

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AdFailed;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AdSkipped;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AdFinished;

	public ReadOnlyReactiveProperty<AdPlacement> AdPlacementFailed;

	public ReadOnlyReactiveProperty<AdPlacement> AdPlacementSkipped;

	public ReadOnlyReactiveProperty<AdPlacement> AdPlacementFinished;

	public AdRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		AdReady = PersistentSingleton<AdService>.Instance.AdReady;
		PersistentSingleton<AdService>.Instance.AdResults.Do(delegate
		{
		}).Subscribe(delegate
		{
			ShowPending.Value = false;
		}).AddTo(instance);
		AdFailed = (from res in PersistentSingleton<AdService>.Instance.AdResults
			where res.result == AdService.V2PShowResult.Failed
			select res into _
			select true).ToSequentialReadOnlyReactiveProperty();
		AdSkipped = (from res in PersistentSingleton<AdService>.Instance.AdResults
			where res.result == AdService.V2PShowResult.Skipped
			select res into _
			select true).ToSequentialReadOnlyReactiveProperty();
		AdFinished = (from res in PersistentSingleton<AdService>.Instance.AdResults
			where res.result == AdService.V2PShowResult.Finished
			select res into _
			select true).ToSequentialReadOnlyReactiveProperty();
		AdPlacementFailed = (from res in PersistentSingleton<AdService>.Instance.AdResults
			where res.result == AdService.V2PShowResult.Failed
			select res into ad
			select ad.placement).ToSequentialReadOnlyReactiveProperty();
		AdPlacementSkipped = (from res in PersistentSingleton<AdService>.Instance.AdResults
			where res.result == AdService.V2PShowResult.Skipped
			select res into ad
			select ad.placement).ToSequentialReadOnlyReactiveProperty();
		AdPlacementFinished = (from res in PersistentSingleton<AdService>.Instance.AdResults
			where res.result == AdService.V2PShowResult.Finished
			select res into ad
			select ad.placement).ToSequentialReadOnlyReactiveProperty();
		AdPlacementFinished.Subscribe(delegate(AdPlacement watched)
		{
			PlayerData.Instance.AdsInSession++;
			PlayerData.Instance.AdsInLifetime++;
			AdFrequencyConfig adFrequencyConfig = PersistentSingleton<Economies>.Instance.AdFrequencies.Find((AdFrequencyConfig s) => s.Placement == watched);
			if (adFrequencyConfig != null && (adFrequencyConfig.DailyCap < 100 || adFrequencyConfig.Cooldown > 0))
			{
				if (adFrequencyConfig.DailyCap >= 100)
				{
					for (int num = PlayerData.Instance.AdsWatched.Count - 1; num >= 0; num--)
					{
						if (PlayerData.Instance.AdsWatched[num].Placement == watched)
						{
							PlayerData.Instance.AdsWatched.RemoveAt(num);
						}
					}
				}
				PlayerData.Instance.AdsWatched.Add(new AdHistory(ServerTimeService.NowTicks(), watched));
			}
		}).AddTo(instance);
		PersistentSingleton<SessionService>.Instance.newSession.Subscribe(delegate
		{
			PlayerData.Instance.AdsInSession = 0;
		}).AddTo(instance);
	}

	public void ShowAd(AdPlacement ad)
	{
		ShowPending.Value = true;
		PersistentSingleton<AdService>.Instance.ShowAd(ad);
	}

	public TimeSpan GetNextTimeToShowAd(AdPlacement adPlacement)
	{
		List<AdHistory> adsWatched24H = PlayerData.Instance.GetAdsWatched24H();
		AdFrequencyConfig adFrequencyConfig = PersistentSingleton<Economies>.Instance.AdFrequencies.Find((AdFrequencyConfig s) => s.Placement == adPlacement);
		double num = (from adh in adsWatched24H
			where adh.Placement == adPlacement
			select adh into ad
			select TimeSpan.FromTicks(ServerTimeService.NowTicks() - ad.TimeStamp).TotalSeconds).LastOrDefault();
		if (num == 0.0)
		{
			return TimeSpan.Zero;
		}
		int num2 = adsWatched24H.Count((AdHistory s) => s.Placement == adPlacement);
		if (num2 < adFrequencyConfig.DailyCap)
		{
			return TimeSpan.FromSeconds((double)(adFrequencyConfig.Cooldown * 60) - num);
		}
		double num3 = (from adh in adsWatched24H.ToList()
			where adh.Placement == adPlacement
			select adh into ad
			select TimeSpan.FromTicks(ServerTimeService.NowTicks() - ad.TimeStamp).TotalMinutes).FirstOrDefault();
		return TimeSpan.FromMinutes(1440.0 - num3);
	}
}
