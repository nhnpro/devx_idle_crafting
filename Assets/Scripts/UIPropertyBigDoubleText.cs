using Big;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyBigDoubleText : UIPropertyBase
{
	[SerializeField]
	private int m_precision = 2;

	private Text m_text;

	protected void Start()
	{
		m_text = GetComponent<Text>();
		IReadOnlyReactiveProperty<BigDouble> property = GetProperty<BigDouble>();
		(from big in property.TakeUntilDestroy(this)
			select BigString.ToString(big, m_precision)).SubscribeToText(m_text).AddTo(this);
	}

	protected void OnDestroy()
	{
	}
}
