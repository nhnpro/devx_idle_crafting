using Big;
using System;
using System.Collections.Generic;
using UniRx;

[PropertyClass]
public class CloudSyncRunner : Singleton<CloudSyncRunner>
{
	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> LocalProgress = new ReactiveProperty<BigDouble>(0L);

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> CloudProgress = new ReactiveProperty<BigDouble>(0L);

	private JSONObject m_cloudSave;

	public CloudSyncRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}

	public void CloudSync(string reason)
	{
		if (ConnectivityService.InternetConnectionAvailable.Value && !string.IsNullOrEmpty(PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Value))
		{
			PersistentSingleton<PlayFabService>.Instance.GetUserData(delegate(JSONObject json)
			{
				CloudSyncOrUpload(json.GetField("Data"), reason);
			}, null);
		}
	}

	private void CloudSyncOrUpload(JSONObject json, string reason)
	{
		m_cloudSave = null;
		BigDouble a = -1L;
		if (json != null && json.HasField("Base"))
		{
			json = json.GetField("Base");
			if (json != null && json.HasField("Value"))
			{
				string val = PlayFabService.JSONUnescape(json.asString("Value", () => string.Empty));
				m_cloudSave = JSONObject.Create(val);
				a = m_cloudSave.asBigDouble("LifetimeCoins", () => -1L);
			}
		}
		if (m_cloudSave != null && ShouldForceUpdate(m_cloudSave))
		{
			Debug.LogError("Show Force Update");
			// ForceUpdateService.ShowForceUpdateDialog();
		}
		else if (m_cloudSave != null && IsCloudNewer(m_cloudSave, PlayerData.Instance))
		{
			LocalProgress.Value = PlayerData.Instance.LifetimeCoins.Value;
			CloudProgress.Value = BigDouble.Max(a, 0L);
			Observable.Return(value: true).Delay(TimeSpan.FromSeconds(3.0)).Take(1)
				.Subscribe(delegate
				{
					BindingManager.Instance.CloudSyncPopup.SetActive(value: true);
				})
				.AddTo(SceneLoader.Instance);
		}
		else
		{
			UploadSaveToCloud(PlayerData.Instance, reason, null, null);
		}
	}

	private static bool ShouldForceUpdate(JSONObject cloudSave)
	{
		return cloudSave.asLong("SaveFileVersion", () => -1L) > 4;
	}

	public static bool IsCloudNewer(JSONObject cloudSave, BaseData data)
	{
		long num = 1000L;
		return cloudSave.asLong("LastSavedToCloud", () => 0L) > data.LastSavedToCloud.Value + num;
	}

	public void ApplyCloudSaveAndRestart()
	{
		BindingManager.Instance.SystemPopup.SetActive(value: true);
		PlayerDataLoader.Instance().FillBaseData(m_cloudSave, PlayerData.Instance);
		UploadSaveToCloud(PlayerData.Instance, "cloud_save", delegate
		{
			SceneLoadHelper.LoadInitScene();
		}, null);
	}

	public void UploadSaveToCloud(PlayerData playerData, string reason, Action<JSONObject> callback, Action<string> errorCallback)
	{
		long time = ServerTimeService.NowTicks();
		JSONObject jSONObject = PersistentSingleton<SaveLoad>.Instance.DataToJSON(playerData);
		jSONObject.SetField("LastSavedToCloud", time);
		Dictionary<string, JSONObject> dictionary = new Dictionary<string, JSONObject>();
		dictionary.Add("Base", jSONObject);
		Dictionary<string, JSONObject> saveData = dictionary;
		Dictionary<string, int> stats = CreateStatistics(playerData);
		Action<JSONObject> callback2 = delegate(JSONObject json)
		{
			playerData.LastSavedToCloud.Value = time;
			PersistentSingleton<SaveLoad>.Instance.Save(playerData, reason);
			if (callback != null)
			{
				callback(json);
			}
		};
		PersistentSingleton<PlayFabService>.Instance.UpdatePlayerData(saveData, stats, 4L, callback2, errorCallback);
	}

	private Dictionary<string, int> CreateStatistics(PlayerData player)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary["Highscore"] = player.LifetimeChunk.Value;
		dictionary["SaveFileVersion"] = 4;
		return dictionary;
	}
}
