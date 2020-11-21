using Big;
using System.Collections.Generic;
using UnityEngine;

public class HeroTeamRunner : Singleton<HeroTeamRunner>
{
	private List<HeroRunner> m_heroRunners = new List<HeroRunner>();

	private List<HeroCostRunner> m_heroCostRunners = new List<HeroCostRunner>();

	private List<HeroDamageRunner> m_heroDamageRunners = new List<HeroDamageRunner>();

	public IEnumerable<HeroRunner> Companions()
	{
		for (int i = 1; i < Singleton<EconomyHelpers>.Instance.GetNumHeroes(); i++)
		{
			yield return GetOrCreateHeroRunner(i);
		}
	}

	public IEnumerable<HeroCostRunner> Costs()
	{
		for (int i = 1; i < Singleton<EconomyHelpers>.Instance.GetNumHeroes(); i++)
		{
			yield return GetOrCreateHeroCostRunner(i);
		}
	}

	public HeroRunner GetOrCreateHeroRunner(int hero)
	{
		m_heroRunners.EnsureSize(hero, (int count) => new HeroRunner(count));
		return m_heroRunners[hero];
	}

	public HeroCostRunner GetOrCreateHeroCostRunner(int hero)
	{
		m_heroCostRunners.EnsureSize(hero, (int count) => new HeroCostRunner(count));
		return m_heroCostRunners[hero];
	}

	public HeroDamageRunner GetOrCreateHeroDamageRunner(int hero)
	{
		m_heroDamageRunners.EnsureSize(hero, (int count) => new HeroDamageRunner(count));
		return m_heroDamageRunners[hero];
	}

	public BigDouble GetTeamDamage(int startIndex = 0)
	{
		BigDouble bigDouble = BigDouble.ZERO;
		for (int i = Mathf.Min(startIndex, Singleton<EconomyHelpers>.Instance.GetNumHeroes()); i < Singleton<EconomyHelpers>.Instance.GetNumHeroes(); i++)
		{
			bigDouble += GetOrCreateHeroDamageRunner(i).Damage.Value;
		}
		return bigDouble;
	}
}
