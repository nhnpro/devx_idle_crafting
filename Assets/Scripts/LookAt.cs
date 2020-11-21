using UnityEngine;

public class LookAt : MonoBehaviour
{
	[SerializeField]
	private Transform m_target;

	[SerializeField]
	private Vector3 m_up;

	protected void Update()
	{
		base.transform.LookAt(m_target, m_up);
	}
}
