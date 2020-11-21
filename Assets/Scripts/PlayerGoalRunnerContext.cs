using System.Collections.Generic;
using UnityEngine;

public class PlayerGoalRunnerContext : PropertyContext
{
	private PlayerGoalRunner m_runner;

	public override void Install(Dictionary<string, object> parameters)
	{
		m_runner = (parameters["PlayerGoalRunner"] as PlayerGoalRunner);
		Add("PlayerGoalRunner", m_runner);
	}

	public void OnClaim()
	{
		Vector3 spawnPoint = base.transform.position + new Vector3(3.5f, -1f, 0f);
		m_runner.Claim(spawnPoint);
	}
}
