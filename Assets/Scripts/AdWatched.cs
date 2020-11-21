using System.Collections.Generic;

public class AdWatched
{
	public AdService.V2PShowResult result;

	public AdNetwork network;

	public AdPlacement placement;

	public string zone;

	public int floorValue;

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Placement", placement.ToString());
		dictionary.Add("Network", network.ToString());
		dictionary.Add("Result", result.ToString());
		if (zone != null)
		{
			dictionary.Add("Zone", zone);
		}
		if (floorValue > 0)
		{
			dictionary.Add("Floor", floorValue.ToString());
		}
		return dictionary;
	}
}
