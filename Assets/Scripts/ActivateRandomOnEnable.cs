using UnityEngine;

public class ActivateRandomOnEnable : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_targetsToActivate;

	protected void OnEnable()
	{
		int num = UnityEngine.Random.Range(0, m_targetsToActivate.Length);
		m_targetsToActivate[num].SetActive(value: true);
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
