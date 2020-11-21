using Firebase;
using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.Analytics;

public static class ConfigLoader
{
	private static List<KeyValuePair<string, string>> m_configs;

	private static uint m_crc32;

	[CompilerGenerated]
	private static Func<JSONObject, Dictionary<string, string>> _003C_003Ef__mg_0024cache0;

	public static bool Fetched
	{
		get;
		private set;
	}

	public static IEnumerator FetchFirebaseConfigs()
	{
		Subject<bool> configSubject = new Subject<bool>();
		Task configTask;
		try
		{
			ConfigSettings settings = default(ConfigSettings);
			settings.IsDeveloperMode = false;
			FirebaseRemoteConfig.Settings = settings;
			configTask = FirebaseRemoteConfig.FetchAsync(TimeSpan.FromMinutes(12.0)).ContinueWith(delegate
			{
				configSubject.OnNext(value: true);
			});
		}
		catch (InitializationException ex)
		{
			Analytics.CustomEvent("FirebaseInitializationException", new Dictionary<string, object>
			{
				{
					"InitializationException",
					true
				}
			});
			UnityEngine.Debug.LogWarning("ConfigLoader.FetchFirebaseConfigs " + ex.Message);
			yield break;
		}
		catch (Exception ex2)
		{
			Analytics.CustomEvent("FirebaseException", new Dictionary<string, object>
			{
				{
					"Exception",
					true
				}
			});
			UnityEngine.Debug.LogWarning("ConfigLoader.FetchFirebaseConfigs " + ex2.Message);
			yield break;
		}
		yield return configSubject.Take(1).Timeout(TimeSpan.FromSeconds(10.0)).ToYieldInstruction(throwOnError: false);
		if (configTask != null && configTask.IsCompleted && FirebaseRemoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
		{
			m_configs = ParseConfigsFromFirebase();
		}
		Fetched = true;
	}

	private static int GetIntOrZero(List<KeyValuePair<string, string>> list, string key)
	{
		KeyValuePair<string, string> keyValuePair = list.Find((KeyValuePair<string, string> kv) => kv.Key == key);
		int result = 0;
		int.TryParse(keyValuePair.Value, out result);
		return result;
	}

	public static bool OverrideCacheWithFirebaseConfigs()
	{
		if (m_configs == null)
		{
			return false;
		}
		if (m_crc32 == PlayerData.Instance.StringCacheCrc32)
		{
			return false;
		}
		PersistentSingleton<StringCache>.Instance.Clear();
		foreach (KeyValuePair<string, string> config in m_configs)
		{
			PersistentSingleton<StringCache>.Instance.Cache(config.Key, config.Value);
		}
		PlayerData.Instance.StringCacheCrc32 = m_crc32;
		PersistentSingleton<MainSaver>.Instance.PleaseSave("cache_update");
		return true;
	}

	private static List<KeyValuePair<string, string>> ParseConfigsFromFirebase()
	{
		FirebaseRemoteConfig.ActivateFetched();
		IEnumerable<string> keys = FirebaseRemoteConfig.Keys;
		string[] array = (from key in keys
			where FirebaseRemoteConfig.GetValue(key).StringValue != string.Empty
			select key).ToArray();
		m_crc32 = GetCRC(array);
		List<JSONObject> messages = (from key in array
			select FirebaseRemoteConfig.GetValue(key).StringValue into raw
			select JSONObject.Create(raw)).ToList();
		return GetConfigs(messages).Concat(GetGameSettings(messages)).ToList();
	}

	private static uint GetCRC(string[] keys)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string key in keys)
		{
			stringBuilder.Append(FirebaseRemoteConfig.GetValue(key).StringValue);
		}
		return Crc32.Compute(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
	}

	private static List<KeyValuePair<string, string>> GetGameSettings(List<JSONObject> messages)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		JSONObject settingsJSON = JSONObject.obj;
		(from m in messages
			select m.asCustom("Settings", (JSONObject e) => e, () => null) into m
			where m != null
			select m).Reverse().ToList().ForEach(delegate(JSONObject m)
		{
			settingsJSON.Merge(m);
		});
		if (settingsJSON.list.Count() > 0)
		{
			list.Add(new KeyValuePair<string, string>("Settings", settingsJSON.ToString()));
		}
		return list;
	}

	private static List<KeyValuePair<string, string>> GetConfigs(List<JSONObject> messages)
	{
		return messages.SelectMany((JSONObject message) => message.asCustom("Config", ParseConfigs, () => new Dictionary<string, string>())).ToList();
	}

	private static Dictionary<string, string> ParseConfigs(JSONObject j)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < j.keys.Count; i++)
		{
			try
			{
				string text = j.keys[i];
				string text2 = j.asCustom(text, (JSONObject raw) => raw.str.Replace("\\t", "\t").Replace("\\n", "\n"), () => null);
				if (text2 != null)
				{
					dictionary.Add(text, text2);
				}
			}
			catch (Exception msg)
			{
				UnityEngine.Debug.LogError(msg);
			}
		}
		return dictionary;
	}
}
