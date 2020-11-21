using Big;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UICraftingUpgradeManager : MonoBehaviour
{
	protected void Start()
	{
		GearRunner gearRunner = (GearRunner)Singleton<PropertyManager>.Instance.GetContext("GearRunner", base.transform);
		UniRx.IObservable<CraftingRequirement> left = GearCollectionRunner.CreateBlocksObservable();
		left.CombineLatest(gearRunner.UpgradeRequirement, (CraftingRequirement blks, CraftingRequirement req) => req).Subscribe(delegate(CraftingRequirement req)
		{
			SetupResources(req);
		}).AddTo(this);
	}

	private void SetupResources(CraftingRequirement req)
	{
		base.transform.DestroyChildrenImmediate();
		if (req.Resources[6] > 0)
		{
			AddCard(BigString.ToString(new BigDouble(req.Resources[6])), "Relics", PlayerData.Instance.BlocksCollected[6].Value >= req.Resources[6]);
		}
		for (int i = 0; i < 6; i++)
		{
			if (req.Resources[i] > 0)
			{
				string text = BigString.ToString(new BigDouble(req.Resources[i]));
				BlockType blockType = (BlockType)i;
				AddCard(text, blockType.ToString(), PlayerData.Instance.BlocksCollected[i].Value >= req.Resources[i]);
			}
		}
	}

	private void AddCard(string text, string type, bool satisfied)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Clear();
		dictionary.Add("StringValue", text);
		dictionary.Add("BoolValue", satisfied);
		GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/CraftingResources/CraftingResource_" + type, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
