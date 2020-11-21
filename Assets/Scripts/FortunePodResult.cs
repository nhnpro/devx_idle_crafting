using System.Collections.Generic;

public class FortunePodResult
{
	public bool claimedRewards;

	public int fails;

	public int level;

	public FortunePodResult(bool cr, int f, int l)
	{
		claimedRewards = cr;
		fails = f;
		level = l;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ClaimedRewards", claimedRewards.ToString());
		dictionary.Add("Fails", fails.ToString());
		dictionary.Add("Level", level.ToString());
		return dictionary;
	}
}
