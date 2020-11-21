using System.Collections.Generic;
using UnityEngine;

public class SimplePoolManager : MonoBehaviour
{
	[SerializeField]
	private int m_initialSize = 8;

	[SerializeField]
	private bool m_allowGrowth = true;

	[SerializeField]
	private GameObject m_prefab;

	private List<GameObject> m_objects;

	protected void Start()
	{
		for (int i = 0; i < m_initialSize; i++)
		{
			CreateNew();
		}
	}

	private GameObject CreateNew()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_prefab);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		gameObject.SetActive(value: false);
		return gameObject;
	}

	public GameObject GetPooledObject()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!child.gameObject.activeSelf)
			{
				return child.gameObject;
			}
		}
		if (m_allowGrowth)
		{
			return CreateNew();
		}
		return null;
	}

	public T GetPooledComponent<T>() where T : Component
	{
		GameObject pooledObject = GetPooledObject();
		if (pooledObject == null)
		{
			return (T)null;
		}
		return pooledObject.GetComponent<T>();
	}
}
