using System;
using System.Collections.Generic;

public class IAPStarted
{
	public IAPConfig Config
	{
		get;
		private set;
	}

	public string Placement
	{
		get;
		private set;
	}

	public IAPStarted(IAPConfig cfg, string p)
	{
		Config = cfg;
		Placement = p;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		try
		{
			dictionary.Add("ProductID", Config.ProductID);
			dictionary.Add("Placement", Placement);
			return dictionary;
		}
		catch (Exception ex)
		{
			dictionary.Add("Error", ex.Message);
			return dictionary;
		}
	}
}
