using UnityEngine;

public class UIStartAdventure : MonoBehaviour
{
	public void OnClick()
	{
		Singleton<ChunkRunner>.Instance.StartAdventure();
	}
}
