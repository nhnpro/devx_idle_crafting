using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	public bool DrivenFromCode;

	public string key = string.Empty;

	private Text text;

	public string[] args = new string[0];

	protected void Start()
	{
		text = GetComponent<Text>();
		if (!DrivenFromCode)
		{
			SetText(key, args);
		}
	}

	public void SetText(string key, params object[] args)
	{
		text.text = PersistentSingleton<LocalizationService>.Instance.Text(key, args);
	}
}
