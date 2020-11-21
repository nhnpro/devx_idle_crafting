using UnityEngine;

public class ActivateOnDisable : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_targetsToActivate;

	protected void OnEnable()
	{
		GameObject[] targetsToActivate = m_targetsToActivate;
		foreach (GameObject gameObject in targetsToActivate)
		{
			gameObject.SetActive(value: false);
		}
	}

	protected void OnDisable()
	{
		GameObject[] targetsToActivate = m_targetsToActivate;
		foreach (GameObject gameObject in targetsToActivate)
		{
			gameObject.SetActive(value: true);
		}
	}
}
