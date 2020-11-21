using System;
using System.Collections.Generic;
using System.Linq;

public class AdLoadRequest
{
	public AdProvider provider;

	public IEnumerable<AdProvider> alreadyTriedProviders;

	public long timeStampSinceNoAds;

	public Dictionary<string, string> asDictionary(long now)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("SecondsWithoutAds", Math.Round(TimeSpan.FromTicks(now - timeStampSinceNoAds).TotalSeconds).ToString());
		dictionary.Add("Provider", provider.NetworkId.ToString());
		List<AdProvider> list = alreadyTriedProviders.ToList();
		for (int i = 0; i < list.Count; i++)
		{
			dictionary.Add("Previous_Provider_" + i, list[i].NetworkId.ToString());
		}
		return dictionary;
	}
}
