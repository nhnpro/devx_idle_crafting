using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;

public class PlayFabService : PersistentSingleton<PlayFabService>
{
	public const long SaveThrottleSeconds = 2L;

	public const string FB_CUSTOM_ID_FILE = "/craftcustominfo.txt";

	public ReactiveProperty<string> LoggedOnPlayerId = Observable.Never<string>().ToReactiveProperty();

	public ReactiveProperty<string> LinkedFacebookId = Observable.Never<string>().ToReactiveProperty();

	public bool ShouldLoginImmediately;

	private string m_sessionTicket;

	private Queue<PlayFabCommand> m_commandQueue = new Queue<PlayFabCommand>();

	private PlayFabCommand m_commandInFlight;

	private ReactiveProperty<bool> m_hasCommandsInFlight = new ReactiveProperty<bool>();

	public static string m_playerIdentifier = string.Empty;

	public void LateInitialize()
	{
		if (ShouldLoginImmediately)
		{
			Login();
			ShouldLoginImmediately = false;
		}
		if (base.Inited)
		{
			Singleton<CloudSyncRunner>.Instance.CloudSync("init_pf");
			return;
		}
		UniRx.IObservable<bool> first = from should in ConnectivityService.InternetConnectionAvailable.CombineLatest(PlayerData.Instance.LifetimePrestiges, (bool net, int prestiges) => net && string.IsNullOrEmpty(LoggedOnPlayerId.Value) && prestiges > 0)
			where should
			select should;
		UniRx.IObservable<bool> observable = from should in ConnectivityService.InternetConnectionAvailable.CombineLatest(PersistentSingleton<FacebookAPIService>.Instance.IsLoggedToFB, (bool net, bool fb) => net && fb)
			where should
			select should;
		first.Merge(observable).Subscribe(delegate
		{
			Login();
		});
		MainThreadDispatcher.StartCoroutine(QueueRoutine());
		(from chunk in PlayerData.Instance.LifetimeChunk
			select (chunk != 0) ? Observable.Never<bool>() : m_hasCommandsInFlight.AsObservable()).Switch().Subscribe(delegate(bool inFlight)
		{
			if (BindingManager.Instance != null)
			{
				BindingManager.Instance.SystemPopup.SetActive(inFlight);
			}
		});
		LoggedOnPlayerId.Subscribe(delegate(string loggedID)
		{
			PlayerData.Instance.PFId.Value = loggedID;
		});
		LoggedOnPlayerId.CombineLatest(PlayerData.Instance.DisplayName, (string id, string dn) => dn).Subscribe(delegate(string name)
		{
			PlayFabService playFabService = this;
			GetAccountInfo(delegate(JSONObject json)
			{
				string a = json.asJSONObject("AccountInfo").asJSONObject("TitleInfo").asString("DisplayName", () => string.Empty);
				if (name != string.Empty && a != name)
				{
					playFabService.UpdateUserDisplayName(name, null, null);
				}
				else if (name == string.Empty && a == string.Empty)
				{
					(from fbname in Singleton<FacebookRunner>.Instance.PlayerName.AsObservable().Take(1)
						where !string.IsNullOrEmpty(fbname)
						select fbname).Subscribe(delegate(string fbname)
					{
						playFabService.UpdateUserDisplayName(fbname, null, null);
					});
				}
			}, null);
		});
		base.Inited = true;
	}

	private void QueueCommand(PlayFabCommand cmd)
	{
		m_commandQueue.Enqueue(cmd);
	}

	public void ClearCmdQueue()
	{
		m_commandInFlight = null;
		m_commandQueue.Clear();
	}

	private IEnumerator QueueRoutine()
	{
		while (true)
		{
			yield return null;
			if (m_commandQueue.Count > 0 && m_commandInFlight == null)
			{
				m_commandInFlight = m_commandQueue.Dequeue();
			}
			if (m_commandInFlight == null)
			{
				m_hasCommandsInFlight.Value = false;
				continue;
			}
			m_hasCommandsInFlight.Value = true;
			m_commandInFlight.Tries++;
			Dictionary<string, string> headers = new Dictionary<string, string>
			{
				["Content-Type"] = "application/json"
			};
			string resolvedSessionTicket = (m_commandInFlight.OverrideSessionTicket != null) ? m_commandInFlight.OverrideSessionTicket : m_sessionTicket;
			if (resolvedSessionTicket != null)
			{
				headers["X-Authentication"] = resolvedSessionTicket;
			}
			WWW www = new WWW(m_commandInFlight.Url, Encoding.UTF8.GetBytes(m_commandInFlight.Body.ToString()), headers);
			yield return www;
			string error = www.error;
			if (m_commandInFlight == null)
			{
				continue;
			}
			if (error != null)
			{
				if (m_commandInFlight.Url == PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "UpdateUserTitleDisplayName" || m_commandInFlight.Url == PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetFriendLeaderboard")
				{
					if (m_commandInFlight.ErrorCallback != null)
					{
						m_commandInFlight.ErrorCallback(error);
					}
					m_commandInFlight = null;
				}
				yield return new WaitForSeconds(5f);
				if (m_commandInFlight != null && m_commandInFlight.Tries >= 3)
				{
					if (m_commandInFlight.ErrorCallback != null)
					{
						m_commandInFlight.ErrorCallback(error);
					}
					ClearCmdQueue();
					Login();
				}
			}
			else
			{
				if (m_commandInFlight.Callback != null)
				{
					m_commandInFlight.Callback(ResponseToJSON(www));
				}
				m_commandInFlight = null;
			}
		}
	}

	public static string GetSystemUniqueIdentifier()
	{
		if (m_playerIdentifier == string.Empty)
		{
			return "CA_" + SystemInfo.deviceUniqueIdentifier;
		}
		return "CA_" + m_playerIdentifier;
	}

	public static DateTime StringToDateTime(string sortableDate)
	{
		return DateTime.Parse(sortableDate, CultureInfo.InvariantCulture).ToUniversalTime();
	}

	public void LinkCustomId(string customId, bool force, string sessionTicket, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("CustomId", customId);
		jSONObject.AddField("ForceLink", force);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "LinkCustomID", jSONObject, callback, errorCallback, sessionTicket);
		QueueCommand(cmd);
	}

	public void LinkFacebookAccount(AccessToken access, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("AccessToken", access.TokenString);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "LinkFacebookAccount", jSONObject, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void LoginWithFacebook(AccessToken access, bool createAccount, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("TitleId", PersistentSingleton<GameSettings>.Instance.PlayFabTitleId);
		jSONObject.AddField("CreateAccount", createAccount);
		jSONObject.AddField("InfoRequestParameters", GetLoginInfoRequestParameters());
		jSONObject.AddField("AccessToken", access.TokenString);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "LoginWithFacebook", jSONObject, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void Login()
	{
		JSONObject customInfo = GetCustomInfo();
		if (customInfo.HasField("CustomId"))
		{
			LoginWithCustomIdAndGetInfo(customInfo.asString("CustomId", () => string.Empty), delegate(JSONObject json)
			{
				PostLoginHandler(json);
			}, null);
		}
		else
		{
			LoginAndGetInfo(delegate(JSONObject json)
			{
				PostLoginHandler(json);
			}, null);
		}
	}

	public void LoginAndGetInfo(Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("OS", SystemInfo.operatingSystem);
		jSONObject.AddField("AndroidDeviceId", GetSystemUniqueIdentifier());
		jSONObject.AddField("AndroidDevice", SystemInfo.deviceModel);
		string loginEndpoint = "LoginWithAndroidDeviceID";
		LoginAndGetInfo(jSONObject, loginEndpoint, callback, errorCallback);
	}

	public void LoginWithCustomIdAndGetInfo(string customId, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("OS", SystemInfo.operatingSystem);
		jSONObject.AddField("CustomId", customId);
		string loginEndpoint = "LoginWithCustomID";
		LoginAndGetInfo(jSONObject, loginEndpoint, callback, errorCallback);
	}

	public void LoginAndGetInfo(JSONObject loginRequest, string loginEndpoint, Action<JSONObject> callback, Action<string> errorCallback, bool createAccount = true)
	{
		loginRequest.AddField("TitleId", PersistentSingleton<GameSettings>.Instance.PlayFabTitleId);
		loginRequest.AddField("CreateAccount", createAccount);
		loginRequest.AddField("InfoRequestParameters", GetLoginInfoRequestParameters());
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + loginEndpoint, loginRequest, callback, errorCallback);
		QueueCommand(cmd);
	}

	private void ClaimSession(Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		JSONObject jSONObject2 = JSONObject.Create();
		jSONObject2.AddField("SessionOwnerDeviceId", GetSystemUniqueIdentifier());
		jSONObject.AddField("Data", jSONObject2);
		jSONObject.AddField("Permission", "Private");
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "UpdateUserData", jSONObject, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void GetTournamentLeaderboard(string statisticName, Action<Leaderboard> callback, Action<string> errorCallback, int maxResultsCount, bool useSpecificVersion = false, int version = 0)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("StatisticName", statisticName);
		jSONObject.AddField("PlayFabId", "D1E872B9C9DA0648");
		jSONObject.AddField("MaxResultsCount", 1);
		jSONObject.AddField("UseSpecificVersion", useSpecificVersion);
		jSONObject.AddField("Version", version);
		Action<Leaderboard> fromLeaderboardPoint = delegate(Leaderboard lb)
		{
			int val = 0;
			if (lb.Entries[0].Position.Value > maxResultsCount)
			{
				val = UnityEngine.Random.Range(0, lb.Entries[0].Position.Value - maxResultsCount);
			}
			JSONObject jSONObject2 = JSONObject.Create();
			jSONObject2.AddField("StatisticName", statisticName);
			jSONObject2.AddField("StartPosition", val);
			jSONObject2.AddField("MaxResultsCount", maxResultsCount);
			jSONObject2.AddField("UseSpecificVersion", useSpecificVersion);
			jSONObject2.AddField("Version", version);
			jSONObject2.AddField("ProfileConstraints", GetProfileConstraints());
			PlayFabCommand cmd2 = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetLeaderboard", jSONObject2, delegate(JSONObject json2)
			{
				callback(LeaderboardFromJSON(json2));
			}, errorCallback);
			QueueCommand(cmd2);
		};
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetLeaderboardAroundPlayer", jSONObject, delegate(JSONObject json)
		{
			fromLeaderboardPoint(LeaderboardFromJSON(json));
		}, errorCallback);
		QueueCommand(cmd);
	}

	public void GetGlobalLeaderboard(string statisticName, Action<Leaderboard> callback, Action<string> errorCallback, int maxResultsCount, bool useSpecificVersion = false, int version = 0)
	{
		JSONObject request = JSONObject.Create();
		request.AddField("StatisticName", statisticName);
		request.AddField("StartPosition", 0);
		request.AddField("MaxResultsCount", maxResultsCount);
		request.AddField("UseSpecificVersion", useSpecificVersion);
		request.AddField("Version", version);
		request.AddField("ProfileConstraints", GetProfileConstraints());
		Action<Leaderboard> aroundPlayer = delegate(Leaderboard lb)
		{
			PlayFabCommand cmd2 = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetLeaderboardAroundPlayer", request, delegate(JSONObject json2)
			{
				callback(lb.WithEntries(LeaderboardFromJSON(json2).Entries));
			}, errorCallback);
			QueueCommand(cmd2);
		};
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetLeaderboard", request, delegate(JSONObject json)
		{
			aroundPlayer(LeaderboardFromJSON(json));
		}, errorCallback);
		QueueCommand(cmd);
	}

	public void GetLeaderboardAroundPlayer(string statisticName, string playFabId, Action<Leaderboard> callback, Action<string> errorCallback, int maxResultsCount, bool useSpecificVersion = false, int version = 0)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("StatisticName", statisticName);
		jSONObject.AddField("PlayFabId", playFabId);
		jSONObject.AddField("MaxResultsCount", maxResultsCount);
		jSONObject.AddField("UseSpecificVersion", useSpecificVersion);
		jSONObject.AddField("IncludeFacebookFriends", val: true);
		jSONObject.AddField("Version", version);
		jSONObject.AddField("ProfileConstraints", GetProfileConstraints());
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetLeaderboardAroundPlayer", jSONObject, delegate(JSONObject json)
		{
			callback(LeaderboardFromJSON(json));
		}, errorCallback);
		QueueCommand(cmd);
	}

	public void GetLeaderboard(string statisticName, int startPosition, Action<Leaderboard> callback, Action<string> errorCallback, int maxResultsCount, bool useSpecificVersion = false, int version = 0)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("StatisticName", statisticName);
		jSONObject.AddField("StartPosition", startPosition);
		jSONObject.AddField("MaxResultsCount", maxResultsCount);
		jSONObject.AddField("UseSpecificVersion", useSpecificVersion);
		jSONObject.AddField("IncludeFacebookFriends", val: true);
		jSONObject.AddField("Version", version);
		jSONObject.AddField("ProfileConstraints", GetProfileConstraints());
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetLeaderboard", jSONObject, delegate(JSONObject json)
		{
			callback(LeaderboardFromJSON(json));
		}, errorCallback);
		QueueCommand(cmd);
	}

	public void GetFriendLeaderboard(string statisticName, Action<Leaderboard> callback, Action<string> errorCallback, int maxResultsCount, bool useSpecificVersion = false, int version = 0)
	{
		JSONObject request = JSONObject.Create();
		request.AddField("StatisticName", statisticName);
		request.AddField("StartPosition", 0);
		request.AddField("MaxResultsCount", maxResultsCount);
		request.AddField("UseSpecificVersion", useSpecificVersion);
		request.AddField("IncludeFacebookFriends", val: true);
		request.AddField("Version", version);
		request.AddField("ProfileConstraints", GetProfileConstraints());
		Action<Leaderboard> aroundPlayer = delegate(Leaderboard lb)
		{
			PlayFabCommand cmd2 = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetFriendLeaderboardAroundPlayer", request, delegate(JSONObject json2)
			{
				callback(lb.WithEntries(LeaderboardFromJSON(json2).Entries));
			}, errorCallback);
			QueueCommand(cmd2);
		};
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetFriendLeaderboard", request, delegate(JSONObject json)
		{
			aroundPlayer(LeaderboardFromJSON(json));
		}, errorCallback);
		QueueCommand(cmd);
	}

	private JSONObject GetProfileConstraints()
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("ShowTags", val: true);
		jSONObject.AddField("ShowDisplayName", val: true);
		return jSONObject;
	}

	private Leaderboard LeaderboardFromJSON(JSONObject json)
	{
		return new Leaderboard(json.asInt("Version", () => 0), json.asJSONObject("Leaderboard", JSONObject.Type.ARRAY).list.Select(delegate(JSONObject entry)
		{
			List<string> list = new List<string>();
			List<JSONObject> list2 = entry.asJSONObject("Profile").asJSONObject("Tags", JSONObject.Type.ARRAY).list;
			foreach (JSONObject item2 in list2)
			{
				string item = item2.asString("TagValue", () => string.Empty);
				list.Add(item);
			}
			return new LeaderboardEntry
			{
				PlayerId = new ReactiveProperty<string>(entry.asString("PlayFabId", () => string.Empty)),
				DisplayName = new ReactiveProperty<string>(entry.asString("DisplayName", () => string.Empty)),
				StatValue = new ReactiveProperty<int>(entry.asInt("StatValue", () => 0)),
				Position = new ReactiveProperty<int>(entry.asInt("Position", () => 0) + 1),
				Tags = new ReactiveProperty<List<string>>(list)
			};
		}).ToList());
	}

	public void GetUserData(Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("Keys", "Base");
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetUserData", jSONObject, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void GetAccountInfo(Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject body = JSONObject.Create();
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetAccountInfo", body, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void UpdatePlayerData(Dictionary<string, JSONObject> saveData, Dictionary<string, int> stats, long saveFileVersion, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		foreach (KeyValuePair<string, JSONObject> saveDatum in saveData)
		{
			jSONObject.AddField(saveDatum.Key, JSONEscape(saveDatum.Value.ToString()));
		}
		foreach (KeyValuePair<string, int> stat in stats)
		{
			jSONObject.AddField(stat.Key, stat.Value);
		}
		jSONObject.AddField("SaveFileVersion", saveFileVersion);
		jSONObject.AddField("DeviceId", GetSystemUniqueIdentifier());
		ExecuteCloudScript("updatePlayer", callback, errorCallback, jSONObject);
	}

	public void UpdatePlayerStatistic(JSONObject statistic, Action<JSONObject> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.Add(statistic);
		JSONObject jSONObject2 = JSONObject.Create();
		jSONObject2.AddField("Statistics", jSONObject);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "UpdatePlayerStatistics", jSONObject2, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void UpdateUserDisplayName(string name, Action<JSONObject> callback, Action<string> errorCallback)
	{
		name = name.PadRight(3, ' ');
		name = name.Substring(0, Mathf.Min(name.Length, 25));
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("DisplayName", name);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "UpdateUserTitleDisplayName", jSONObject, callback, errorCallback);
		QueueCommand(cmd);
	}

	public void GetPlayFabToFacebookIdMapping(List<string> facebookIds, Action<Dictionary<string, string>> callback, Action<string> errorCallback)
	{
		if (facebookIds.Count == 0)
		{
			callback(new Dictionary<string, string>());
			return;
		}
		JSONObject jSONObject = JSONObject.Create();
		JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
		foreach (string facebookId in facebookIds)
		{
			jSONObject2.Add(facebookId);
		}
		jSONObject.AddField("FacebookIDs", jSONObject2);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetPlayFabIDsFromFacebookIDs", jSONObject, delegate(JSONObject json)
		{
			Dictionary<string, string> obj = (from e in json.asJSONObject("Data", JSONObject.Type.ARRAY).list
				where e.HasField("PlayFabId") && e.HasField("FacebookId")
				select e).ToDictionary((JSONObject e) => e.asString("PlayFabId", () => string.Empty), (JSONObject e) => e.asString("FacebookId", () => string.Empty));
			callback(obj);
		}, errorCallback);
		QueueCommand(cmd);
	}

	public void GetFacebookToPlayFabID(string facebookId, Action<string> callback, Action<string> errorCallback)
	{
		JSONObject jSONObject = JSONObject.Create();
		JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
		jSONObject2.Add(facebookId);
		jSONObject.AddField("FacebookIDs", jSONObject2);
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "GetPlayFabIDsFromFacebookIDs", jSONObject, delegate(JSONObject json)
		{
			Dictionary<string, string> dictionary = (from e in json.asJSONObject("Data", JSONObject.Type.ARRAY).list
				where e.HasField("PlayFabId") && e.HasField("FacebookId")
				select e).ToDictionary((JSONObject e) => e.asString("FacebookId", () => string.Empty), (JSONObject e) => e.asString("PlayFabId", () => string.Empty));
			callback((!dictionary.ContainsKey(facebookId)) ? string.Empty : dictionary[facebookId]);
		}, errorCallback);
		QueueCommand(cmd);
	}

	public void ExecuteCloudScript(string functionName, Action<JSONObject> callback, Action<string> errorCallback, JSONObject parameters = null)
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("FunctionName", functionName);
		jSONObject.AddField("GeneratePlayStreamEvent", val: true);
		if (parameters != null)
		{
			jSONObject.AddField("FunctionParameter", parameters);
		}
		PlayFabCommand cmd = new PlayFabCommand(PersistentSingleton<GameSettings>.Instance.PlayFabUrl + "ExecuteCloudScript", jSONObject, callback, errorCallback);
		QueueCommand(cmd);
	}

	public static JSONObject ResponseToJSON(WWW www)
	{
		JSONObject jSONObject = JSONObject.Create(www.text).asJSONObject("data");
		try
		{
			string value = www.responseHeaders.First((KeyValuePair<string, string> e) => e.Key.ToLower().Equals("date")).Value;
			jSONObject.AddField("Timestamp", DateTime.Parse(value).ToUniversalTime().Ticks);
			return jSONObject;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Could not get response timestamp: " + ex.Message);
			return jSONObject;
		}
	}

	private void PostLoginHandler(JSONObject loginResponse)
	{
		m_sessionTicket = loginResponse.asString("SessionTicket", () => string.Empty);
		LinkedFacebookId.Value = new PlayFabAccountIds(loginResponse).FacebookId;
		LoggedOnPlayerId.Value = loginResponse.asString("PlayFabId", () => string.Empty);
		ClaimSession(null, null);
		if (loginResponse.asBool("NewlyCreated", () => false) && PlayerData.Instance.LastSavedToCloud.Value < 1)
		{
			Singleton<CloudSyncRunner>.Instance.UploadSaveToCloud(PlayerData.Instance, "new_account", null, null);
		}
		else
		{
			Singleton<CloudSyncRunner>.Instance.CloudSync("login");
		}
	}

	private JSONObject GetLoginInfoRequestParameters()
	{
		JSONObject jSONObject = JSONObject.Create();
		jSONObject.AddField("GetTitleData", val: true);
		jSONObject.AddField("GetUserAccountInfo", val: true);
		jSONObject.AddField("GetUserData", val: true);
		return jSONObject;
	}

	public JSONObject GetCustomInfo()
	{
		string path = Application.persistentDataPath + "/craftcustominfo.txt";
		try
		{
			return (!File.Exists(path)) ? JSONObject.Create() : JSONObject.Create(Encryptor.Decrypt(File.ReadAllText(path)));
		}
		catch (Exception)
		{
			return JSONObject.Create();
		}
	}

	public void WriteCustomInfo(JSONObject customInfo)
	{
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + "/craftcustominfo.txt", Encryptor.Encrypt(customInfo.ToString()));
		}
		catch (IOException)
		{
			SaveLoad.ShowDiskFullDialog();
		}
	}

	private PlayFabException GetException(WWWErrorException wwwError)
	{
		try
		{
			JSONObject json = JSONObject.Create(wwwError.WWW.text);
			int errorCode = json.asInt("errorCode", () => 0);
			string errorName = json.asString("error", () => string.Empty);
			string errorMessage = json.asString("errorMessage", () => string.Empty);
			return new PlayFabException(errorCode, errorName, errorMessage);
		}
		catch (Exception)
		{
			return new PlayFabException(PlayFabError.UNKNOWN, string.Empty, string.Empty);
		}
	}

	public static string JSONEscape(string jsonString)
	{
		return jsonString.Replace("\\", "\\\\").Replace("\"", "\\\"");
	}

	public static string JSONUnescape(string escapedJsonString)
	{
		return Regex.Unescape(escapedJsonString);
	}
}
