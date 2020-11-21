using System.Collections;
using UnityEngine;

public class UIFocusParentVerticalGroup : MonoBehaviour
{
	private Transform m_parent;

	[SerializeField]
	private Vector3 m_offset = Vector3.zero;

	private void OnEnable()
	{
		StartCoroutine(EnableRoutine());
	}

	private IEnumerator EnableRoutine()
	{
		yield return null;
		m_parent = base.transform.parent;
		if (m_parent != null)
		{
			Vector3 localPosition = -m_parent.InverseTransformPoint(base.transform.position + m_offset);
			m_parent.localPosition = localPosition;
		}
	}
}
