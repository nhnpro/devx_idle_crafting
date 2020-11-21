using UnityEngine;

public class UIMapTravel : MonoBehaviour
{
	public void OnTravel()
	{
		Singleton<WorldRunner>.Instance.CloseMap();
	}
}
