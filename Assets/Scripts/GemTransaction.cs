using System.Collections.Generic;

public class GemTransaction
{
	public int amount;

	public string transaction;

	public string reason;

	public string type;

	public int chunk;

	public GemTransaction(int a, string t, string r, string ty, int c)
	{
		amount = a;
		transaction = t;
		reason = r;
		type = ty;
		chunk = c;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Amount", amount.ToString());
		dictionary.Add("Transaction", transaction);
		dictionary.Add("Reason", reason);
		dictionary.Add("Type", type);
		dictionary.Add("Current_Chunk", chunk.ToString());
		return dictionary;
	}
}
