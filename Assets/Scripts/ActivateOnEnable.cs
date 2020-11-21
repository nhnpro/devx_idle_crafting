using UnityEngine;

public class ActivateOnEnable : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_targetsToActivate;

	protected void OnEnable()
	{
		GameObject[] targetsToActivate = m_targetsToActivate;
		foreach (GameObject gameObject in targetsToActivate)
		{
			gameObject.SetActive(value: true);
		}
	}

	protected void OnDisable()
	{
		GameObject[] targetsToActivate = m_targetsToActivate;
		foreach (GameObject gameObject in targetsToActivate)
		{
			gameObject.SetActive(value: false);
		}
	}
}
