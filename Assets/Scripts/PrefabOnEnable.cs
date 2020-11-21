using UnityEngine;

public class PrefabOnEnable : MonoBehaviour
{
	[SerializeField]
	private GameObject m_prefab;

	protected void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		GameObject gameObject = UnityEngine.Object.Instantiate(m_prefab, Vector3.zero, Quaternion.identity);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
