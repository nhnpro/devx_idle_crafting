using UniRx;
using UnityEngine;

public class XPromoAppButton : MonoBehaviour
{
	[Tooltip("Either 'FA', 'BA', 'CA', 'SA', 'TW' or 'IN'")]
	[SerializeField]
	private string m_xpromoApp;

	private bool m_isInstalled;

	protected void Start()
	{
		XPromoConfig cfg = PersistentSingleton<Economies>.Instance.XPromo.Find((XPromoConfig c) => c.ID == m_xpromoApp);
		(from p in Observable.EveryApplicationPause().StartWith(value: false)
			where !p
			select p).Subscribe(delegate
		{
			// m_isInstalled = XPromoPlugin.CheckForExternalApp(cfg);
		}).AddTo(this);
	}

	public void OnButton()
	{
		XPromoConfig cfg = PersistentSingleton<Economies>.Instance.XPromo.Find((XPromoConfig c) => c.ID == m_xpromoApp);
		if (m_isInstalled)
		{
			// XPromoPlugin.OpenAppOnDevice(cfg);
		}
		else
		{
			// XPromoPlugin.OpenAppPage(cfg);
		}
	}
}
