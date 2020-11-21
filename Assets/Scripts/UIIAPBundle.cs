using UnityEngine;

public class UIIAPBundle : MonoBehaviour
{
	[SerializeField]
	private string buttonPlacement = "store";

	public void OnBuyBundle()
	{
		Singleton<IAPBundleRunner>.Instance.BuyIAPProduct(buttonPlacement);
	}

	public void OnShowOfferPopup()
	{
		Singleton<IAPBundleRunner>.Instance.ShowOfferPopup.SetValueAndForceNotify(value: true);
	}

	public void OnHideOfferPopup()
	{
		Singleton<IAPBundleRunner>.Instance.ShowOfferPopup.SetValueAndForceNotify(value: false);
	}
}
