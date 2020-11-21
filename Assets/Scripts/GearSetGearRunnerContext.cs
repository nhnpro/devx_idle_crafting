using System.Collections.Generic;
using UnityEngine;

public class GearSetGearRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_gearSetGearIndex;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("GearSetGearIndex"))
		{
			m_gearSetGearIndex = (int)parameters["GearSetGearIndex"];
		}
		GearSetRunner gearSetRunner = (GearSetRunner)Singleton<PropertyManager>.Instance.GetContext("GearSetRunner", base.transform);
		GearRunner gearRunner = gearSetRunner.GearRunners[m_gearSetGearIndex];
		Add("GearRunner", gearRunner);
		Add("GearViewRunner", Singleton<GearCollectionRunner>.Instance.GetOrCreateGearViewRunner(gearRunner.GearIndex));
	}
}
