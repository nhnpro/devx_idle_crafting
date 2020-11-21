using System.Collections.Generic;
using UnityEngine;

public class UILanguageButton : PropertyContext
{
	[SerializeField]
	private int m_languageIndex;

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("LangIndex"))
		{
			m_languageIndex = (int)parameters["LangIndex"];
		}
	}

	public void OnSelectLanguage()
	{
		PersistentSingleton<LocalizationService>.Instance.SelectLanguage(m_languageIndex);
		BindingManager.Instance.LanguageConfirmParent.ShowInfo();
	}
}
