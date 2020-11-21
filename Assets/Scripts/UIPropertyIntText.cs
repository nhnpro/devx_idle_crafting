using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyIntText : UIPropertyBase
{
	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<int> property = GetProperty<int>();
		property.TakeUntilDestroy(this).SubscribeToText(m_text).AddTo(this);
	}
}
