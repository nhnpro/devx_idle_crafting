using System;
using System.Collections.Generic;
using System.IO;

public class FacebookDataHandler
{
	public const string FBDATA_FILE = "/fbData.idl";

	private static FacebookDataHandler m_instance;

	private FacebookDataHandler()
	{
	}

	public static FacebookDataHandler Instance()
	{
		if (m_instance == null)
		{
			m_instance = new FacebookDataHandler();
		}
		return m_instance;
	}

	public void Save(FacebookAPIService service)
	{
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + "/fbData.idl", FacebookDataToJSON(service).ToString());
		}
		catch (IOException)
		{
			SaveLoad.ShowDiskFullDialog();
		}
	}

	private JSONObject FacebookDataToJSON(FacebookAPIService service)
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		FillFacebookJson(jSONObject, service);
		return jSONObject;
	}

	private void FillFacebookJson(JSONObject json, FacebookAPIService service)
	{
		json.AddField("FBPlayers", DataHelper.FBPlayersToJSON(service));
	}

	public void Load(FacebookAPIService service)
	{
		LoadFile(service, "/fbData.idl");
	}

	private void LoadFile(FacebookAPIService service, string fileName)
	{
		if (!File.Exists(PersistentDataPath.Get() + fileName))
		{
			CreateEmptyData(service);
		}
		else
		{
			JSONToFacebookData(LoadToJSON(fileName), service);
		}
	}

	public void CreateEmptyData(FacebookAPIService service)
	{
		JSONObject json = new JSONObject();
		FillFacebookData(json, service);
		Save(service);
	}

	private void JSONToFacebookData(JSONObject json, FacebookAPIService service)
	{
		FillFacebookData(json, service);
	}

	private JSONObject LoadToJSON(string fileName)
	{
		string text = File.ReadAllText(PersistentDataPath.Get() + fileName);
		if (text.Contains("FBPlayers"))
		{
			return JSONObject.Create(text);
		}
		throw new Exception("Could not find FBPlayers in save file");
	}

	public void FillFacebookData(JSONObject json, FacebookAPIService service)
	{
		service.FBPlayers = json.asCustom("FBPlayers", (JSONObject f) => new Dictionary<string, FBPlayer>(DataHelper.JSONToFBPlayers(f)), () => new Dictionary<string, FBPlayer>());
	}
}
