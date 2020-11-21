using UnityEngine;

public class UIIAPButtons : MonoBehaviour
{
	public void OnHidePurchaseSuccess()
	{
		Singleton<IAPRunner>.Instance.PurchaseSuccess.SetValueAndForceNotify(value: false);
	}

	public void OnHidePurchaseFail()
	{
		Singleton<IAPRunner>.Instance.PurchaseFail.SetValueAndForceNotify(value: false);
	}
}
