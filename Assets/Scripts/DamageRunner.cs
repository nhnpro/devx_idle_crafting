using Big;

public class DamageRunner : Singleton<DamageRunner>
{
	public void HeroHit(int heroIndex, IBlock block)
	{
		if (block != null)
		{
			HeroDamageRunner orCreateHeroDamageRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(heroIndex);
			BigDouble damage = orCreateHeroDamageRunner.RequestHeroDamage();
			Singleton<ChunkRunner>.Instance.CauseAoeDamage(damage, block.Position(), 1f);
		}
	}
}
