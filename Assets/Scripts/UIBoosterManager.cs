using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoosterManager : MonoBehaviour
{
	protected void Start()
	{
		SceneLoader.Instance.StartCoroutine(PopulateBoosters());
	}

	private IEnumerator PopulateBoosters()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> pars = new Dictionary<string, object>();
		for (int i = 0; i < Singleton<EconomyHelpers>.Instance.GetNumBoosters(); i++)
		{
			pars.Clear();
			pars.Add("BoosterIndex", i);
			GameObject card = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/ShopBoosters/ShopBooster", Vector3.zero, Quaternion.identity, pars);
			card.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}
}
