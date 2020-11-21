using System.Collections.Generic;
using UnityEngine;

public class UIGearInfoManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_gearInfoPrefab;

	private int m_currentGearIndex = -1;

	private GameObject m_prevInfo;

	public void ShowInfo(int gearIndex)
	{
		if (m_currentGearIndex != gearIndex)
		{
			base.transform.DestroyChildrenImmediate();
			GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gearIndex);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("GearIndex", gearIndex);
			dictionary.Add("GearSetIndex", orCreateGearRunner.SetIndex);
			m_prevInfo = Singleton<PropertyManager>.Instance.Instantiate(m_gearInfoPrefab, Vector3.zero, Quaternion.identity, dictionary);
			m_prevInfo.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			m_prevInfo.gameObject.SetActive(value: true);
		}
		m_currentGearIndex = gearIndex;
	}
}
