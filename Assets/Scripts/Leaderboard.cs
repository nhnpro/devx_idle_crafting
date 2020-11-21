using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class Leaderboard
{
	public int Version;

	public List<LeaderboardEntry> Entries;

	[CompilerGenerated]
	private static Func<IGrouping<string, LeaderboardEntry>, LeaderboardEntry> _003C_003Ef__mg_0024cache0;

	public Leaderboard(int version, List<LeaderboardEntry> entries)
	{
		Version = version;
		Entries = entries;
	}

	public Leaderboard WithEntries(List<LeaderboardEntry> additionalEntries)
	{
		Entries.AddRange(additionalEntries);
		Entries = (from e in Entries
			group e by e.PlayerId.Value).Select(Enumerable.First<LeaderboardEntry>).ToList();
		return this;
	}
}
