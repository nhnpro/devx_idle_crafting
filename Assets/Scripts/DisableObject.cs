using UnityEngine;

public class DisableObject : MonoBehaviour
{
	public void OnDisableObject()
	{
		base.gameObject.SetActive(value: false);
	}
}
