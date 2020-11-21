using System.Collections.Generic;
using UnityEngine;

public class BlockResourceRunnerContext : PropertyContext
{
	[SerializeField]
	private BlockType m_type;

	private BlockResourceRunner m_runner;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (m_type < BlockType.NumCraftingTypes)
		{
			m_runner = Singleton<GearCollectionRunner>.Instance.GetOrCreateBlockResourceRunner(m_type);
		}
		else
		{
			m_runner = (parameters["BlockResourceRunner"] as BlockResourceRunner);
		}
		Add("BlockResourceRunner", m_runner);
	}
}
