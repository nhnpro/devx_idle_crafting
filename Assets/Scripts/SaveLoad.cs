using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

public class SaveLoad : PersistentSingleton<SaveLoad>
{
	public const string SAVE_FILE = "/saveGame.idl";

	public const string BAK_FILE = "/saveGameBak.idl";

	public void Save(BaseData baseData, string reason)
	{
		JSONObject jSONObject = DataToJSON(baseData);
		jSONObject.SetField("AppVersion", Application.version);
		jSONObject.SetField("LastSaved", ServerTimeService.NowTicks());
		jSONObject.SetField("LastSavedBy", reason);
		baseData.LastSaved = ServerTimeService.NowTicks();
		baseData.LastSavedBy = reason;
		Save(jSONObject);
		PersistentSingleton<FacebookAPIService>.Instance.SaveFBData();
	}

	private void Save(JSONObject json)
	{
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + "/saveGame.idl", Encryptor.Encrypt(json.ToString()));
		}
		catch (IOException)
		{
			ShowDiskFullDialog();
		}
	}

	private void SaveBackup(BaseData baseData)
	{
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + "/saveGameBak.idl", Encryptor.Encrypt(DataToJSON(baseData).ToString()));
		}
		catch (IOException)
		{
			ShowDiskFullDialog();
		}
	}

	public void DebugSaveTo(BaseData data, string filename, string reason)
	{
		data.LastSaved = ServerTimeService.NowTicks();
		data.LastSavedBy = reason;
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + filename, Encryptor.Encrypt(DataToJSON(data).ToString()));
		}
		catch (IOException)
		{
			ShowDiskFullDialog();
		}
	}

	public void Load(BaseData data)
	{
		LoadFile(data, "/saveGame.idl", shouldSaveBackup: true, LoadBackup);
	}

	public void Load(string filename, BaseData data)
	{
		LoadFile(data, filename, shouldSaveBackup: true, LoadBackup);
	}

	public void DebugResetData(BaseData data)
	{
		PersistentSingleton<ARService>.Instance.DebugDeleteARLevel();
		PersistentSingleton<IAPService>.Instance.UnpublishPurchases();
		PersistentSingleton<FacebookAPIService>.Instance.ResetFacebookLogin();
		CreateNewData(data);
	}

	public void CreateNewData(BaseData data)
	{
		JSONObject j = new JSONObject();
		PlayerDataLoader.Instance().FillBaseData(j, data);
		data.SetupNewData(ServerTimeService.NowTicks());
		Save(data, "Initial");
	}

	private void LoadBackup(BaseData data)
	{
		LoadFile(data, "/saveGameBak.idl", shouldSaveBackup: false, CreateNewData);
	}

	private JSONObject LoadToJSON(string fileName)
	{
		string text = File.ReadAllText(PersistentDataPath.Get() + fileName);
		string text2 = Encryptor.Decrypt(text);
		if (text2.Contains("PlayerId"))
		{
			return JSONObject.Create(text2);
		}
		TrackLoadingError(GetPlayerId(text2), new Dictionary<string, object>
		{
			{
				"encrypted",
				text
			},
			{
				"decrypted",
				text2
			}
		});
		throw new Exception("Could not find PlayerId in save file");
	}

	private void LoadFile(BaseData data, string fileName, bool shouldSaveBackup, Action<BaseData> funcLoadDefault)
	{
		if (!File.Exists(PersistentDataPath.Get() + fileName))
		{
			funcLoadDefault(data);
		}
		else
		{
			try
			{
				JSONToData(LoadToJSON(fileName), data);
				if (shouldSaveBackup)
				{
					SaveBackup(data);
				}
			}
			catch (Exception)
			{
				funcLoadDefault(data);
			}
		}
	}

	private string GetPlayerId(string decrypted)
	{
		try
		{
			return decrypted.Substring(decrypted.IndexOf("PlayerId") + 11, 36);
		}
		catch (Exception)
		{
		}
		return "N/A";
	}

	private void TrackLoadingError(string playerId, Dictionary<string, object> failParams)
	{
		Analytics.SetUserId(playerId);
		Analytics.CustomEvent("load_failure", failParams);
	}

	public bool FileExists(string fileName)
	{
		string path = PersistentDataPath.Get() + "/" + fileName;
		return File.Exists(path);
	}

	public JSONObject DataToJSON(BaseData baseData)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		PlayerDataLoader.Instance().FillJson(jSONObject, baseData);
		return jSONObject;
	}

	private void JSONToData(JSONObject json, BaseData data)
	{
		PlayerDataLoader.Instance().FillBaseData(json, data);
	}

	public static void ShowDiskFullDialog()
	{
		Debug.LogError("ShowDiskFullDialog");
		// PopupMessage.Create(PersistentSingleton<LocalizationService>.Instance.Text("DiskFull.Title"), PersistentSingleton<LocalizationService>.Instance.Text("DiskFull.Body"), PersistentSingleton<LocalizationService>.Instance.Text("DiskFull.Button"));
	}
}
