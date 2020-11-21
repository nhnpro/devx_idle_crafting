using Big;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[PropertyClass]
public class IAPBundleRunner : Singleton<IAPBundleRunner>
{
	private int[,] BundleTiersToGearSets = new int[3, 4]
	{
		{
			0,
			1,
			2,
			-1
		},
		{
			3,
			4,
			-1,
			-1
		},
		{
			5,
			6,
			7,
			-1
		}
	};

	public ReactiveProperty<IAPProductEnum> ProductID = new ReactiveProperty<IAPProductEnum>();

	private IAPConfig m_iapConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == IAPProductEnum.bundle1);

	[PropertyBool]
	public ReactiveProperty<bool> BundleActive;

	[PropertyBool]
	public ReactiveProperty<bool> BundleAvailable;

	[PropertyString]
	public ReactiveProperty<string> PriceString;

	[PropertyString]
	public ReactiveProperty<string> SaleString;

	[PropertyString]
	public ReactiveProperty<string> Title = new ReactiveProperty<string>();

	[PropertyString]
	public ReactiveProperty<string> MainDescription = new ReactiveProperty<string>();

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> MainRewardAmount = new ReactiveProperty<BigDouble>();

	public ReactiveProperty<RewardData> MainReward = new ReactiveProperty<RewardData>();

	public ReactiveProperty<RewardData> Extra1Reward = new ReactiveProperty<RewardData>();

	public ReactiveProperty<RewardData> Extra2Reward = new ReactiveProperty<RewardData>();

	[PropertyString]
	public ReadOnlyReactiveProperty<string> DurationLeft;

	private ReactiveProperty<int> SecondsLeft;

	private ReactiveProperty<int> DeactiveLeft;

	private ReactiveProperty<int> TransitionLeft;

	private ReactiveProperty<bool> cycleOn = Observable.Never<bool>().ToReactiveProperty();

	[PropertyBool]
	public ReactiveProperty<bool> ShowOfferPopup = new ReactiveProperty<bool>(initialValue: false);

	public IAPBundleRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		UniRx.IObservable<double> source = (from comb in TickerService.MasterTicksSlow.CombineLatest(cycleOn, (long tick, bool cycle) => new
			{
				tick,
				cycle
			})
			where comb.cycle
			select comb into tick
			select TimeSpan.FromTicks(ServerTimeService.NowTicks() - PlayerData.Instance.BundleTimeStamp.Value).TotalSeconds).TakeUntilDestroy(instance);
		SecondsLeft = (from timeElapsed in source
			select PersistentSingleton<GameSettings>.Instance.BundleActiveTime - (int)timeElapsed into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		(from secs in SecondsLeft
			where secs > PersistentSingleton<GameSettings>.Instance.BundleActiveTime
			select secs).Subscribe(delegate
		{
			PlayerData.Instance.BundleTimeStamp.Value = ServerTimeService.NowTicks();
		}).AddTo(instance);
		BundleActive = (from secs in SecondsLeft
			select secs > 0).TakeUntilDestroy(instance).ToReactiveProperty();
		BundleAvailable = (from synced in ServerTimeService.IsSynced
			select (synced)).TakeUntilDestroy(instance).ToReactiveProperty();
		DurationLeft = (from seconds in SecondsLeft
			select TextUtils.FormatSecondsShort(seconds)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DeactiveLeft = (from timeElapsed in source
			select PersistentSingleton<GameSettings>.Instance.BundleActiveTime + PersistentSingleton<GameSettings>.Instance.BundleDeactiveTime - (int)timeElapsed into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		(from secs in DeactiveLeft
			where secs > PersistentSingleton<GameSettings>.Instance.BundleDeactiveTime + PersistentSingleton<GameSettings>.Instance.BundleActiveTime
			select secs).Subscribe(delegate
		{
			PlayerData.Instance.BundleTimeStamp.Value = ServerTimeService.NowTicks() - TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.BundleActiveTime).Ticks;
		}).AddTo(instance);
		(from timeLeft in DeactiveLeft
			where timeLeft == 0
			select timeLeft).Subscribe(delegate
		{
			SetNewBundle();
		}).AddTo(instance);
		TransitionLeft = (from comb in TickerService.MasterTicksSlow.CombineLatest(cycleOn, (long tick, bool cycle) => new
			{
				tick,
				cycle
			})
			where !comb.cycle
			select comb into tick
			select TimeSpan.FromTicks(ServerTimeService.NowTicks() - PlayerData.Instance.BundleTimeStamp.Value).TotalSeconds into timeElapsed
			select PersistentSingleton<GameSettings>.Instance.BundleTransitionTime - (int)timeElapsed into secsLeft
			select (int)Mathf.Max(0f, secsLeft)).DistinctUntilChanged().TakeUntilDestroy(instance).ToReactiveProperty();
		(from secs in TransitionLeft
			where secs > PersistentSingleton<GameSettings>.Instance.BundleTransitionTime
			select secs).Subscribe(delegate
		{
			PlayerData.Instance.BundleTimeStamp.Value = ServerTimeService.NowTicks();
		}).AddTo(instance);
		(from timeLeft in TransitionLeft
			where timeLeft == 0
			select timeLeft).Subscribe(delegate
		{
			SetNewBundle();
		}).AddTo(instance);
		PriceString = (from fetched in PersistentSingleton<IAPService>.Instance.DataFetched
			select (!fetched) ? PersistentSingleton<LocalizationService>.Instance.Text("IAPItem.DefaultButtonText") : m_iapConfig.GetLocalizedPriceStringOrDefault()).TakeUntilDestroy(instance).ToReactiveProperty();
		SaleString = (from fetched in PersistentSingleton<IAPService>.Instance.DataFetched
			select (!fetched) ? string.Empty : PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == m_iapConfig.SaleEnum).GetLocalizedPriceStringOrEmpty()).TakeUntilDestroy(instance).ToReactiveProperty();
		(from active in BundleActive
			where !active
			select active).Subscribe(delegate
		{
			ShowOfferPopup.SetValueAndForceNotify(value: false);
		}).AddTo(instance);
		(from avail in (from sync in (from active in BundleActive.Pairwise()
					where active.Current && !active.Previous
					select active into _
					select ServerTimeService.IsSynced).Switch()
				where sync
				select sync into _
				select Singleton<WelcomeBackRunner>.Instance.WelcomeBackAvailable.AsObservable()).Switch().Take(1)
			where !avail
			select avail).Subscribe(delegate
		{
			ShowOfferPopup.SetValueAndForceNotify(value: true);
		}).AddTo(instance);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from chunk in (from prestiges in PlayerData.Instance.LifetimePrestiges
				select (prestiges != 0) ? Observable.Never<int>() : PlayerData.Instance.MainChunk.AsObservable()).Switch().Pairwise()
			where chunk.Current == 17 && chunk.Previous == 16
			select chunk).Subscribe(delegate
		{
			SetNewBundle();
			ShowOfferPopup.SetValueAndForceNotify(value: true);
		}).AddTo(instance);
		(from current in PlayerData.Instance.CurrentBundleEnum
			where current >= -1
			select current).Subscribe(delegate(int num)
		{
			InitializeBundleRunner(num);
		}).AddTo(instance);
		(from iap in Singleton<IAPRunner>.Instance.IAPCompleted
			where iap.Config.Type == IAPType.BundleConsumable || iap.Config.Type == IAPType.BundleDurable
			select iap).Subscribe(delegate
		{
			PlayerData.Instance.LifetimeBundles.Value++;
			PlayerData.Instance.BundleTimeStamp.Value = ServerTimeService.NowTicks();
			PlayerData.Instance.CurrentBundleEnum.Value = -1;
			PlayerData.Instance.CurrentBundleGearIndex.Value = -1;
			SecondsLeft.Value = 0;
		}).AddTo(instance);
	}

	private void SetNewBundle()
	{
		List<BundleTierConfig> bundles = PersistentSingleton<Economies>.Instance.BundleTiers;
		int value = PlayerData.Instance.LifetimeBundles.Value;
		if (value == 0 || PlayerData.Instance.LifetimePrestiges.Value == 0)
		{
			bundles = PersistentSingleton<Economies>.Instance.BundleTiers.FindAll((BundleTierConfig s) => s.BundleTier == 1);
		}
		else
		{
			switch (value)
			{
			case 1:
				bundles = PersistentSingleton<Economies>.Instance.BundleTiers.FindAll((BundleTierConfig s) => s.BundleTier == 2);
				break;
			case 2:
				bundles = PersistentSingleton<Economies>.Instance.BundleTiers.FindAll((BundleTierConfig s) => s.BundleTier == 3);
				break;
			}
		}
		int currentBundle = (int)bundles[UnityEngine.Random.Range(0, bundles.Count)].ProductEnum;
		if (PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == (IAPProductEnum)currentBundle).Reward.Type == RewardEnum.AddToGearLevel)
		{
			BundleTierConfig bundleTierConfig = PersistentSingleton<Economies>.Instance.BundleTiers.Find((BundleTierConfig s) => s.ProductEnum == m_iapConfig.ProductEnum);
			List<GearSetRunner> list = new List<GearSetRunner>();
			for (int j = 0; j < BundleTiersToGearSets.GetLength(1); j++)
			{
				if (BundleTiersToGearSets[bundleTierConfig.BundleTier - 1, j] != -1)
				{
					GearSetRunner orCreateGearSetRunner = Singleton<GearSetCollectionRunner>.Instance.GetOrCreateGearSetRunner(BundleTiersToGearSets[bundleTierConfig.BundleTier - 1, j]);
					if (orCreateGearSetRunner != null)
					{
						list.Add(orCreateGearSetRunner);
					}
				}
			}
			GearRunner gearRunner = Singleton<GearSetCollectionRunner>.Instance.GetRandomUnlockableGear(list);
			if (gearRunner == null)
			{
				gearRunner = Singleton<GearSetCollectionRunner>.Instance.GetRandomHighestGear(list);
			}
			if (gearRunner != null)
			{
				PlayerData.Instance.CurrentBundleGearIndex.Value = gearRunner.GearIndex;
			}
			else
			{
				int i;
				for (i = bundles.Count - 1; i >= 0; i--)
				{
					IAPConfig iAPConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == bundles[i].ProductEnum);
					if (iAPConfig.Reward.Type == RewardEnum.AddToGearLevel)
					{
						bundles.Remove(bundles[i]);
					}
				}
				if (bundles.Count == 0)
				{
					bundles.Add(PersistentSingleton<Economies>.Instance.BundleTiers.Find((BundleTierConfig s) => s.ProductEnum == IAPProductEnum.bundle1));
				}
				currentBundle = (int)bundles[UnityEngine.Random.Range(0, bundles.Count)].ProductEnum;
			}
		}
		PlayerData.Instance.BundleTimeStamp.Value = ServerTimeService.NowTicks();
		PlayerData.Instance.CurrentBundleEnum.SetValueAndForceNotify(currentBundle);
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.NewBundle.Value = (IAPProductEnum)currentBundle;
		}
	}

	private void InitializeBundleRunner(int productIDNum)
	{
		if (productIDNum == -1)
		{
			if (PlayerData.Instance.BundleTimeStamp.Value != 0)
			{
				cycleOn.SetValueAndForceNotify(value: false);
				PersistentSingleton<NotificationRunner>.Instance.CreateBundleNotification(deleteOld: true, TimeSpan.FromTicks(PlayerData.Instance.BundleTimeStamp.Value) + TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.BundleTransitionTime), IAPProductEnum.bundleNew);
			}
			return;
		}
		ProductID.Value = (IAPProductEnum)productIDNum;
		m_iapConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == ProductID.Value);
		Title.Value = PersistentSingleton<LocalizationService>.Instance.Text("IAPItem.Title." + ProductID.Value);
		MainDescription.Value = PersistentSingleton<LocalizationService>.Instance.Text("IAPItem.Desc." + ProductID.Value);
		MainRewardAmount.Value = m_iapConfig.Reward.Amount;
		MainReward.SetValueAndForceNotify(m_iapConfig.Reward);
		Extra1Reward.SetValueAndForceNotify(m_iapConfig.ExtraRewards[0]);
		Extra2Reward.SetValueAndForceNotify(m_iapConfig.ExtraRewards[1]);
		PriceString.SetValueAndForceNotify(m_iapConfig.GetLocalizedPriceStringOrDefault());
		SaleString.SetValueAndForceNotify(PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig s) => s.ProductEnum == m_iapConfig.SaleEnum).GetLocalizedPriceStringOrEmpty());
		cycleOn.SetValueAndForceNotify(value: true);
		PersistentSingleton<NotificationRunner>.Instance.CreateBundleNotification(deleteOld: true, TimeSpan.FromTicks(PlayerData.Instance.BundleTimeStamp.Value) + TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.BundleActiveTime - 1200), ProductID.Value);
		PersistentSingleton<NotificationRunner>.Instance.CreateBundleNotification(deleteOld: false, TimeSpan.FromTicks(PlayerData.Instance.BundleTimeStamp.Value) + TimeSpan.FromSeconds(PersistentSingleton<GameSettings>.Instance.BundleActiveTime + PersistentSingleton<GameSettings>.Instance.BundleDeactiveTime), IAPProductEnum.bundleNew);
	}

	public void BuyIAPProduct(string placement)
	{
		Singleton<IAPRunner>.Instance.BuyIAP(m_iapConfig, placement);
	}

	public void SetBundleIAP(int bundleNum)
	{
		List<BundleTierConfig> bundleTiers = PersistentSingleton<Economies>.Instance.BundleTiers;
		PlayerData.Instance.BundleTimeStamp.Value = ServerTimeService.NowTicks();
		PlayerData.Instance.CurrentBundleEnum.SetValueAndForceNotify((int)bundleTiers[bundleNum].ProductEnum);
		PlayerData.Instance.CurrentBundleGearIndex.Value = 0;
	}
}
