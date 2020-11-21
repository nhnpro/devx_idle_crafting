using System;
using System.Collections.Generic;
using System.Linq;

public class AdHistory
{
	public long TimeStamp
	{
		get;
		private set;
	}

	public AdPlacement Placement
	{
		get;
		private set;
	}

	public AdHistory(long time, AdPlacement placement)
	{
		TimeStamp = time;
		Placement = placement;
	}

	public static List<AdHistory> JSONToAdHistories(JSONObject obj)
	{
		return (from o in obj.list
			select new AdHistory(o.asCustom("Time", (JSONObject f) => long.Parse(f.str), () => 0L), o.asCustom("Placement", (JSONObject f) => ParsePlacement(f.str), () => AdPlacement.None))).ToList();
	}

	public static AdPlacement ParsePlacement(string placement)
	{
		try
		{
			return (AdPlacement)Enum.Parse(typeof(AdPlacement), placement);
		}
		catch (Exception)
		{
			return AdPlacement.None;
		}
	}

	public static long OldestThatCounts()
	{
		return new DateTime(ServerTimeService.NowTicks()).Subtract(TimeSpan.FromHours(24.0)).Ticks;
	}
}
