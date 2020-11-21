using System.Collections.Generic;

public class ChunkGeneratingConfig
{
	public int Chunk;

	public int MaxBlocks;

	public int FourBlockMin;

	public int FourBlockMax;

	public int TwoBlockMin;

	public int TwoBlockMax;

	public List<WeightedObject<BlockType>> Materials;

	public int GoldBlockAverage;

	public int DiamondMin;

	public int DiamondMax;

	public float DiamondChance;

	public int TNTMin;

	public int TNTMax;

	public float TNTChance;
}
