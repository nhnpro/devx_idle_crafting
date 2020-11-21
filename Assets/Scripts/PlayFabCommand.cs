using System;

public class PlayFabCommand
{
	public string Url
	{
		get;
		private set;
	}

	public JSONObject Body
	{
		get;
		private set;
	}

	public Action<JSONObject> Callback
	{
		get;
		private set;
	}

	public Action<string> ErrorCallback
	{
		get;
		private set;
	}

	public string OverrideSessionTicket
	{
		get;
		private set;
	}

	public int Tries
	{
		get;
		set;
	}

	public PlayFabCommand(string url, JSONObject body, Action<JSONObject> callback, Action<string> errorCallback, string overrideSessionTicket = null)
	{
		Url = url;
		Body = body;
		Callback = callback;
		ErrorCallback = errorCallback;
		OverrideSessionTicket = overrideSessionTicket;
		Tries = 0;
	}
}
