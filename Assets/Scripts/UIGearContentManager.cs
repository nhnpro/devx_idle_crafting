using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGearContentManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_gearPrefab;

	[SerializeField]
	private GearCategory m_category;

	protected void Start()
	{
		base.transform.DestroyChildrenImmediate();
		SceneLoader.Instance.StartCoroutine(PopulateGearContents());
	}

	private IEnumerator PopulateGearContents()
	{
		Dictionary<string, object> pars = new Dictionary<string, object>();
		foreach (GearRunner gear in Singleton<GearCollectionRunner>.Instance.Gears())
		{
			if (gear.Config.Category == m_category)
			{
				pars.Clear();
				pars.Add("GearIndex", gear.GearIndex);
				GameObject card = Singleton<PropertyManager>.Instance.Instantiate(m_gearPrefab, Vector3.zero, Quaternion.identity, pars);
				card.transform.SetParent(base.transform, worldPositionStays: false);
				yield return null;
			}
		}
	}
}
