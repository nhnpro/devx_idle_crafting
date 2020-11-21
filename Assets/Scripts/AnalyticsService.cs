using Facebook.Unity;
using Firebase.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class AnalyticsService : PersistentSingleton<AnalyticsService>
{
	public ReactiveProperty<FortunePodResult> FortunePodsResult = Observable.Never<FortunePodResult>().ToReactiveProperty();

	public void InitializeAnalytics()
	{
		if (!base.Inited)
		{
			FirebaseAnalytics.SetAnalyticsCollectionEnabled(enabled: true);
			FirebaseAnalytics.SetUserId(PlayerData.Instance.PlayerId);
			PersistentSingleton<IAPService>.Instance.IAPValidated.Subscribe(delegate
			{
				LogPurchaseEvent();
			});
			PersistentSingleton<IAPService>.Instance.IAPInvalidated.Subscribe(delegate
			{
				LogInvalidPurchaseEvent();
			});
			base.Inited = true;
		}
	}

	private void LogPurchaseEvent()
	{
		PlayerData.Instance.PurchasesMade.Value++;
	}

	private void LogInvalidPurchaseEvent()
	{
		PlayerData.Instance.InvalidPurchasesMade.Value++;
	}

	public void TrackEvent(string key, Dictionary<string, string> parameters = null, string FB_key = "", float FBParam = 0f)
	{
		if (parameters == null)
		{
			parameters = new Dictionary<string, string>();
		}
		PersistentSingleton<GlobalAnalyticsParameters>.Instance.AddFirebaseParameters(parameters);
		if (PersistentSingleton<GameSettings>.Instance.FirebaseAnalyticsEnabled)
		{
			if (parameters.Count > 25)
			{
				UnityEngine.Debug.LogError("Too many parameters for firebase: " + string.Join(",", parameters.Keys.ToArray()));
			}
			FirebaseAnalytics.LogEvent(key, ToFirebaseParameters(parameters));
		}
		if (FB.IsInitialized)
		{
			Dictionary<string, object> dictionary = ((IEnumerable<KeyValuePair<string, string>>)parameters).ToDictionary((Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> p) => p.Key), (Func<KeyValuePair<string, string>, object>)((KeyValuePair<string, string> p) => p.Value));
			PersistentSingleton<GlobalAnalyticsParameters>.Instance.AddFacebookParameters(dictionary);
			if (dictionary.Count > 25)
			{
				UnityEngine.Debug.LogError("Too many parameter for Facebook analytics " + string.Join(",", dictionary.Keys.ToArray()));
			}
			FB.LogAppEvent(string.IsNullOrEmpty(FB_key) ? key : FB_key, FBParam, dictionary.Take(25).ToDictionary((KeyValuePair<string, object> kv) => kv.Key, (KeyValuePair<string, object> kv) => kv.Value));
		}
	}

	private Parameter[] ToFirebaseParameters(Dictionary<string, string> dict)
	{
		return (from kv in dict.Take(25)
			select new Parameter(kv.Key, kv.Value)).ToArray();
	}

	public static void LogEvent(string key, Dictionary<string, string> parameters)
	{
		string text = key;
		foreach (KeyValuePair<string, string> parameter in parameters)
		{
			string text2 = text;
			text = text2 + "[" + parameter.Key + "=" + parameter.Value + "]";
		}
	}
}
