using System.Collections.Generic;
using System.Linq;

public static class XPromoParser
{
	public static List<XPromoConfig> ParseXpromo(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			string id = line.asString(0, line.toError<string>());
			string appName = line.asString(1, line.toError<string>());
			string iosApp = line.asString(2, line.toError<string>());
			string pkgId = line.asString(3, line.toError<string>());
			string launchUrl = line.asString(4, line.toError<string>());
			string webUrl = line.asString(5, line.toError<string>());
			string appsFlyerUrl = line.asString(6, line.toError<string>());
			return new XPromoConfig(id, appName, iosApp, pkgId, launchUrl, webUrl, appsFlyerUrl);
		}).ToList();
	}
}
