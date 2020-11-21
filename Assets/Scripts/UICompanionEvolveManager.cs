using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompanionEvolveManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_evolvePopupPrefab;

	private int m_currentHeroIndex = -1;

	private GameObject m_prevInfo;

	public void ShowInfo(int heroIndex)
	{
		if (m_currentHeroIndex != heroIndex)
		{
			base.transform.DestroyChildrenImmediate();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", heroIndex);
			m_prevInfo = Singleton<PropertyManager>.Instance.Instantiate(m_evolvePopupPrefab, Vector3.zero, Quaternion.identity, dictionary);
			m_prevInfo.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			m_prevInfo.gameObject.SetActive(value: true);
		}
		StartCoroutine(StartEvolutionAnimations());
		m_currentHeroIndex = heroIndex;
	}

	private IEnumerator StartEvolutionAnimations()
	{
		yield return false;
		Transform popupParent = m_prevInfo.transform.GetChild(0);
		CreatureUIController cuic = popupParent.GetComponentInChildren<CreatureUIController>();
		cuic.StartCoroutine(cuic.EvolutionAnimations());
	}
}
