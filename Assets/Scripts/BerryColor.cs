using UnityEngine;

public class BerryColor : MonoBehaviour
{
	public void SetBerryColor(int colorIndex)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (i == colorIndex)
			{
				child.gameObject.SetActive(value: true);
			}
			else
			{
				child.gameObject.SetActive(value: false);
			}
		}
	}
}
