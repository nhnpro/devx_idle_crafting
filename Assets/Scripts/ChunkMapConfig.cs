using System.Collections.Generic;
using System.Linq;

public class ChunkMapConfig
{
	private List<int[]> ChunkMap;

	public void SetChunkMap(List<int[]> chunkMap)
	{
		ChunkMap = chunkMap;
	}

	public List<WeightedObject<int[]>> ChunkMapClone()
	{
		return (from spot in ChunkMap
			select new WeightedObject<int[]>
			{
				Value = new int[4]
				{
					spot[0],
					spot[1],
					spot[2],
					spot[3]
				},
				Weight = 1f
			}).ToList();
	}
}
