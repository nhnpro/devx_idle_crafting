using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeroCardManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_heroCardPrefab;

	protected void Start()
	{
		SceneLoader.Instance.StartCoroutine(PopulateHeroCards());
	}

	private IEnumerator PopulateHeroCards()
	{
		Dictionary<string, object> pars = new Dictionary<string, object>();
		for (int i = 1; i < Singleton<EconomyHelpers>.Instance.GetNumHeroes(); i++)
		{
			pars.Clear();
			pars.Add("HeroIndex", i);
			GameObject card = Singleton<PropertyManager>.Instance.Instantiate(m_heroCardPrefab, Vector3.zero, Quaternion.identity, pars);
			card.transform.SetParent(base.transform, worldPositionStays: false);
			card.GetComponent<UIPropertyCompareSetActive>().SetCmpValueAndUpdate(i);
			yield return null;
		}
	}
}
