using System;
using System.Collections.Generic;

public class IAPNotCompleted
{
	public IAPConfig Config
	{
		get;
		private set;
	}

	public string Status
	{
		get;
		private set;
	}

	public string Error
	{
		get;
		private set;
	}

	public IAPNotCompleted(IAPConfig cfg, string s, string e)
	{
		Config = cfg;
		Status = s;
		Error = e;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		try
		{
			dictionary.Add("ProductID", (!string.IsNullOrEmpty(Config.ProductID)) ? Status : "null");
			dictionary.Add("Status", (!string.IsNullOrEmpty(Status)) ? Status : "null");
			dictionary.Add("Error", (!string.IsNullOrEmpty(Error)) ? Error : "null");
			return dictionary;
		}
		catch (Exception ex)
		{
			dictionary.Add("Error", ex.Message);
			return dictionary;
		}
	}
}
