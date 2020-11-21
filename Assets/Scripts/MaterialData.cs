public class MaterialData
{
	public BlockType Type
	{
		get;
		private set;
	}

	public int StartingPoint
	{
		get;
		private set;
	}

	public MaterialData(BlockType type, int startingPoint)
	{
		Type = type;
		StartingPoint = startingPoint;
	}
}
