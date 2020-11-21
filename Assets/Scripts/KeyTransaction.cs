using System.Collections.Generic;

public class KeyTransaction
{
	public int amount;

	public string reason;

	public int lifetimeKeys;

	public KeyTransaction(int a, string r)
	{
		amount = a;
		reason = r;
		lifetimeKeys = PlayerData.Instance.LifetimeKeys.Value;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Amount", amount.ToString());
		dictionary.Add("Reason", reason);
		dictionary.Add("Lifetime_Keys", lifetimeKeys.ToString());
		return dictionary;
	}
}
