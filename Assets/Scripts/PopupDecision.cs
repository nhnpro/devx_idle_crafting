using System.Collections.Generic;

public class PopupDecision
{
	public string popup;

	public string decision;

	public PopupDecision(string p, string d)
	{
		popup = p;
		decision = d;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Popup", popup);
		dictionary.Add("Decision", decision);
		return dictionary;
	}
}
