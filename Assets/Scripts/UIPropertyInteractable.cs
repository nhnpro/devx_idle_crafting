using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIPropertyInteractable : UIPropertyBase
{
	private Selectable m_selectable;

	protected void Start()
	{
		m_selectable = GetComponent<Selectable>();
		IReadOnlyReactiveProperty<bool> property = GetProperty<bool>();
		property.TakeUntilDestroy(this).SubscribeToInteractable(m_selectable).AddTo(this);
	}
}
