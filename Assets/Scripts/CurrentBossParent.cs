using UnityEngine;

public class CurrentBossParent : MonoBehaviour
{
	private GameObject m_gameObject;

	protected void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		int value = Singleton<BossIndexRunner>.Instance.CurrentBossIndex.Value;
		if (value == -1)
		{
			m_gameObject = GameObjectExtensions.InstantiateFromResources("Trapped/TrappedCoins", Vector3.zero, Quaternion.identity);
		}
		else
		{
			m_gameObject = GameObjectExtensions.InstantiateFromResources("Trapped/TrappedCompanion", Vector3.zero, Quaternion.identity);
		}
		m_gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
