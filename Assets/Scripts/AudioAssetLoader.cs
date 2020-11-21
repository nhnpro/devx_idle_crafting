using System.Collections.Generic;
using UnityEngine;

public class AudioAssetLoader : MonoBehaviour
{
	public bool m_loaded;

	public string m_resourcesAsset = string.Empty;

	public bool m_loadOnAwake;

	public AudioGroupComponent audioGroupComponent;

	public List<GameObject> m_loadedAssets = new List<GameObject>();

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		audioGroupComponent = base.gameObject.transform.GetComponent<AudioGroupComponent>();
		if (m_loadOnAwake)
		{
			LoadAssets();
		}
	}

	private void OnDisable()
	{
		UnLoadAssets();
	}

	public void LoadAssets()
	{
		if (!m_loaded)
		{
			InstantiateResourceAsset();
		}
	}

	public void UnLoadAssets()
	{
		if (m_loadedAssets != null)
		{
			foreach (GameObject loadedAsset in m_loadedAssets)
			{
				if (loadedAsset != null)
				{
					AudioController.Instance.Log("Destroy AudioPrefab: " + loadedAsset + " at " + base.name);
					UnityEngine.Object.Destroy(loadedAsset);
				}
			}
			if (audioGroupComponent != null)
			{
			}
			m_loadedAssets.Clear();
			m_loaded = false;
		}
	}

	private void RegisterInstantiatedWithGroup()
	{
		if (audioGroupComponent != null)
		{
			AudioController.Instance.Log("Instantiated Prefabs - Registered with Group - " + base.name);
		}
		else
		{
			AudioController.Instance.Log("Instantiated Prefabs - Could not Register with Group :/ - " + base.name);
		}
	}

	private void InstantiateResourceAsset()
	{
		if (m_resourcesAsset != string.Empty)
		{
			Object @object = Resources.Load(m_resourcesAsset);
			if (@object != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = base.transform.localPosition;
				gameObject.transform.localRotation = base.transform.localRotation;
				m_loadedAssets.Add(gameObject);
				AudioController.Instance.Log("Instantiate AudioPrefab: " + m_resourcesAsset + " as child of " + base.name);
				RegisterInstantiatedWithGroup();
				m_loaded = true;
			}
			else
			{
				AudioController.Instance.LogError("Failed to Instantiate, could not find " + m_resourcesAsset + " - Check that naming is correct, and the asset is in a Resources folder");
			}
		}
	}
}
