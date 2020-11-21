public class PerkUnlockedInfo
{
	public int HeroIndex
	{
		get;
		private set;
	}

	public int PerkIndex
	{
		get;
		private set;
	}

	public PerkUnlockedInfo(int hero, int perk)
	{
		HeroIndex = hero;
		PerkIndex = perk;
	}
}
