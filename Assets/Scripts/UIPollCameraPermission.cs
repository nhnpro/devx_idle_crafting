using UnityEngine;

public class UIPollCameraPermission : MonoBehaviour
{
	protected void Update()
	{
		if (ARService.HasCameraPermission())
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
