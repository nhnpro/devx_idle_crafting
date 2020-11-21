using UnityEngine;

public class UISetActive : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_gameObjects;

	protected void Start()
	{
		GameObject[] gameObjects = m_gameObjects;
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.SetActive(value: true);
		}
	}
}
