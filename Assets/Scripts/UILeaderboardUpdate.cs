using UnityEngine;

public class UILeaderboardUpdate : MonoBehaviour
{
	protected void OnEnable()
	{
		Singleton<LeaderboardRunner>.Instance.LeaderboardUpdateTrigger.SetValueAndForceNotify(value: true);
	}
}
