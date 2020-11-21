public static class HeroStateFactory
{
	public static HeroState GetOrCreateHeroState(int hero)
	{
		PlayerData.Instance.HeroStates.EnsureSize(hero, (int _) => new HeroState());
		return PlayerData.Instance.HeroStates[hero];
	}
}
