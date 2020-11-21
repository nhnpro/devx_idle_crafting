using SysLocalePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalizationService : PersistentSingleton<LocalizationService>
{
	private List<string> m_supportedLanguages = new List<string>();

	private Dictionary<string, string> m_texts = new Dictionary<string, string>();

	public bool m_reverseText;

	private int m_languageSelection;

	public void ParseLocalization()
	{
		m_texts.Clear();
		Parse(PersistentSingleton<StringCache>.Instance.Get("texts"), ref m_supportedLanguages, ref m_texts);
		ParseOverwrite(PersistentSingleton<StringCache>.Instance.Get("textsOverwrite"));
	}

	private void Parse(string text, ref List<string> languages, ref Dictionary<string, string> texts)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text + "\t");
		languages = (from s in source.First().Skip(1)
			select s.Trim() into s
			where s != "Description"
			select s).ToList();
		string text2 = Language();
		int num = m_supportedLanguages.IndexOf(text2) + 1;
		PlayerData.Instance.Language = text2;
		foreach (string[] item in from r in source.Skip(1)
			where r[0] != string.Empty
			select r)
		{
			try
			{
				texts.Add(item[0], item[num]);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Unable to get localized: " + string.Join(",", item) + "|index: " + num + ", len: " + item.Length + ", exception: " + ex.Message);
			}
		}
		texts.Add(string.Empty, string.Empty);
	}

	private void ParseOverwrite(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text + "\t");
		string item = Language();
		int num = m_supportedLanguages.IndexOf(item) + 1;
		foreach (string[] item2 in from r in source.Skip(1)
			where r[0] != string.Empty
			select r)
		{
			if (m_texts.ContainsKey(item2[0]) && item2.Length > num && item2[num] != string.Empty)
			{
				m_texts[item2[0]] = item2[num];
			}
		}
	}

	private string SelectedLanguage()
	{
		return (PlayerData.Instance.Language != null) ? PlayerData.Instance.Language : string.Empty;
	}

	private string DefaultLanguage()
	{
		return SystemLanguage.English.ToString();
	}

	private string OsLanguage()
	{
		string text = Application.systemLanguage.ToString();
		if (text == "Chinese")
		{
			string systemLocale = SysLocale.getSystemLocale();
			text = ((!systemLocale.Contains("zh-CN") && !systemLocale.Contains("zh_CN") && !systemLocale.Contains("zh-SG") && !systemLocale.Contains("zh_SG") && !systemLocale.Contains("zh-Hans") && !systemLocale.Contains("zh_Hans")) ? "ChineseTraditional" : "ChineseSimplified");
		}
		return text;
	}

	private string Language()
	{
		return new string[3]
		{
			SelectedLanguage(),
			OsLanguage(),
			DefaultLanguage()
		}.First((string l) => m_supportedLanguages.Contains(l));
	}

	public string Text(string key, params object[] args)
	{
		string result = null;
		try
		{
			result = string.Format(m_texts[key], args);
			return result;
		}
		catch (Exception)
		{
			return result;
		}
	}

	public List<string> SupportedLanguages()
	{
		return m_supportedLanguages;
	}

	public void SelectLanguage(int langIndex)
	{
		m_languageSelection = langIndex;
	}

	public void ChangeLanguage()
	{
		List<string> list = PersistentSingleton<LocalizationService>.Instance.SupportedLanguages();
		PlayerData.Instance.Language = list[m_languageSelection];
		PersistentSingleton<LocalizationService>.Instance.ParseLocalization();
		SceneLoadHelper.LoadMainScene();
	}
}
