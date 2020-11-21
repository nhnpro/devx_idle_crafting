using UnityEngine;

public class PrefabPlaceholder : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_prefabs;

	protected void Awake()
	{
		GameObject original = m_prefabs[Random.Range(0, m_prefabs.Length)];
		GameObject gameObject = UnityEngine.Object.Instantiate(original, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform.parent;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void OnDrawGizmos()
	{
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
