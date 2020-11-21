using UniRx;
using UnityEngine;

public class UIPropertyActivate : UIAlwaysStartPropertyBase
{
	[SerializeField]
	private bool m_activateOnTrue = true;

	[SerializeField]
	private bool m_active = true;

	public override void AlwaysStart()
	{
		IReadOnlyReactiveProperty<bool> property = GetProperty<bool>();
		(from active in property.TakeWhile((bool _) => this != null)
			where active == m_activateOnTrue
			select active into _
			select m_active).SubscribeToActiveUntilNull(base.gameObject).AddTo(this);
	}
}
