public class CraftingRequirement
{
	public long[] Resources = new long[7];

	public bool Satisfies(CraftingRequirement req)
	{
		for (int i = 0; i < Resources.Length; i++)
		{
			if (Resources[i] < req.Resources[i])
			{
				return false;
			}
		}
		return true;
	}
}
