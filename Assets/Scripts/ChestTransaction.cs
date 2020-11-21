using System.Collections.Generic;

public class ChestTransaction
{
	public int amount;

	public string transaction;

	public string reason;

	public int openedChests;

	public ChestTransaction(int a, string t, string r)
	{
		amount = a;
		transaction = t;
		reason = r;
		openedChests = PlayerData.Instance.LifetimeAllOpenedChests.Value;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Amount", amount.ToString());
		dictionary.Add("Transaction", transaction);
		dictionary.Add("Reason", reason);
		dictionary.Add("Lifetime_Opened_Chests", openedChests.ToString());
		return dictionary;
	}
}
