using System.Collections.Generic;
using UnityEngine;

public class UIPerkInfoManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_perkInfoPrefab;

	private int m_prevPerkIndex = -1;

	private int m_prevHeroIndex = -1;

	private GameObject m_prevInfo;

	public void ShowInfo(int heroIndex, int perkIndex)
	{
		if (m_prevPerkIndex != perkIndex || m_prevHeroIndex != heroIndex)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", heroIndex);
			dictionary.Add("PerkIndex", perkIndex);
			base.transform.DestroyChildrenImmediate();
			m_prevInfo = Singleton<PropertyManager>.Instance.Instantiate(m_perkInfoPrefab, Vector3.zero, Quaternion.identity, dictionary);
			m_prevInfo.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			m_prevInfo.gameObject.SetActive(value: true);
		}
		m_prevPerkIndex = perkIndex;
		m_prevHeroIndex = heroIndex;
	}
}
