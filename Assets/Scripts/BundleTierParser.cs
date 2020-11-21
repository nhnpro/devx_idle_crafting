using System.Collections.Generic;
using System.Linq;

public static class BundleTierParser
{
	public static List<BundleTierConfig> ParseBundleTiers(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new BundleTierConfig
			{
				ProductEnum = line.asEnum(0, line.toError<IAPProductEnum>()),
				BundleTier = line.asInt(1, line.toError<int>())
			}).ToList();
	}
}
