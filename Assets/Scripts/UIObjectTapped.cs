using UnityEngine;

public class UIObjectTapped : MonoBehaviour
{
	[SerializeField]
	private string tappableObject;

	public void OnObjectTapped()
	{
		PersistentSingleton<GameAnalytics>.Instance.ObjectTapped.Value = tappableObject;
	}
}
