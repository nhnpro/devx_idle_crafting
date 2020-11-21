using System.Collections.Generic;
using UnityEngine;

public class MapRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_chapter;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("Chapter"))
		{
			m_chapter = (int)parameters["Chapter"];
		}
		Add("Chapter", m_chapter);
	}
}
