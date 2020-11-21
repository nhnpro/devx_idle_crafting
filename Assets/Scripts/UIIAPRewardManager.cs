using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIAPRewardManager : MonoBehaviour
{
	protected void OnEnable()
	{
		StartCoroutine(PopulateRewards());
	}

	private IEnumerator PopulateRewards()
	{
		yield return null;
		base.transform.DestroyChildrenImmediate();
		foreach (string[] reward in Singleton<IAPRunner>.Instance.PurchaseRewards)
		{
			Dictionary<string, object> pars = new Dictionary<string, object>
			{
				{
					"GearIndex",
					PlayerData.Instance.CurrentBundleGearIndex.Value
				},
				{
					"StringValue",
					reward[0]
				}
			};
			GameObject go = Singleton<PropertyManager>.Instance.InstantiateFromResources(reward[1], Vector3.zero, Quaternion.identity, pars);
			go.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}
}
