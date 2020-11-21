using System.Collections.Generic;
using System.Linq;

public static class ChunkMapParser
{
	public static ChunkMapConfig ParseChunkMap(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text);
		List<int[]> list = new List<int[]>();
		for (int i = 0; i < source.Count(); i++)
		{
			for (int j = 0; j < source.ElementAt(i).Length; j++)
			{
				int num = source.ElementAt(i).asInt(j, source.ElementAt(i).toError<int>());
				if (num > 0)
				{
					list.Add(new int[4]
					{
						j - 5,
						0,
						8 - i,
						num
					});
				}
			}
		}
		ChunkMapConfig chunkMapConfig = new ChunkMapConfig();
		chunkMapConfig.SetChunkMap(list);
		return chunkMapConfig;
	}
}
