using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrillRewardManager : MonoBehaviour
{
	protected void OnEnable()
	{
		StartCoroutine(PopulateDrillRewards());
	}

	private IEnumerator PopulateDrillRewards()
	{
		yield return null;
		base.transform.DestroyChildrenImmediate();
		foreach (string[] reward in Singleton<DrillRunner>.Instance.Rewards)
		{
			Dictionary<string, object> pars = new Dictionary<string, object>
			{
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
