using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class XPromoRunner : Singleton<XPromoRunner>
{
	public ReactiveProperty<string> ChangeApp = new ReactiveProperty<string>();

	private int m_appIndex = -1;

	public XPromoRunner()
	{
		SetNextApp();
		SceneLoader.Instance.StartCoroutine(ChangeRoutine());
	}

	private IEnumerator ChangeRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(PersistentSingleton<GameSettings>.Instance.XPromoDuration);
			SetNextApp();
		}
	}

	public void SetNextApp()
	{
		List<string> xPromotedApps = PersistentSingleton<GameSettings>.Instance.XPromotedApps;
		m_appIndex = (m_appIndex + 1) % xPromotedApps.Count;
		ChangeApp.Value = xPromotedApps[m_appIndex];
	}
}
