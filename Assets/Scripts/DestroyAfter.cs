using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
	[SerializeField]
	private float m_seconds;

	protected void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, m_seconds);
	}
}
