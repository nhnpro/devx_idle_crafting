using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CreatureCollectionRunner : Singleton<CreatureCollectionRunner>
{
	private List<AbstractCreatureController> m_controllers = new List<AbstractCreatureController>();

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		GetOrCreateCreature(0);
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigePost
			select order).Subscribe(delegate
		{
			ResetCreatures();
		}).AddTo(instance);
	}

	public bool CollidesAnyAliveExcept(Vector3 pos, int heroIndex)
	{
		for (int i = 0; i < Singleton<EconomyHelpers>.Instance.GetNumHeroes(); i++)
		{
			if (i != heroIndex)
			{
				HeroRunner orCreateHeroRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(i);
				if (!orCreateHeroRunner.Found.Value)
				{
					return false;
				}
				AbstractCreatureController orCreateCreature = GetOrCreateCreature(i);
				if (orCreateCreature.Collides(pos))
				{
					return true;
				}
			}
		}
		return false;
	}

	public AbstractCreatureController GetOrCreateCreature(int heroIndex)
	{
		m_controllers.EnsureSize(heroIndex, (int count) => CreateCreature(count));
		return m_controllers[heroIndex];
	}

	private AbstractCreatureController CreateCreature(int heroIndex)
	{
		GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources(GetPrefabPath(heroIndex), Vector3.zero, Quaternion.identity, null);
		AbstractCreatureController componentInChildren = gameObject.GetComponentInChildren<AbstractCreatureController>();
		componentInChildren.Init(heroIndex, CreatureStateEnum.Enter);
		return componentInChildren;
	}

	private void ResetCreatures()
	{
		foreach (AbstractCreatureController controller in m_controllers)
		{
			UnityEngine.Object.DestroyImmediate(controller.gameObject);
		}
		m_controllers.Clear();
		GetOrCreateCreature(0);
	}

	private string GetPrefabPath(int heroIndex)
	{
		int b = PersistentSingleton<Economies>.Instance.Heroes.Count - 1;
		return "Creatures/Creature" + Mathf.Min(heroIndex, b);
	}
}
