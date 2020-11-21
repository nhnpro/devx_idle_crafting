using System.Collections.Generic;
using System.Linq;

public static class CraftingParser
{
	public static List<CraftingConfig> ParseCrafting(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			CraftingConfig craftingConfig = new CraftingConfig
			{
				Relics = line.asLong(1, line.toError<long>())
			};
			craftingConfig.Materials[0] = line.asLong(2, line.toError<long>());
			craftingConfig.Materials[1] = line.asLong(3, line.toError<long>());
			craftingConfig.Materials[2] = line.asLong(4, line.toError<long>());
			craftingConfig.Materials[3] = line.asLong(5, line.toError<long>());
			craftingConfig.Materials[4] = line.asLong(6, line.toError<long>());
			craftingConfig.Materials[5] = line.asLong(7, line.toError<long>());
			return craftingConfig;
		}).ToList();
	}
}
