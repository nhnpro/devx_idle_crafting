using UniRx;
using UnityEngine;

public class UIBundleSetActive : AlwaysStartBehaviour
{
	[SerializeField]
	private IAPProductEnum m_bundle;

	[SerializeField]
	private bool m_activeOnTrue = true;

	public override void AlwaysStart()
	{
		IAPConfig iap = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig cfg) => cfg.ProductEnum == m_bundle);
		(from _ in PlayerData.Instance.PurchasedIAPBundleIDs.ObserveCountChanged(notifyCurrentCount: true)
			select PlayerData.Instance.PurchasedIAPBundleIDs.Contains(iap.ProductID) into b
			select b == m_activeOnTrue).SubscribeToActiveUntilNull(base.gameObject).AddTo(this);
	}
}
