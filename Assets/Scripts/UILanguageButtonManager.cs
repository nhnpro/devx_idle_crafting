using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILanguageButtonManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_languageCardPrefab;

	protected void Start()
	{
		SceneLoader.Instance.StartCoroutine(PopulateLanguages());
	}

	private IEnumerator PopulateLanguages()
	{
		yield return null;
		Dictionary<string, object> pars = new Dictionary<string, object>();
		List<string> langs = PersistentSingleton<LocalizationService>.Instance.SupportedLanguages();
		for (int i = 0; i < langs.Count; i++)
		{
			pars.Clear();
			pars.Add("LangIndex", i);
			string lang = PersistentSingleton<LocalizationService>.Instance.Text("UI.Language." + langs[i]);
			pars.Add("StringValue", lang);
			GameObject card = Singleton<PropertyManager>.Instance.Instantiate(m_languageCardPrefab, Vector3.zero, Quaternion.identity, pars);
			card.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}
}
