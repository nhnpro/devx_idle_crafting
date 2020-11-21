using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PressButtonWithDelay : MonoBehaviour
{
	[SerializeField]
	private float m_delay = 0.5f;

	[SerializeField]
	private Button m_theButton;

	public static int RefCount
	{
		get;
		private set;
	}

	protected void OnEnable()
	{
		RefCount++;
		StartCoroutine(PressButton());
	}

	protected void OnDisable()
	{
		StopAllCoroutines();
		RefCount--;
	}

	private IEnumerator PressButton()
	{
		yield return new WaitForSeconds(m_delay);
		if (m_theButton.interactable)
		{
			m_theButton.onClick.Invoke();
		}
	}
}
