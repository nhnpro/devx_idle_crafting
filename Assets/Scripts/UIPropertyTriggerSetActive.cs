using UniRx;
using UnityEngine;

public class UIPropertyTriggerSetActive : UIAlwaysStartPropertyBase
{
	[SerializeField]
	private bool m_activateOnTrue = true;

	public override void AlwaysStart()
	{
		IReadOnlyReactiveProperty<bool> property = GetProperty<bool>();
		if (m_activateOnTrue)
		{
			(from active in property.TakeWhile((bool _) => this != null)
				where active
				select active).SubscribeToActive(base.gameObject).AddTo(this);
		}
		else
		{
			(from active in property.TakeWhile((bool _) => this != null)
				where !active
				select active).SubscribeToActive(base.gameObject).AddTo(this);
		}
	}
}
