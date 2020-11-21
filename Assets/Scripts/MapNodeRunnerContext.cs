using System.Collections.Generic;
using UnityEngine;

public class MapNodeRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_nodeOffset;

	private MapNodeRunner m_runner;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("NodeOffset"))
		{
			m_nodeOffset = (int)parameters["NodeOffset"];
		}
		int num = (int)Singleton<PropertyManager>.Instance.GetContext("Chapter", base.transform);
		m_runner = Singleton<MapRunner>.Instance.GetOrCreateMapNodeRunner(num * 4 + m_nodeOffset);
		Add("MapNodeRunner", m_runner);
	}
}
