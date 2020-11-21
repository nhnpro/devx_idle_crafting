using Big;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TweakableManager : Singleton<TweakableManager>
{
	private static List<byte[]> sm_listMem = new List<byte[]>();

	public TweakableManager()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
	}

	private UniRx.IObservable<string> providerInfo(FlooredAdProvider p)
	{
		return from _ in p.FloorValue.Merge(from _ in p.Zone
				select 1)
			select p.ProviderInfo();
	}

	private void DebugGiveChunkReward()
	{
		int value = PlayerData.Instance.MainChunk.Value;
		ChunkGeneratingConfig chunkGeneratingConfig = Singleton<EconomyHelpers>.Instance.GetChunkGeneratingConfig(value, bossFight: false);
		GiveChunkReward(chunkGeneratingConfig, boss: false);
		if (value % 5 == 4)
		{
			ChunkGeneratingConfig chunkGeneratingConfig2 = Singleton<EconomyHelpers>.Instance.GetChunkGeneratingConfig(value, bossFight: true);
			GiveChunkReward(chunkGeneratingConfig2, boss: true);
		}
	}

	private void GiveChunkReward(ChunkGeneratingConfig cnfg, bool boss)
	{
		BigDouble left = BigDouble.ZERO;
		if (boss)
		{
			int value = PlayerData.Instance.MainChunk.Value;
			BigDouble amount = Singleton<EconomyHelpers>.Instance.GetRelicsFromBoss(value) * PlayerData.Instance.BoostersEffect[2].Value;
			Singleton<FundRunner>.Instance.AddRelicsToBackpak(amount);
			bool flag = ChunkRunner.IsLastChunkForNode(value);
			BiomeConfig value2 = Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value;
			left = ((!flag) ? value2.MiniBossReward : value2.BossReward);
			left *= Singleton<CumulativeBonusRunner>.Instance.BonusMult[2].Value;
		}
		int[] array = new int[7];
		int num = 0;
		if (Random.Range(0f, 1f) < cnfg.TNTChance)
		{
			num = Random.Range(cnfg.TNTMin, cnfg.TNTMax + 1) * 8;
		}
		int num2 = 0;
		if (Random.Range(0f, 1f) < cnfg.DiamondChance)
		{
			num2 = Random.Range(cnfg.DiamondMin, cnfg.DiamondMax);
		}
		Singleton<FundRunner>.Instance.AddGems(num2 * PersistentSingleton<GameSettings>.Instance.GemBlockReward, "debugMenu", "debug");
		for (int i = 0; i < cnfg.MaxBlocks - num2 - num - cnfg.GoldBlockAverage; i++)
		{
			array[(int)cnfg.Materials.AllotObject()]++;
		}
		for (int j = 0; j < array.Length; j++)
		{
			PlayerData.Instance.BlocksInBackpack[j].Value += array[j];
		}
		left += cnfg.MaxBlocks * Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value.BlockReward * Singleton<CumulativeBonusRunner>.Instance.BonusMult[1].Value * PlayerData.Instance.BoostersEffect[1].Value;
		Singleton<FundRunner>.Instance.AddCoins(left);
	}

	public void Alloc10MB()
	{
		sm_listMem.Add(new byte[10485760]);
	}
}
