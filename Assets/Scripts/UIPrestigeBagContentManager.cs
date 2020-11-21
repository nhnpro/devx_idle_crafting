using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrestigeBagContentManager : MonoBehaviour
{
	public void InitializeCards()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		for (int i = 0; i < 7; i++)
		{
			if (PlayerData.Instance.BlocksInBackpack[i].Value > 0)
			{
				dictionary.Clear();
				dictionary.Add("BlockResourceRunner", Singleton<GearCollectionRunner>.Instance.GetOrCreateBlockResourceRunner((BlockType)i));
				GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("UI/InventoryResources/InventoryItem_" + (BlockType)i, Vector3.zero, Quaternion.identity, dictionary);
				gameObject.SetActive(value: false);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}

	protected void OnEnable()
	{
		StartCoroutine(AnimateCards());
	}

	private IEnumerator AnimateCards()
	{
		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			elapsedTime += Time.deltaTime;
			if (Input.GetMouseButtonDown(0))
			{
				elapsedTime = 1f;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject card = base.transform.GetChild(i).gameObject;
			card.SetActive(value: true);
			float cardTime = 0f;
			while (cardTime < 2.5f)
			{
				cardTime += Time.deltaTime;
				if (Input.GetMouseButtonDown(0))
				{
					cardTime = 2.5f;
					card.GetComponent<Animator>().SetTrigger("Skip");
					yield return null;
				}
				else
				{
					yield return null;
				}
			}
		}
		Singleton<PrestigeRunner>.Instance.CompletePrestigeSequence();
	}
}
