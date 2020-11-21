using UnityEngine;

public class TabSwitcher : MonoBehaviour
{
	public GameObject[] Tabs;

	public void ActivateTab(GameObject tab)
	{
		GameObject[] tabs = Tabs;
		foreach (GameObject gameObject in tabs)
		{
			gameObject.SetActive(value: false);
		}
		tab.SetActive(value: true);
	}
}
