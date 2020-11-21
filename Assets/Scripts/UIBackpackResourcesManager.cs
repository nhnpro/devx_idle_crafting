using System.Collections.Generic;
using UnityEngine;

public class UIBackpackResourcesManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_inventoryResourcePrefab;

	[SerializeField]
	private GameObject m_emptyNotification;

	protected void OnEnable()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		bool active = true;
		for (int i = 0; i < 7; i++)
		{
			long num = 0L;
			if (i != 5)
			{
				num = PlayerData.Instance.BlocksInBackpack[i].Value;
			}
			if (num > 0)
			{
				dictionary.Clear();
				dictionary.Add("BlockResourceRunner", Singleton<GearCollectionRunner>.Instance.GetOrCreateBlockResourceRunner((BlockType)i));
				Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("UI/InventoryResources/InventoryItem_" + (BlockType)i + "_Gray", Vector3.zero, Quaternion.identity, dictionary, base.transform);
				active = false;
			}
		}
		m_emptyNotification.SetActive(active);
	}
}
