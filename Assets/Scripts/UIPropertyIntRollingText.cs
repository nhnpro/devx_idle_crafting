using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyIntRollingText : UIPropertyBase
{
	[SerializeField]
	private float m_delay = 1f;

	[SerializeField]
	private bool m_resetOnEnable;

	private Text m_text;

	private RollingFloat m_rolling = new RollingFloat();

	private IReadOnlyReactiveProperty<int> m_reactiveInt;

	protected void Start()
	{
		m_reactiveInt = GetProperty<int>();
		m_text = GetComponent<Text>();
		m_rolling.SetCurrent(m_reactiveInt.Value);
		m_text.text = Mathf.RoundToInt(m_rolling.Current).ToString();
		m_reactiveInt.TakeUntilDestroy(this).Delay(TimeSpan.FromSeconds(m_delay)).Subscribe(delegate(int target)
		{
			m_rolling.SetTarget(target);
		})
			.AddTo(this);
	}

	protected void OnEnable()
	{
		if (m_reactiveInt != null && m_resetOnEnable)
		{
			m_rolling.SetCurrent(m_reactiveInt.Value);
			m_text.text = Mathf.RoundToInt(m_rolling.Current).ToString();
		}
	}

	protected void Update()
	{
		if (m_rolling.Update())
		{
			m_text.text = Mathf.RoundToInt(m_rolling.Current).ToString();
		}
	}
}
