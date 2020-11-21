using System.Collections.Generic;
using UnityEngine;

public class HeroEvolveRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_heroIndex;

	public int HeroIndex => m_heroIndex;

	public HeroEvolveRunner EvolveRunner
	{
		get;
		private set;
	}

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("HeroIndex"))
		{
			m_heroIndex = (int)parameters["HeroIndex"];
		}
		HeroRunner orCreateHeroRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex);
		EvolveRunner = new HeroEvolveRunner(orCreateHeroRunner);
		Add("HeroEvolveRunner", EvolveRunner);
	}

	public void OnAnimate()
	{
		EvolveRunner.Animate();
	}
}
