using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class DrJellyRunner : Singleton<DrJellyRunner>
{
	[PropertyInt]
	public ReadOnlyReactiveProperty<int> SpawningLevel;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DrJellyAppeared;

	[PropertyInt]
	public ReadOnlyReactiveProperty<int> DrJellyLevel;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> DrJellyBattle;

	public ReactiveProperty<int> DrJellyExitLevel = new ReactiveProperty<int>(-1);

	public ReactiveProperty<int> UnfrozenCompanionIndex = new ReactiveProperty<int>(0);

	public DrJellyRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SpawningLevel = (from lvl in PlayerData.Instance.DrJellySpawningLevel
			select (lvl)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrJellyAppeared = PlayerData.Instance.MainChunk.CombineLatest(PlayerData.Instance.DrJellySpawningLevel, (int chunk, int lvl) => chunk == lvl).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrJellyLevel = (from lvl in PlayerData.Instance.DrJellyLevel
			select (lvl)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrJellyBattle = PlayerData.Instance.MainChunk.CombineLatest(PlayerData.Instance.DrJellyLevel, (int chunk, int lvl) => chunk == lvl).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		DrJellyLevel.Subscribe(delegate
		{
			ForceJellyHeliAsBiome();
		}).AddTo(instance);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		PlayerData.Instance.MainChunk.Subscribe(delegate(int chunk)
		{
			RefreshDrJellyLevels(chunk);
		}).AddTo(instance);
		(from drActive in DrJellyBattle.CombineLatest(Singleton<BossBattleRunner>.Instance.BossBattleActive, (bool dr, bool active) => dr && active)
			where drActive
			select drActive).Do(delegate
		{
			Singleton<BossBattleRunner>.Instance.PauseBossBattle();
		}).Delay(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
		{
			Singleton<BossBattleRunner>.Instance.UnpauseBossBattle();
		})
			.AddTo(instance);
		(from battle in DrJellyBattle.Pairwise()
			where !battle.Current && battle.Previous
			select battle).Subscribe(delegate
		{
			DrJellyExitLevel.Value = DrJellyLevel.Value;
		}).AddTo(instance);
	}

	private void ForceJellyHeliAsBiome()
	{
		if (PlayerData.Instance.MainChunk.Value <= DrJellyLevel.Value)
		{
			PersistentSingleton<Economies>.Instance.Biomes.Last((BiomeConfig bd) => bd.Chunk <= DrJellyLevel.Value).BiomeIndex = 3;
		}
	}

	private void RefreshDrJellyLevels(int chunk)
	{
		if (chunk == 0 || PlayerData.Instance.DrJellySpawningLevel.Value < 0 || (DrJellyLevel.Value > 0 && chunk > DrJellyLevel.Value))
		{
			PlayerData.Instance.DrJellySpawningLevel.Value = GetSpawningLevel(chunk);
			PlayerData.Instance.DrJellyLevel.Value = -1;
		}
		if (chunk == SpawningLevel.Value && DrJellyLevel.Value < chunk)
		{
			PlayerData.Instance.DrJellyLevel.Value = GetDrJellyLevel(chunk);
		}
	}

	private int GetSpawningLevel(int chunk)
	{
		int num = chunk + UnityEngine.Random.Range(PersistentSingleton<GameSettings>.Instance.DrJellySpawnAfterMin, PersistentSingleton<GameSettings>.Instance.DrJellySpawnAfterMax);
		if (PlayerData.Instance.LifetimePrestiges.Value == 0 && PlayerData.Instance.MainChunk.Value < 40)
		{
			num = 37;
		}
		if (num % 10 == 0)
		{
			num++;
		}
		return num;
	}

	private int GetDrJellyLevel(int startChunk)
	{
		int num = Mathf.FloorToInt((float)startChunk / 10f) + 1;
		List<int> list = new List<int>();
		for (int i = 0; i < 3; i++)
		{
			if (CheckIfNodeIsEmpty(num + i))
			{
				list.Add(num + i);
			}
		}
		if (list.Count == 0)
		{
			for (int j = 3; j < 6; j++)
			{
				if (CheckIfNodeIsEmpty(num + j))
				{
					list.Add(num + j);
				}
			}
		}
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)] * 10 + 9;
		}
		return -1;
	}

	private bool CheckIfNodeIsEmpty(int node)
	{
		int newCreatureInRangeOrNone = Singleton<EconomyHelpers>.Instance.GetNewCreatureInRangeOrNone(PlayerData.Instance.MainChunk.Value, node * 10, (node + 1) * 10 - 1);
		if (newCreatureInRangeOrNone != -1)
		{
			return false;
		}
		int newGearSetInRangeOrNone = Singleton<EconomyHelpers>.Instance.GetNewGearSetInRangeOrNone(PlayerData.Instance.LifetimeChunk.Value, node * 10, (node + 1) * 10 - 1);
		if (newGearSetInRangeOrNone != -1)
		{
			return false;
		}
		int num = Singleton<PrestigeRunner>.Instance.PrestigeRequirement.Value - 1;
		if (num >= node * 10 && num < (node + 1) * 10)
		{
			return false;
		}
		return true;
	}
}
