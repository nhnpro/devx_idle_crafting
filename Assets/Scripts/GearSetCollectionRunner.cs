using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class GearSetCollectionRunner : Singleton<GearSetCollectionRunner>
{
	private List<GearSetRunner> m_gearSetRunners = new List<GearSetRunner>();

	public ReadOnlyReactiveProperty<int> MaxSetsToShow;

	public ReadOnlyReactiveProperty<List<int>> SetUnlocked;

	[PropertyInt]
	public ReadOnlyReactiveProperty<int> NextSetUnlockAt;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AllSetsUnlocked;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AllAvailableGearsUnlocked;

	public ReadOnlyReactiveProperty<float>[] BonusMult = new ReadOnlyReactiveProperty<float>[25];

	public GearSetCollectionRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		for (int i = 0; i < 25; i++)
		{
			BonusMult[i] = CreateObservableForAll((BonusTypeEnum)i).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		}
		ReactiveProperty<int> mainChunk = PlayerData.Instance.LifetimeChunk;
		MaxSetsToShow = (from lvl in mainChunk
			select GetMaxSetsToShow(lvl)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SetUnlocked = (from pair in MaxSetsToShow.Pairwise()
			where pair.Previous < pair.Current
			select pair into _
			select GetUnlockedGears(mainChunk.Value)).Do(delegate
		{
			BindingManager.Instance.HeroLevelParent.ShowInfo();
		}).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		NextSetUnlockAt = (from lvl in mainChunk
			select GetNextSetUnlockAt(lvl)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		AllSetsUnlocked = (from lvl in mainChunk
			select IsAllSetsUnlocked(lvl)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		AllAvailableGearsUnlocked = (from unlockAvailable in (from lvl in mainChunk
				select GetMaxSetsToShow(lvl)).CombineLatest(PlayerData.Instance.LifetimeGears, (int maxSets, int unlocks) => maxSets * 3 > unlocks)
			select !unlockAvailable).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void DebugGiveGear(int gear)
	{
		GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gear);
		orCreateGearRunner.Level.Value++;
	}

	private List<int> GetUnlockedGears(int mainChunk)
	{
		List<GearRunner> list = Singleton<GearCollectionRunner>.Instance.Gears().ToList();
		List<int> list2 = new List<int>();
		foreach (GearRunner item in list)
		{
			if (item.SetIndex < MaxSetsToShow.Value && item.SetIndex >= GetMaxSetsToShow(mainChunk - 1))
			{
				list2.Add(item.GearIndex);
			}
		}
		return list2;
	}

	private UniRx.IObservable<float> CreateObservableForAll(BonusTypeEnum bonusType)
	{
		List<UniRx.IObservable<float>> multipliers = (from config in PersistentSingleton<Economies>.Instance.GearSets
			where config.Bonus.BonusType == bonusType
			select CreateGearSetBonusMultiplier(bonusType, config.SetIndex, config.Bonus.Amount)).ToList();
		return BonusTypeHelper.CreateCombine(bonusType, multipliers);
	}

	private UniRx.IObservable<float> CreateGearSetBonusMultiplier(BonusTypeEnum bonusType, int setIndex, float mult)
	{
		GearSetRunner orCreateGearSetRunner = GetOrCreateGearSetRunner(setIndex);
		return from level in orCreateGearSetRunner.MaxAllLevel
			select (level < 1) ? BonusTypeHelper.GetOrigin(bonusType) : mult;
	}

	public IEnumerable<GearSetRunner> GearSets()
	{
		for (int i = 0; i < PersistentSingleton<Economies>.Instance.Gears.Count / 3; i++)
		{
			yield return GetOrCreateGearSetRunner(i);
		}
	}

	public GearSetRunner GetOrCreateGearSetRunner(int gear)
	{
		m_gearSetRunners.EnsureSize(gear, (int count) => new GearSetRunner(count));
		return m_gearSetRunners[gear];
	}

	private int GetMaxSetsToShow(int mainChunk)
	{
		List<GearSet> gearSets = PersistentSingleton<Economies>.Instance.GearSets;
		for (int i = 0; i < gearSets.Count; i++)
		{
			if (gearSets[i].ChunkUnlockLevel > mainChunk)
			{
				return i;
			}
		}
		return gearSets.Count;
	}

	private int GetNextSetUnlockAt(int mainChunk)
	{
		List<GearSet> gearSets = PersistentSingleton<Economies>.Instance.GearSets;
		for (int i = 0; i < gearSets.Count; i++)
		{
			if (gearSets[i].ChunkUnlockLevel > mainChunk)
			{
				return gearSets[i].ChunkUnlockLevel;
			}
		}
		return -1;
	}

	private bool IsAllSetsUnlocked(int mainChunk)
	{
		return GetMaxSetsToShow(mainChunk) == PersistentSingleton<Economies>.Instance.GearSets.Count;
	}

	public GearRunner GetRandomUnlockableGear(List<GearSetRunner> gearSetRunners)
	{
		List<GearRunner> list = new List<GearRunner>();
		foreach (GearSetRunner gearSetRunner in gearSetRunners)
		{
			foreach (GearRunner gearRunner in gearSetRunner.GearRunners)
			{
				if (gearRunner.Level.Value == 0)
				{
					list.Add(gearRunner);
				}
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return null;
	}

	public GearRunner GetRandomHighestGear(List<GearSetRunner> gearSetRunners)
	{
		List<GearRunner> list = new List<GearRunner>();
		foreach (GearSetRunner gearSetRunner in gearSetRunners)
		{
			foreach (GearRunner gearRunner in gearSetRunner.GearRunners)
			{
				if (!gearRunner.MaxLevelReached.Value)
				{
					list.Add(gearRunner);
				}
			}
		}
		int num = 0;
		foreach (GearRunner item in list)
		{
			if (item.Level.Value > num)
			{
				num = item.Level.Value;
			}
		}
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if (list[num2].Level.Value != num)
			{
				list.Remove(list[num2]);
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return null;
	}
}
