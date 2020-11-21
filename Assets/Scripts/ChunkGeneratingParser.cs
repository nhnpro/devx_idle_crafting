using System.Collections.Generic;
using System.Linq;

public static class ChunkGeneratingParser
{
	public static List<ChunkGeneratingConfig> ParseChunkGeneratings(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			ChunkGeneratingConfig chunkGeneratingConfig = new ChunkGeneratingConfig
			{
				Chunk = line.asInt(0, line.toError<int>()),
				MaxBlocks = line.asInt(1, line.toError<int>()),
				FourBlockMin = line.asInt(2, line.toError<int>()),
				FourBlockMax = line.asInt(3, line.toError<int>()),
				TwoBlockMin = line.asInt(4, line.toError<int>()),
				TwoBlockMax = line.asInt(5, line.toError<int>())
			};
			List<WeightedObject<BlockType>> list = new List<WeightedObject<BlockType>>();
			for (int i = 6; i < line.Length - 7; i++)
			{
				BlockType value = (BlockType)(i - 6);
				WeightedObject<BlockType> item = new WeightedObject<BlockType>
				{
					Value = value,
					Weight = line.asFloat(i, line.toError<float>())
				};
				list.Add(item);
			}
			chunkGeneratingConfig.Materials = list;
			chunkGeneratingConfig.GoldBlockAverage = line.asInt(11, line.toError<int>());
			chunkGeneratingConfig.DiamondMin = line.asInt(12, line.toError<int>());
			chunkGeneratingConfig.DiamondMax = line.asInt(13, line.toError<int>());
			chunkGeneratingConfig.DiamondChance = line.asFloat(14, line.toError<float>());
			chunkGeneratingConfig.TNTMin = line.asInt(15, line.toError<int>());
			chunkGeneratingConfig.TNTMax = line.asInt(16, line.toError<int>());
			chunkGeneratingConfig.TNTChance = line.asFloat(17, line.toError<float>());
			return chunkGeneratingConfig;
		}).ToList();
	}
}
