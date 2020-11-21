using UnityEngine;

public class UIPrestigeManager : MonoBehaviour
{
	public void OnPrestige()
	{
		Singleton<PrestigeRunner>.Instance.StartPrestigeSequence();
	}
}
