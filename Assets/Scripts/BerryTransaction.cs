using System.Collections.Generic;

public class BerryTransaction
{
	public int amount;

	public int heroIndex;

	public string reason;

	public long lifetimeBerries;

	public BerryTransaction(int a, int h, string r)
	{
		amount = a;
		heroIndex = h;
		reason = r;
		lifetimeBerries = PlayerData.Instance.LifetimeBerries.Value;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Amount", amount.ToString());
		dictionary.Add("HeroIndex", heroIndex.ToString());
		dictionary.Add("Reason", reason);
		dictionary.Add("Lifetime_Berries", lifetimeBerries.ToString());
		return dictionary;
	}
}
