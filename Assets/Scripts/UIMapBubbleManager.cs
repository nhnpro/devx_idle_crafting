using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapBubbleManager : MonoBehaviour
{
	private void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		StartCoroutine(Enable());
	}

	private IEnumerator Enable()
	{
		yield return false;
		MapNodeRunner mapnode = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
		int node = mapnode.NodeIndex;
		if (node * 10 + 9 == Singleton<DrJellyRunner>.Instance.DrJellyExitLevel.Value)
		{
			GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/MapDrJellyEscape", Vector3.zero, Quaternion.identity, null);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			Singleton<DrJellyRunner>.Instance.DrJellyExitLevel.SetValueAndForceNotify(-1);
		}
		if ((node + 1) * 10 > Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index)
		{
			int newCreatureInRangeOrNone = Singleton<EconomyHelpers>.Instance.GetNewCreatureInRangeOrNone(PlayerData.Instance.MainChunk.Value, node * 10, (node + 1) * 10 - 1);
			if (newCreatureInRangeOrNone > -1)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("HeroIndex", newCreatureInRangeOrNone);
				GameObject gameObject2 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/NewCompanionBubble", Vector3.zero, Quaternion.identity, dictionary);
				gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
			}
			int newGearSetInRangeOrNone = Singleton<EconomyHelpers>.Instance.GetNewGearSetInRangeOrNone(PlayerData.Instance.LifetimeChunk.Value, node * 10, (node + 1) * 10 - 1);
			if (newGearSetInRangeOrNone > -1)
			{
				GameObject gameObject3 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/NewBlueprintBubble", Vector3.zero, Quaternion.identity, null);
				gameObject3.transform.SetParent(base.transform, worldPositionStays: false);
			}
			int num = Singleton<PrestigeRunner>.Instance.PrestigeRequirement.Value - 1;
			if ((node + 1) * 10 - 1 >= PlayerData.Instance.MainChunk.Value && num >= node * 10 && num < (node + 1) * 10)
			{
				GameObject gameObject4 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/NewPrestigePointBubble", Vector3.zero, Quaternion.identity, null);
				gameObject4.transform.SetParent(base.transform, worldPositionStays: false);
			}
			int value = Singleton<DrJellyRunner>.Instance.DrJellyLevel.Value;
			if (value >= node * 10 && value < (node + 1) * 10)
			{
				GameObject gameObject5 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/NewDrJellyBubble", Vector3.zero, Quaternion.identity, null);
				gameObject5.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}
}
