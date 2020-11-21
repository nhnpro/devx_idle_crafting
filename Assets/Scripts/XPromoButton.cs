using UniRx;
using UnityEngine;

public class XPromoButton : AlwaysStartBehaviour
{
	[Tooltip("Either 'FA', 'BA', 'CA', 'SA', 'TW' or 'IN'")]
	[SerializeField]
	private string m_xpromoApp;

	[SerializeField]
	private UIPopupManager m_popupManager;

	private bool m_isInstalled;

	public override void AlwaysStart()
	{
		Singleton<XPromoRunner>.Instance.ChangeApp.Subscribe(delegate(string app)
		{
			Show(app == m_xpromoApp);
		}).AddTo(SceneLoader.Instance);
	}

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

	private void Show(bool show)
	{
		base.gameObject.SetActive(show);
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
			m_popupManager.ShowInfo();
		}
		Singleton<XPromoRunner>.Instance.SetNextApp();
	}
}
