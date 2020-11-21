using System;
using System.Collections.Generic;

public class XPromoAction
{
	public string actionType;

	public string target;

	public XPromoAction(string a, string t)
	{
		actionType = a;
		target = t;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		try
		{
			dictionary.Add("ActionType", actionType);
			dictionary.Add("Target", target);
			return dictionary;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Exception hoover in XPromoAction" + ex.Message);
			dictionary.Add("Error", ex.Message);
			return dictionary;
		}
	}
}
