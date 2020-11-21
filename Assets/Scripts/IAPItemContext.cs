using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class IAPItemContext : PropertyContext
{
	[SerializeField]
	private IAPProductEnum m_productID;

	[SerializeField]
	private string buttonPlacement = "store";

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("IAPProductID"))
		{
			m_productID = (IAPProductEnum)parameters["IAPProductID"];
		}
		Add("IAPItemRunner", Singleton<IAPItemCollectionRunner>.Instance.GetOrCreateIAPItemRunner(m_productID));
	}

	public void OnBuy()
	{
		Singleton<IAPItemCollectionRunner>.Instance.GetOrCreateIAPItemRunner(m_productID).BuyIAPProduct(buttonPlacement);
		(from iap in Singleton<IAPRunner>.Instance.IAPCompleted
			where iap.Config.ProductEnum == m_productID
			select iap).Take(1).Subscribe(delegate
		{
			BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: false);
		}).AddTo(this);
	}
}
