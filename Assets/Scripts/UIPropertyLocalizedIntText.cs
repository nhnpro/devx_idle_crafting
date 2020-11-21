using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyLocalizedIntText : UIPropertyBase
{
	[SerializeField]
	private string m_key = string.Empty;

	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<int> property = GetProperty<int>();
		(from l in property.TakeUntilDestroy(this)
			select PersistentSingleton<LocalizationService>.Instance.Text(m_key, l)).SubscribeToText(m_text).AddTo(this);
	}
}
