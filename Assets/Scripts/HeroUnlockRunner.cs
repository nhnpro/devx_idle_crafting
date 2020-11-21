using UniRx;
using UnityEngine;

public class HeroUnlockRunner : Singleton<HeroUnlockRunner>
{
	public ReactiveProperty<int> NumUnlockedHeroes;

	public HeroUnlockRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		NumUnlockedHeroes = (from chunk in PlayerData.Instance.MainChunk
			select Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(chunk)).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		PlayerData.Instance.MainChunk.Subscribe(delegate(int index)
		{
			UnlockHeroes(index);
		}).AddTo(instance);
		(from unlocks in NumUnlockedHeroes
			where unlocks > PlayerData.Instance.LifetimeCreatures.Value
			select unlocks).Subscribe(delegate(int unlocks)
		{
			PlayerData.Instance.LifetimeCreatures.Value = unlocks;
		}).AddTo(instance);
	}

	private void UnlockHeroes(int chunkIndex)
	{
		for (int i = 0; i < Singleton<EconomyHelpers>.Instance.GetNumHeroes(); i++)
		{
			HeroRunner orCreateHeroRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(i);
			if (chunkIndex >= orCreateHeroRunner.ChunkIndex.Value)
			{
				orCreateHeroRunner.Level.Value = Mathf.Max(orCreateHeroRunner.Level.Value, 1);
				orCreateHeroRunner.Tier.Value = Mathf.Max(orCreateHeroRunner.Tier.Value, 1);
				Singleton<CreatureCollectionRunner>.Instance.GetOrCreateCreature(orCreateHeroRunner.HeroIndex);
			}
			else
			{
				orCreateHeroRunner.Level.Value = 0;
			}
		}
	}
}
