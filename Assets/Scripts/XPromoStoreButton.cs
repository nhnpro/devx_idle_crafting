/*
using UnityEngine;

public class XPromoStoreButton : MonoBehaviour
{
	[Tooltip("Either 'FA', 'BA', 'CA', 'SA', ''TW  or 'IN'")]
	[SerializeField]
	private string m_xpromoApp;

	public void OnButton()
	{
		XPromoConfig cfg = PersistentSingleton<Economies>.Instance.XPromo.Find((XPromoConfig c) => c.ID == m_xpromoApp);
		XPromoPlugin.OpenAppPage(cfg);
	}
}
*/
