using System.Collections.Generic;
using UnityEngine;

public class UICompanionInfoManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_companionInfoPrefab;

	private int m_currentHeroIndex = -1;

	private GameObject m_prevInfo;

	public void ShowInfo(int heroIndex)
	{
		if (m_currentHeroIndex != heroIndex)
		{
			base.transform.DestroyChildrenImmediate();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", heroIndex);
			m_prevInfo = Singleton<PropertyManager>.Instance.Instantiate(m_companionInfoPrefab, Vector3.zero, Quaternion.identity, dictionary);
			m_prevInfo.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			m_prevInfo.gameObject.SetActive(value: true);
		}
		m_currentHeroIndex = heroIndex;
	}
}
