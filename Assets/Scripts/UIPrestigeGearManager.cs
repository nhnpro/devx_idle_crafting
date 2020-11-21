using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrestigeGearManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_prestigeGearPrefab;

	protected void Start()
	{
		SceneLoader.Instance.StartCoroutine(PopulatePrestigeGears());
	}

	private IEnumerator PopulatePrestigeGears()
	{
		Dictionary<string, object> pars = new Dictionary<string, object>();
		for (int i = 0; i < PersistentSingleton<Economies>.Instance.Gears.Count; i++)
		{
			pars.Clear();
			pars.Add("GearIndex", i);
			GameObject go = Singleton<PropertyManager>.Instance.Instantiate(m_prestigeGearPrefab, Vector3.zero, Quaternion.identity, pars);
			go.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}
}
