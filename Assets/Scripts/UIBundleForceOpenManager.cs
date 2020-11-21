using UniRx;
using UnityEngine;

public class UIBundleForceOpenManager : MonoBehaviour
{
	private bool m_triedOnce;

	public void OnPossiblyForceBundleOpen()
	{
		if (!m_triedOnce)
		{
			(from _ in (from sync in (from active in Singleton<IAPBundleRunner>.Instance.BundleActive.Take(1)
						where active
						select active into _
						select ServerTimeService.IsSynced).Switch()
					where sync
					select sync).Take(1)
				select UnityEngine.Random.Range(0, 2) > 0).Subscribe(delegate(bool activate)
			{
				Singleton<IAPBundleRunner>.Instance.ShowOfferPopup.SetValueAndForceNotify(activate);
			}).AddTo(this);
		}
		m_triedOnce = true;
	}
}
