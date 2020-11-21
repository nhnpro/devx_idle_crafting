using System.Collections.Generic;
using System.Linq;

public class FlooredProvidersFactory : PersistentSingleton<FlooredProvidersFactory>
{
	public List<FlooredAdProvider> FlooredAdProviders;

	public void Init()
	{
		if (!base.Inited)
		{
			FlooredAdProviders = Create();
			base.Inited = true;
		}
	}

	public List<FlooredAdProvider> Create()
	{
		List<FlooredAdProvider> list = new List<FlooredAdProvider>();
		addFacebookZones(list);
		addAdMobZones(list);
		addUnityZones(list);
		addAdColonyProvider(list);
		return list;
	}

	private void addAdColonyProvider(List<FlooredAdProvider> ps)
	{
		List<ZoneAndFloor> adColonyFloors = PersistentSingleton<Economies>.Instance.AdColonyFloors;
		if (adColonyFloors.Count != 0)
		{
			ZoneAndFloor zoneAndFloor = adColonyFloors[0];
			if (zoneAndFloor.Floor > 0)
			{
				addProviderDelegate(ps, PersistentSingleton<AdColonyProvider>.Instance, zoneAndFloor.Zone, zoneAndFloor.Floor);
			}
		}
	}

	private void addFacebookZones(List<FlooredAdProvider> ps)
	{
		List<ZoneMapping> zoneMappings = PersistentSingleton<Economies>.Instance.FacebookZoneMappings;
		(from z in PersistentSingleton<Economies>.Instance.FacebookFloors
			where zoneMappings.Any((ZoneMapping m) => m.Zone == z.Zone) && z.Floor > 0
			select z).ToList().ForEach(delegate(ZoneAndFloor z)
		{
			addProviderDelegate(ps, new FacebookProvider(zoneMappings.First((ZoneMapping m) => m.Zone == z.Zone).Id), z.Zone, z.Floor);
		});
	}

	private void addAdMobZones(List<FlooredAdProvider> ps)
	{
		List<ZoneData> adMobZoneData = PersistentSingleton<Economies>.Instance.AdMobZoneData;
		if (adMobZoneData.Count > 0)
		{
			AdMobZoneProvider adMobZoneProvider = new AdMobZoneProvider();
			adMobZoneProvider.Init();
			ps.Add(new AdMobFlooredProvider(adMobZoneProvider, adMobZoneData));
		}
	}

	private void addUnityZones(List<FlooredAdProvider> ps)
	{
		List<ZoneData> unityZoneData = PersistentSingleton<Economies>.Instance.UnityZoneData;
		if (unityZoneData.Count > 0)
		{
			UnityZoneProvider unityZoneProvider = new UnityZoneProvider(unityZoneData);
			unityZoneProvider.Init();
			ps.Add(unityZoneProvider);
		}
	}

	private void addProviderDelegate(List<FlooredAdProvider> ps, AdProvider provider, string zone, int floor)
	{
		provider.Init();
		ps.Add(new AdProviderDelegate(provider, zone, floor));
	}
}
