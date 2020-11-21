using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public abstract class BaseData
{
	public enum ReviewStates
	{
		Pending,
		Reviewed,
		NeedsImprovement
	}

	public string PlayerId;

	public string AppVersion;

	public long SaveFileVersion;

	public int AcceptedVersion;

	public string Language;

	public long LastSaved;

	public ReactiveProperty<long> LastSavedToCloud = new ReactiveProperty<long>(0L);

	public string LastSavedBy;

	public ReactiveProperty<long> LastSessionStart = new ReactiveProperty<long>(0L);

	public long InstallTime;

	public ReactiveProperty<long> SumOfPreviousSessionTimes = new ReactiveProperty<long>(0L);

	public ReactiveProperty<int> DaysRetained = new ReactiveProperty<int>(-1);

	public ReactiveCollection<AdHistory> AdsWatched = new ReactiveCollection<AdHistory>();

	public ReactiveProperty<int> SessionNumber = new ReactiveProperty<int>(-1);

	public ReactiveProperty<int> PurchasesMade = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> InvalidPurchasesMade = new ReactiveProperty<int>(0);

	public int AdsInSession;

	public int AdsInLifetime;

	public ReactiveProperty<ReviewStates> ReviewState = new ReactiveProperty<ReviewStates>(ReviewStates.Pending);

	public string LastVersionReviewed;

	public ReactiveProperty<bool> HasReviewed = new ReactiveProperty<bool>(initialValue: false);

	public long MinutesInGame;

	[PropertyBool]
	public ReactiveProperty<bool> NotificationDecision = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<string> FBId = new ReactiveProperty<string>(string.Empty);

	public ReactiveProperty<string> PFId = new ReactiveProperty<string>(string.Empty);

	public long StringCacheCrc32;

	public ReactiveCollection<string> PurchasedIAPBundleIDs = new ReactiveCollection<string>();

	public bool GPGSPermission = true;

	public bool GPGSPermissionAsked;

	public bool AndroidAchievementPermission;

	public string AndroidAchievementAskedVersion = "none";

	public string LastSeenGameVersion;

	public void SetupNewData(long nowUtcTicks)
	{
		PlayerId = Guid.NewGuid().ToString();
		AppVersion = Application.version;
		LastSessionStart.Value = nowUtcTicks;
		InstallTime = nowUtcTicks;
	}

	public List<AdHistory> GetAdsWatched24H()
	{
		long yesterday = ServerTimeService.NowTicks() - TimeSpan.FromHours(24.0).Ticks;
		return (from adh in AdsWatched.ToList()
			where adh.TimeStamp >= yesterday
			select adh).ToList();
	}
}
