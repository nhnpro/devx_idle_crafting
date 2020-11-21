using UnityEngine;

public class UIMapJellyIntroParent : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(PersistentSingleton<GameSettings>.Instance.MapJellyOn);
	}
}
