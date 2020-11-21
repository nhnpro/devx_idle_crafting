using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSkipRewardsParent : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(PopulateSkipRewards());
	}

	private IEnumerator PopulateSkipRewards()
	{
		yield return null;
		base.transform.DestroyChildrenImmediate();
		for (int j = 0; j < Singleton<LevelSkipRunner>.Instance.SkippedResources.Length; j++)
		{
			if (Singleton<LevelSkipRunner>.Instance.SkippedResources[j] > 0)
			{
				Dictionary<string, object> pars = new Dictionary<string, object>
				{
					{
						"StringValue",
						Singleton<LevelSkipRunner>.Instance.SkippedResources[j].ToString()
					}
				};
				GameObject go = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/GoatSkipMaterialProfiles/GoatMaterial_" + (BlockType)j, Vector3.zero, Quaternion.identity, pars);
				go.transform.SetParent(base.transform, worldPositionStays: false);
				yield return null;
			}
		}
		for (int i = Singleton<LevelSkipRunner>.Instance.SkippedCompanions[0]; i < Singleton<LevelSkipRunner>.Instance.SkippedCompanions[1]; i++)
		{
			Dictionary<string, object> pars2 = new Dictionary<string, object>
			{
				{
					"HeroIndex",
					i
				}
			};
			GameObject go2 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/MinimapBubbles/MInimap_NewCompanionBubble", Vector3.zero, Quaternion.identity, pars2);
			go2.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}
}
