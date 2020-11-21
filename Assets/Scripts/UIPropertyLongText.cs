using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyLongText : UIPropertyBase
{
	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<long> property = GetProperty<long>();
		property.TakeUntilDestroy(this).SubscribeToText(m_text).AddTo(this);
	}
}
