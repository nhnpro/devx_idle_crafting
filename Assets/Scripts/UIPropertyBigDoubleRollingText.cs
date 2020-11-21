using Big;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIPropertyBigDoubleRollingText : UIPropertyBase
{
	[SerializeField]
	private int m_precision = 2;

	[SerializeField]
	private float m_delay = 1f;

	[SerializeField]
	private bool m_resetOnEnable;

	private Text m_text;

	private RollingBigDouble m_rolling = new RollingBigDouble();

	private IReadOnlyReactiveProperty<BigDouble> m_reactiveBig;

	protected void Start()
	{
		m_reactiveBig = GetProperty<BigDouble>();
		m_text = GetComponent<Text>();
		m_rolling.SetCurrent(m_reactiveBig.Value);
		m_text.text = BigString.ToString(m_rolling.Current, m_precision);
		(from target in m_reactiveBig
			where target < m_rolling.Current
			select target).TakeUntilDestroy(this).Subscribe(delegate(BigDouble target)
		{
			SetNow(target);
		}).AddTo(this);
		(from target in m_reactiveBig
			where target > m_rolling.Current
			select target).TakeUntilDestroy(this).Delay(TimeSpan.FromSeconds(m_delay)).Subscribe(delegate(BigDouble target)
		{
			m_rolling.SetTarget(target);
		})
			.AddTo(this);
	}

	protected void OnEnable()
	{
		if (m_reactiveBig != null && m_resetOnEnable)
		{
			m_rolling.SetCurrent(m_reactiveBig.Value);
			m_text.text = BigString.ToString(m_rolling.Current, m_precision);
		}
	}

	protected void Update()
	{
		if (m_rolling.Update())
		{
			if (m_rolling.Current > new BigDouble(1000.0))
			{
				m_text.text = BigString.ToString(m_rolling.Current, m_precision);
			}
			else if (m_rolling.Current > new BigDouble(100.0))
			{
				m_text.text = BigString.ToString(m_rolling.Current, 3);
			}
			else if (m_rolling.Current > new BigDouble(10.0))
			{
				m_text.text = BigString.ToString(m_rolling.Current);
			}
			else if (m_rolling.Current > new BigDouble(0.0))
			{
				m_text.text = BigString.ToString(m_rolling.Current, 1);
			}
			else
			{
				m_text.text = BigString.ToString(m_rolling.Current, 3);
			}
		}
	}

	private void SetNow(BigDouble big)
	{
		m_rolling.SetCurrent(big);
		m_text.text = BigString.ToString(big, m_precision);
	}
}
