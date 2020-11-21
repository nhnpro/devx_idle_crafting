using System.Collections;
using UnityEngine;

public class SuccessBossParent : MonoBehaviour
{
	private GameObject m_gameObject;

	protected void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		m_gameObject = null;
		int value = Singleton<BossIndexRunner>.Instance.CurrentBossIndex.Value;
		if (value != -1)
		{
			m_gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath(value), Vector3.zero, Quaternion.identity);
			m_gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			StartCoroutine(RenderQueueRoutine());
		}
	}

	private IEnumerator RenderQueueRoutine()
	{
		yield return null;
		if (m_gameObject != null)
		{
			m_gameObject.GetComponentInChildren<Renderer>().material.renderQueue = 2999;
		}
	}

	private string GetPrefabPath(int heroIndex)
	{
		return "CreaturesInsideJelly/Creature" + heroIndex;
	}
}
