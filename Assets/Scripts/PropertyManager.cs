using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PropertyManager : Singleton<PropertyManager>
{
	private Dictionary<Type, object> m_rootContexts = new Dictionary<Type, object>();

	public void AddRootContext(object context)
	{
		m_rootContexts.Add(context.GetType(), context);
	}

	public void InstallScene()
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			InstallContexts(gameObject.transform, null);
		}
	}

	public object GetContext(string name, Transform current)
	{
		PropertyContext[] components = current.GetComponents<PropertyContext>();
		PropertyContext[] array = components;
		foreach (PropertyContext propertyContext in array)
		{
			object provider = propertyContext.GetProvider(name);
			if (provider != null)
			{
				return provider;
			}
		}
		if (current.transform.parent != null)
		{
			return GetContext(name, current.transform.parent);
		}
		Type type = Type.GetType(name);
		object value = null;
		m_rootContexts.TryGetValue(type, out value);
		if (value != null)
		{
			return value;
		}
		return null;
	}

	public void InstallContexts(Transform xform, Dictionary<string, object> parameters)
	{
		try
		{
			PropertyContext[] components = xform.GetComponents<PropertyContext>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].PerformInstall(parameters);
			}
			UIAlwaysStartPropertyBase[] components2 = xform.GetComponents<UIAlwaysStartPropertyBase>();
			for (int j = 0; j < components2.Length; j++)
			{
				components2[j].AlwaysStart();
			}
			AlwaysStartBehaviour[] components3 = xform.GetComponents<AlwaysStartBehaviour>();
			for (int k = 0; k < components3.Length; k++)
			{
				components3[k].AlwaysStart();
			}
			for (int l = 0; l < xform.childCount; l++)
			{
				InstallContexts(xform.GetChild(l), parameters);
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Expetion for: " + xform.gameObject.GetPath());
			throw ex;
		}
	}

	public GameObject Instantiate(GameObject prefab, Dictionary<string, object> parameters)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		InstallContexts(gameObject.transform, parameters);
		return gameObject;
	}

	public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Dictionary<string, object> parameters)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);
		InstallContexts(gameObject.transform, parameters);
		return gameObject;
	}

	public GameObject InstantiateSetParent(GameObject prefab, Dictionary<string, object> parameters, Transform parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		InstallContexts(gameObject.transform, parameters);
		return gameObject;
	}

	public GameObject InstantiateSetParent(GameObject prefab, Vector3 position, Quaternion rotation, Dictionary<string, object> parameters, Transform parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		InstallContexts(gameObject.transform, parameters);
		return gameObject;
	}

	public GameObject InstantiateFromResources(string path, Vector3 position, Quaternion rotation, Dictionary<string, object> parameters)
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(path, position, rotation);
		InstallContexts(gameObject.transform, parameters);
		return gameObject;
	}

	public GameObject InstantiateFromResourcesSetParent(string path, Vector3 position, Quaternion rotation, Dictionary<string, object> parameters, Transform parent)
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(path, position, rotation);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		InstallContexts(gameObject.transform, parameters);
		return gameObject;
	}
}
