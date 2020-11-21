using System.Collections.Generic;
using UnityEngine;

public class EnableWhenActive : MonoBehaviour
{
	[SerializeField]
	private bool m_enable = true;

	[SerializeField]
	private EnableEnum m_feature;

	private static List<EnableWhenActive>[] m_callerStack;

	protected void Awake()
	{
		if (m_callerStack == null)
		{
			m_callerStack = new List<EnableWhenActive>[6];
			for (int i = 0; i < 6; i++)
			{
				m_callerStack[i] = new List<EnableWhenActive>();
			}
		}
	}

	public void OnEnable()
	{
		m_callerStack[(int)m_feature].Add(this);
		Set(m_feature, m_enable);
	}

	public void OnDisable()
	{
		List<EnableWhenActive> list = m_callerStack[(int)m_feature];
		list.Remove(this);
		if (list.Count == 0)
		{
			Set(m_feature, !m_enable);
		}
		else
		{
			Set(m_feature, list[list.Count - 1].m_enable);
		}
	}

	private void Set(EnableEnum feature, bool enable)
	{
		switch (feature)
		{
		case EnableEnum.Blur:
			Singleton<BlurRunner>.Instance.SetBlur(enable);
			break;
		case EnableEnum.GameView:
			if (Camera.main != null)
			{
				Camera.main.farClipPlane = ((!enable) ? 1 : 200);
			}
			if (Singleton<EnableObjectsRunner>.Instance != null)
			{
				Singleton<EnableObjectsRunner>.Instance.GameView.Value = enable;
			}
			break;
		case EnableEnum.SmallCurrencyHeader:
			if (Singleton<EnableObjectsRunner>.Instance != null)
			{
				Singleton<EnableObjectsRunner>.Instance.SmallCurrencyHeader.Value = enable;
			}
			break;
		case EnableEnum.Popup:
			if (Singleton<EnableObjectsRunner>.Instance != null)
			{
				Singleton<EnableObjectsRunner>.Instance.Popup.Value = enable;
			}
			break;
		case EnableEnum.DelayedCurrencyHeader:
			if (Singleton<EnableObjectsRunner>.Instance != null)
			{
				Singleton<EnableObjectsRunner>.Instance.DelayedCurrencyHeader.Value = enable;
			}
			break;
		case EnableEnum.MapCloseButton:
			if (Singleton<EnableObjectsRunner>.Instance != null)
			{
				Singleton<EnableObjectsRunner>.Instance.MapCloseButton.Value = enable;
			}
			break;
		}
	}
}
