using UniRx;
using UnityEngine;

public class UIActiveBundleSetActive : AlwaysStartBehaviour
{
	[SerializeField]
	private IAPProductEnum m_bundle;

	[SerializeField]
	private bool m_activeOnTrue = true;

	public override void AlwaysStart()
	{
		(from iap in Singleton<IAPBundleRunner>.Instance.ProductID
			select iap == m_bundle into b
			select b == m_activeOnTrue).SubscribeToActive(base.gameObject).AddTo(this);
	}
}
