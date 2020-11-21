using System.Collections.Generic;
using UniRx;

[PropertyClass]
public class GearCollectionRunner : Singleton<GearCollectionRunner>
{
	private List<GearRunner> m_gearRunners = new List<GearRunner>();

	private List<GearViewRunner> m_gearViewRunners = new List<GearViewRunner>();

	private List<BlockResourceRunner> m_blockResourceRunners = new List<BlockResourceRunner>();

	private BlockResourceRunner m_relicResourceRunner;

	public ReactiveProperty<int> GearLevels;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> GearUpgradeAvailable;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> NoUpgradeAvailableAfterPrestige;

	public GearCollectionRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		GearUpgradeAvailable = CreateUpgradeObservable().TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		NoUpgradeAvailableAfterPrestige = CreateUpgradeAfterPrestigeObservable().TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		GearLevels = CreateGearsLevelObservable().TakeUntilDestroy(instance).ToReactiveProperty();
		(from lvls in GearLevels
			where lvls > PlayerData.Instance.LifetimeGearLevels.Value
			select lvls).Subscribe(delegate(int lvls)
		{
			PlayerData.Instance.LifetimeGearLevels.Value = lvls;
		}).AddTo(instance);
	}

	private UniRx.IObservable<bool> CreateUpgradeObservable()
	{
		UniRx.IObservable<bool> observable = Observable.Return<bool>(value: false);
		foreach (GearRunner item in Gears())
		{
			observable = observable.CombineLatest(item.UpgradeAvailable, (bool old, bool current) => current || old);
		}
		return observable;
	}

	private UniRx.IObservable<bool> CreateUpgradeAfterPrestigeObservable()
	{
		UniRx.IObservable<bool> observable = Observable.Return<bool>(value: true);
		foreach (GearRunner item in Gears())
		{
			observable = observable.CombineLatest(item.UpgradeAfterPrestigeAvailable, (bool old, bool current) => old && !current);
		}
		return observable;
	}

	private UniRx.IObservable<int> CreateGearsLevelObservable()
	{
		UniRx.IObservable<int> observable = Observable.Return(0);
		foreach (GearRunner item in Gears())
		{
			observable = observable.CombineLatest(item.Level, (int old, int current) => old + current);
		}
		return observable;
	}

	public IEnumerable<GearRunner> Gears()
	{
		for (int i = 0; i < PersistentSingleton<Economies>.Instance.Gears.Count; i++)
		{
			yield return GetOrCreateGearRunner(i);
		}
	}

	public GearRunner GetOrCreateGearRunner(int gear)
	{
		m_gearRunners.EnsureSize(gear, (int count) => new GearRunner(count));
		return m_gearRunners[gear];
	}

	public GearViewRunner GetOrCreateGearViewRunner(int gear)
	{
		m_gearViewRunners.EnsureSize(gear, (int count) => new GearViewRunner(count));
		return m_gearViewRunners[gear];
	}

	public BlockResourceRunner GetOrCreateBlockResourceRunner(BlockType type)
	{
		m_blockResourceRunners.EnsureSize((int)type, (int count) => new BlockResourceRunner((BlockType)count));
		return m_blockResourceRunners[(int)type];
	}

	public static UniRx.IObservable<CraftingRequirement> CreateBlocksObservable()
	{
		return PlayerData.Instance.BlocksCollected[6].CombineLatest(PlayerData.Instance.BlocksCollected[0], PlayerData.Instance.BlocksCollected[1], PlayerData.Instance.BlocksCollected[2], PlayerData.Instance.BlocksCollected[3], PlayerData.Instance.BlocksCollected[4], PlayerData.Instance.BlocksCollected[5], delegate(long relics, long grass, long dirt, long wood, long stone, long metal, long gold)
		{
			CraftingRequirement craftingRequirement = new CraftingRequirement();
			craftingRequirement.Resources[6] = relics;
			craftingRequirement.Resources[0] = grass;
			craftingRequirement.Resources[1] = dirt;
			craftingRequirement.Resources[2] = wood;
			craftingRequirement.Resources[3] = stone;
			craftingRequirement.Resources[4] = metal;
			craftingRequirement.Resources[5] = gold;
			return craftingRequirement;
		});
	}

	public static UniRx.IObservable<CraftingRequirement> CreateAfterPrestigeBlocksObservable()
	{
		return Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[0].CombineLatest(Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[1], Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[2], Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[3], Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[4], Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[5], Singleton<AfterPrestigeFundCollector>.Instance.FundsAfterPrestige[6], delegate(long grass, long dirt, long wood, long stone, long metal, long gold, long relics)
		{
			CraftingRequirement craftingRequirement = new CraftingRequirement();
			craftingRequirement.Resources[0] = grass;
			craftingRequirement.Resources[1] = dirt;
			craftingRequirement.Resources[2] = wood;
			craftingRequirement.Resources[3] = stone;
			craftingRequirement.Resources[4] = metal;
			craftingRequirement.Resources[5] = gold;
			craftingRequirement.Resources[6] = relics;
			return craftingRequirement;
		});
	}
}
