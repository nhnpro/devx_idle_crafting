using Big;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyLocalizedBigDoubleText : UIPropertyBase
{
	[SerializeField]
	private string m_key = string.Empty;

	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<BigDouble> property = GetProperty<BigDouble>();
		(from big in property.TakeUntilDestroy(this)
			select PersistentSingleton<LocalizationService>.Instance.Text(m_key, BigString.ToString(big))).SubscribeToText(m_text).AddTo(this);
	}
}
