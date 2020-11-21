using System.Collections;
using UnityEngine;

public class DisableAfter : MonoBehaviour
{
	[SerializeField]
	private float m_seconds;

	protected void OnEnable()
	{
		StartCoroutine(DisableAfterSeconds());
	}

	private IEnumerator DisableAfterSeconds()
	{
		yield return new WaitForSeconds(m_seconds);
		base.gameObject.SetActive(value: false);
	}

	public float GetSeconds()
	{
		return m_seconds;
	}
}
