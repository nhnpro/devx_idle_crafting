using UnityEngine;

public class UIBossRewardPanel : MonoBehaviour
{
	public void OnCollect()
	{
		Singleton<BossSuccessRunner>.Instance.Collect();
	}

	public void OnAddToTeam()
	{
		Singleton<BossSuccessRunner>.Instance.AddToTeam();
	}
}
