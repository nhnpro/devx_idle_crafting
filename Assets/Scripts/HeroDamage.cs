using Big;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct HeroDamage
{
	public BigDouble Damage
	{
		get;
		private set;
	}

	public BlockController Block
	{
		get;
		private set;
	}

	public HeroDamage(BigDouble damage, BlockController block)
	{
		Damage = damage;
		Block = block;
	}
}
