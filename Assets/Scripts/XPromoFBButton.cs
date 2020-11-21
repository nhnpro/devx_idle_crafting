using UniRx;
using UnityEngine;

public class XPromoFBButton : AlwaysStartBehaviour
{
	[SerializeField]
	private UIPopupManager m_popupManager;

	public override void AlwaysStart()
	{
		Singleton<XPromoRunner>.Instance.ChangeApp.Subscribe(delegate(string app)
		{
			Show(app == "FB");
		}).AddTo(SceneLoader.Instance);
	}

	private void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	public void OnButton()
	{
		m_popupManager.ShowInfo();
		Singleton<XPromoRunner>.Instance.SetNextApp();
	}
}
