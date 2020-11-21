using System.Collections.Generic;
using System.Linq;

public static class CraftingMaterialParser
{
	public static List<CraftingMaterialConfig> ParseCraftingMaterial(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new CraftingMaterialConfig
			{
				GearIndex = line.asInt(0, line.toError<int>()),
				JellyStartingpoint = line.asInt(1, line.toError<int>()),
				Material1 = ParseMaterialOrNull(line[2]),
				Material2 = ParseMaterialOrNull(line[3]),
				Material3 = ParseMaterialOrNull(line[4])
			}).ToList();
	}

	private static MaterialData ParseMaterialOrNull(string raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return null;
		}
		string[] row = raw.Split("_".ToCharArray());
		BlockType type = row.asEnum(0, row.toError<BlockType>());
		int startingPoint = row.asInt(1, row.toError<int>());
		return new MaterialData(type, startingPoint);
	}
}
