using System.Collections.Generic;
using UnityEngine;

public class GearSetRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_setIndex;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("GearSetIndex"))
		{
			m_setIndex = (int)parameters["GearSetIndex"];
		}
		Add("GearSetRunner", Singleton<GearSetCollectionRunner>.Instance.GetOrCreateGearSetRunner(m_setIndex));
	}
}
