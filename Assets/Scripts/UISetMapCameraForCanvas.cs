using UnityEngine;

public class UISetMapCameraForCanvas : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Canvas>().worldCamera = BindingManager.Instance.MapCamera.GetComponent<Camera>();
	}
}
