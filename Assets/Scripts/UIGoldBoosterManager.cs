using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGoldBoosterManager : MonoBehaviour
{
	protected void Start()
	{
		SceneLoader.Instance.StartCoroutine(PopulateBoosters());
	}

	private IEnumerator PopulateBoosters()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> pars = new Dictionary<string, object>();
		for (int i = 0; i < 3; i++)
		{
			pars.Clear();
			pars.Add("GoldBoosterIndex", i);
			GameObject card = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ShopBoosters/ShopGoldBooster", Vector3.zero, Quaternion.identity, pars);
			card.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}
}
