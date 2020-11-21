using System;
using System.Collections.Generic;
using System.Linq;

public class NetworkConfigParser
{
	public static List<ZoneAndFloor> ParseFacebookFloors()
	{
		return parseAdNetworkFloors(AdNetwork.Facebook);
	}

	public static List<ZoneAndFloor> ParseAdMobFloors()
	{
		return parseAdNetworkFloors(AdNetwork.AdMob);
	}

	public static List<ZoneAndFloor> ParseUnityFloors()
	{
		return parseAdNetworkFloors(AdNetwork.Unity);
	}

	public static List<ZoneAndFloor> ParseAdColonyFloors()
	{
		return parseAdNetworkFloors(AdNetwork.AdColony);
	}

	public static List<ZoneAndFloor> parseAdNetworkFloors(AdNetwork network)
	{
		IEnumerable<string[]> source = TSVParser.Parse(PersistentSingleton<StringCache>.Instance.Get("Ads/network_floors"));
		string[] array = source.First();
		int column = Array.IndexOf(array, network.ToString());
		if (column < 0)
		{
			return new List<ZoneAndFloor>();
		}
		return (from r in source.Skip(1)
			where r.nonEmpty(column)
			select r into line
			select new ZoneAndFloor
			{
				Zone = line.asString(0, line.toError<string>()),
				Floor = line.asInt(column, line.toError<int>())
			}).ToList();
	}

	public static List<ZoneMapping> ParseFacebookZoneMappings()
	{
		return parseZoneMapping("Ads/facebook_zones_android");
	}

	public static List<ZoneMapping> ParseAdMobZoneMappings()
	{
		return parseZoneMapping("Ads/admob_zones_android");
	}

	public static List<ZoneMapping> ParseUnityZoneMappings()
	{
		return parseZoneMapping("Ads/unity_zones_android");
	}

	public static List<ZoneMapping> parseZoneMapping(string configName)
	{
		IEnumerable<string[]> source = TSVParser.Parse(PersistentSingleton<StringCache>.Instance.Get(configName)).Skip(1);
		return (from line in source
			select new ZoneMapping
			{
				Zone = line.asString(0, line.toError<string>()),
				Id = line.asString(1, line.toError<string>())
			}).ToList();
	}

	public static List<ZoneData> ParseZoneDatas(List<ZoneMapping> zoneMappings, List<ZoneAndFloor> floors)
	{
		return (from z in floors
			where zoneMappings.Any((ZoneMapping m) => m.Zone == z.Zone) && z.Floor > 0
			select new ZoneData
			{
				AdUnit = zoneMappings.First((ZoneMapping m) => m.Zone == z.Zone).Id,
				Floor = z.Floor,
				Zone = z.Zone
			}).ToList();
	}

	public static List<ZoneData> ParseUnityZoneData()
	{
		return ParseZoneDatas(ParseUnityZoneMappings(), ParseUnityFloors());
	}

	public static List<ZoneData> ParseAdMobZoneData()
	{
		return ParseZoneDatas(ParseAdMobZoneMappings(), ParseAdMobFloors());
	}
}
