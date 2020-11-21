using UnityEngine;
using UnityEngine.UI;

public class AndroidBackButton : MonoBehaviour
{
	protected void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		{
			base.gameObject.GetComponent<Button>().onClick.Invoke();
		}
	}
}
