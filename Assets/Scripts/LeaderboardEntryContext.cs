using System.Collections.Generic;

public class LeaderboardEntryContext : PropertyContext
{
	private LeaderboardEntry m_entry;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("LeaderboardEntry"))
		{
			m_entry = (parameters["LeaderboardEntry"] as LeaderboardEntry);
		}
		Add("LeaderboardEntry", m_entry);
	}
}
