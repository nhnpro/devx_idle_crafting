using System.Collections.Generic;
using UnityEngine;

public class EntityPoolManager : Singleton<EntityPoolManager>
{
	private Dictionary<string, List<GameObject>> m_pathPools = new Dictionary<string, List<GameObject>>();

	private Dictionary<GameObject, List<GameObject>> m_prefabPools = new Dictionary<GameObject, List<GameObject>>();

	public GameObject GetOrCreateGameObject(GameObject prefab)
	{
		GameObject gameObject = GetOrCreatePool(prefab).Find((GameObject go) => !go.activeSelf);
		if (gameObject == null)
		{
			gameObject = CreateGameObject(prefab);
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private List<GameObject> GetOrCreatePool(GameObject prefab)
	{
		if (!m_prefabPools.ContainsKey(prefab))
		{
			m_prefabPools[prefab] = new List<GameObject>();
		}
		return m_prefabPools[prefab];
	}

	private GameObject CreateGameObject(GameObject prefab)
	{
		GameObject gameObject = Object.Instantiate(prefab);
		m_prefabPools[prefab].Add(gameObject);
		return gameObject;
	}

	public GameObject GetOrCreateGameObject(string prefabPath)
	{
		GameObject gameObject = GetOrCreatePool(prefabPath).Find((GameObject go) => !go.activeSelf);
		if (gameObject == null)
		{
			gameObject = CreateGameObject(prefabPath);
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private List<GameObject> GetOrCreatePool(string prefabPath)
	{
		if (!m_pathPools.ContainsKey(prefabPath))
		{
			m_pathPools[prefabPath] = new List<GameObject>();
		}
		return m_pathPools[prefabPath];
	}

	private GameObject CreateGameObject(string prefabPath)
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(prefabPath);
		m_pathPools[prefabPath].Add(gameObject);
		return gameObject;
	}
}
