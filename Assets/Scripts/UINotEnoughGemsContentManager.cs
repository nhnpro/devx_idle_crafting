using System.Collections.Generic;
using UnityEngine;

public class UINotEnoughGemsContentManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_iapPrefab;

	[SerializeField]
	private GameObject m_bestIapPrefab;

	protected void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("IAPProductID", Singleton<NotEnoughGemsRunner>.Instance.SmallestGemPurchase.Value.ProductEnum);
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_iapPrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		dictionary.Clear();
		dictionary.Add("IAPProductID", Singleton<NotEnoughGemsRunner>.Instance.BiggestGemPurchase.Value.ProductEnum);
		GameObject gameObject2 = Singleton<PropertyManager>.Instance.Instantiate(m_bestIapPrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
