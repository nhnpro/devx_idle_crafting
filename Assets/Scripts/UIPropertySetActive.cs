using UniRx;
using UnityEngine;

public class UIPropertySetActive : UIAlwaysStartPropertyBase
{
	[SerializeField]
	private bool m_activeOnTrue = true;

	public override void AlwaysStart()
	{
		IReadOnlyReactiveProperty<bool> property = GetProperty<bool>();
		(from active in property.TakeWhile((bool _) => this != null)
			select active == m_activeOnTrue).SubscribeToActiveUntilNull(base.gameObject).AddTo(this);
	}
}
