using UnityEngine;

public class UIBoss : MonoBehaviour
{
	public void OnTryBossAgain()
	{
		Singleton<ChunkRunner>.Instance.TryBossAgain();
	}

	public void OnLoseBossBattle()
	{
		Singleton<BossBattleRunner>.Instance.LoseBossBattle();
	}
}
