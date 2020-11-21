using System;
using UniRx;
using UnityEngine;

public class BaseDataConverterImpl : IBaseDataConverter
{
	public virtual void FillBaseData(JSONObject j, BaseData pd)
	{
		pd.PlayerId = j.asString("PlayerId", () => pd.PlayerId);
		pd.AppVersion = j.asString("AppVersion", () => pd.AppVersion);
		pd.SaveFileVersion = j.asLong("SaveFileVersion", () => 0L);
		pd.AcceptedVersion = j.asInt("AcceptedVersion", () => 0);
		pd.Language = j.asString("Language", () => null);
		pd.LastSaved = j.asLong("LastSaved", () => 0L);
		pd.LastSavedToCloud.Value = j.asLong("LastSavedToCloud", () => pd.LastSavedToCloud.Value);
		pd.LastSavedBy = j.asString("LastSavedBy", () => string.Empty);
		pd.LastSessionStart.Value = j.asLong("LastSessionStart", () => pd.LastSessionStart.Value);
		pd.InstallTime = j.asLong("InstallTime", () => pd.InstallTime);
		pd.SumOfPreviousSessionTimes.Value = j.asLong("SumOfPreviousSessionTimes", () => 0L);
		pd.DaysRetained.Value = j.asInt("DaysRetained", () => -1);
		pd.AdsWatched = j.asCustom("AdsWatched", (JSONObject f) => new ReactiveCollection<AdHistory>(AdHistory.JSONToAdHistories(f)), () => new ReactiveCollection<AdHistory>());
		pd.SessionNumber.Value = j.asInt("SessionNumber", () => -1);
		pd.PurchasesMade.Value = j.asInt("PurchasesMade", () => 0);
		pd.InvalidPurchasesMade.Value = j.asInt("InvalidPurchasesMade", () => 0);
		pd.AdsInSession = j.asInt("AdsInSession", () => 0);
		pd.AdsInLifetime = j.asInt("AdsInLifetime", () => 0);
		pd.ReviewState.Value = j.asCustom("ReviewState", (JSONObject f) => (BaseData.ReviewStates)Enum.Parse(typeof(BaseData.ReviewStates), f.str), () => BaseData.ReviewStates.Pending);
		pd.LastVersionReviewed = j.asString("LastVersionReviewed", () => "0");
		pd.HasReviewed.Value = j.asBool("HasReviewed", () => false);
		pd.MinutesInGame = j.asLong("MinutesInGame", () => 0L);
		pd.NotificationDecision.Value = j.asBool("NotificationDecision", () => false);
		pd.FBId.Value = j.asString("FBId", () => string.Empty);
		pd.PFId.Value = j.asString("PFId", () => string.Empty);
		pd.StringCacheCrc32 = j.asLong("StringCacheCrc32", () => 0L);
		pd.PurchasedIAPBundleIDs = j.asCustom("PurchasedIAPBundleIDs", (JSONObject jobj) => new ReactiveCollection<string>(DataHelper.JSONToStringList(jobj)), () => new ReactiveCollection<string>());
		pd.GPGSPermission = j.asBool("GPGSPermission", () => true);
		pd.GPGSPermissionAsked = j.asBool("GPGSPermissionAsked", () => false);
		pd.AndroidAchievementPermission = j.asBool("AndroidAchievementPermission", () => false);
		pd.AndroidAchievementAskedVersion = j.asString("AndroidAchievementAskedVersion", () => "none");
		string fallbackVersion;
		if (pd.LastSaved == 0)
		{
			fallbackVersion = Application.version;
		}
		else
		{
			fallbackVersion = "1.1.10";
		}
		pd.LastSeenGameVersion = j.asString("LastSeenGameVersion", () => fallbackVersion);
	}

	public virtual void FillJson(JSONObject j, BaseData baseData)
	{
		j.AddField("PlayerId", baseData.PlayerId);
		j.AddField("AppVersion", baseData.AppVersion);
		j.AddField("SaveFileVersion", 4);
		j.AddField("AcceptedVersion", baseData.AcceptedVersion);
		if (baseData.Language != null && baseData.Language != string.Empty)
		{
			j.AddField("Language", baseData.Language);
		}
		j.AddField("LastSaved", baseData.LastSaved);
		j.AddField("LastSavedToCloud", baseData.LastSavedToCloud.Value);
		j.AddField("LastSavedBy", baseData.LastSavedBy);
		j.AddField("LastSessionStart", baseData.LastSessionStart.Value);
		j.AddField("InstallTime", baseData.InstallTime);
		j.AddField("SumOfPreviousSessionTimes", baseData.SumOfPreviousSessionTimes.Value);
		j.AddField("DaysRetained", baseData.DaysRetained.Value);
		j.AddField("AdsWatched", DataHelper.AdHistoryToJSON(baseData));
		j.AddField("SessionNumber", baseData.SessionNumber.Value);
		j.AddField("PurchasesMade", baseData.PurchasesMade.Value);
		j.AddField("InvalidPurchasesMade", baseData.InvalidPurchasesMade.Value);
		j.AddField("AdsInSession", baseData.AdsInSession);
		j.AddField("AdsInLifetime", baseData.AdsInLifetime);
		j.AddField("ReviewState", baseData.ReviewState.Value.ToString());
		j.AddField("LastVersionReviewed", baseData.LastVersionReviewed);
		j.AddField("HasReviewed", baseData.HasReviewed.Value);
		j.AddField("MinutesInGame", baseData.MinutesInGame);
		j.AddField("NotificationDecision", baseData.NotificationDecision.Value);
		j.AddField("FBId", baseData.FBId.Value);
		j.AddField("PFId", baseData.PFId.Value);
		j.AddField("StringCacheCrc32", baseData.StringCacheCrc32);
		j.AddField("PurchasedIAPBundleIDs", DataHelper.CollectionToJSON(baseData.PurchasedIAPBundleIDs));
		j.AddField("GPGSPermission", baseData.GPGSPermission);
		j.AddField("GPGSPermissionAsked", baseData.GPGSPermissionAsked);
		j.AddField("AndroidAchievementPermission", baseData.AndroidAchievementPermission);
		j.AddField("AndroidAchievementAskedVersion", baseData.AndroidAchievementAskedVersion);
		j.AddField("LastSeenGameVersion", baseData.LastSeenGameVersion);
	}
}
