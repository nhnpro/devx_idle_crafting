using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyText : UIPropertyBase
{
	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<string> property = GetProperty<string>();
		(from i in property.TakeUntilDestroy(this)
			select i.ToString()).SubscribeToTextUntilNull(base.gameObject, m_text).AddTo(this);
	}
}
