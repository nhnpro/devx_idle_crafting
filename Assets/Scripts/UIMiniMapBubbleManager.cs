using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIMiniMapBubbleManager : MonoBehaviour
{
	private Coroutine m_routine;

	protected void Start()
	{
		(from lvl in Singleton<DrJellyRunner>.Instance.DrJellyLevel
			where lvl > 0
			select lvl).DelayFrame(1).Subscribe(delegate
		{
			if (m_routine != null)
			{
				StopCoroutine(m_routine);
			}
			m_routine = StartCoroutine(Enable());
		}).AddTo(this);
	}

	protected void OnEnable()
	{
		if (m_routine != null)
		{
			StopCoroutine(m_routine);
		}
		m_routine = StartCoroutine(Enable());
	}

	private IEnumerator Enable()
	{
		base.transform.DestroyChildrenImmediate();
		yield return null;
		MapNodeRunner mapnode = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
		int node = mapnode.NodeIndex;
		int companion = Singleton<EconomyHelpers>.Instance.GetNewCreatureInRangeOrNone(PlayerData.Instance.MainChunk.Value, node * 10, (node + 1) * 10 - 1);
		if (companion > -1)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", companion);
			GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/MinimapBubbles/MInimap_NewCompanionBubble", Vector3.zero, Quaternion.identity, dictionary);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}
		yield return null;
		int gear = Singleton<EconomyHelpers>.Instance.GetNewGearSetInRangeOrNone(PlayerData.Instance.LifetimeChunk.Value, node * 10, (node + 1) * 10 - 1);
		if (gear > -1)
		{
			GameObject gameObject2 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/MinimapBubbles/MInimap_NewBlueprintBubble", Vector3.zero, Quaternion.identity, null);
			gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
		}
		yield return null;
		int prestigeRequirement = Singleton<PrestigeRunner>.Instance.PrestigeRequirement.Value - 1;
		if ((node + 1) * 10 - 1 >= PlayerData.Instance.MainChunk.Value && prestigeRequirement >= node * 10 && prestigeRequirement < (node + 1) * 10)
		{
			GameObject gameObject3 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/MinimapBubbles/MInimap_NewPrestigePointBubble", Vector3.zero, Quaternion.identity, null);
			gameObject3.transform.SetParent(base.transform, worldPositionStays: false);
		}
		yield return null;
		int drJelly = Singleton<DrJellyRunner>.Instance.DrJellyLevel.Value;
		if (drJelly >= node * 10 && drJelly < (node + 1) * 10)
		{
			GameObject gameObject4 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/MinimapBubbles/MInimap_NewDrJellyBubble", Vector3.zero, Quaternion.identity, null);
			gameObject4.transform.SetParent(base.transform, worldPositionStays: false);
		}
		yield return null;
	}
}
