using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertySecondsText : UIPropertyBase
{
	[SerializeField]
	private bool m_short;

	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<int> property = GetProperty<int>();
		(from secs in property.TakeUntilDestroy(this)
			select (!m_short) ? TextUtils.FormatSeconds(secs) : TextUtils.FormatSecondsShort(secs)).SubscribeToText(m_text).AddTo(this);
	}
}
