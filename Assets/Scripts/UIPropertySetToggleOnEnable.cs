using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIPropertySetToggleOnEnable : UIPropertyBase
{
	private Toggle m_toggle;

	private IReadOnlyReactiveProperty<bool> reactiveBool;

	protected void Awake()
	{
		m_toggle = GetComponent<Toggle>();
		reactiveBool = GetProperty<bool>();
	}

	protected void OnEnable()
	{
		m_toggle.isOn = reactiveBool.Value;
	}
}
