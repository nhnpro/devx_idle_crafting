using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlockResourcesManager : MonoBehaviour
{
	protected void OnEnable()
	{
		StartCoroutine(OnEnableRoutine());
	}

	private IEnumerator OnEnableRoutine()
	{
		yield return false;
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> pars = new Dictionary<string, object>();
		for (int i = 0; i < 7; i++)
		{
			long num = 0L;
			if (i != 5)
			{
				num = PlayerData.Instance.BlocksCollected[i].Value;
			}
			if (num > 0)
			{
				pars.Clear();
				pars.Add("BlockResourceRunner", Singleton<GearCollectionRunner>.Instance.GetOrCreateBlockResourceRunner((BlockType)i));
				Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("UI/SmeltedProductsInventory/InventoryItem_" + (BlockType)i, Vector3.zero, Quaternion.identity, pars, base.transform);
			}
		}
	}
}
