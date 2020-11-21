using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChestContentManager : MonoBehaviour
{
	protected void OnEnable()
	{
		List<GameObjectAndAction> list = new List<GameObjectAndAction>();
		base.transform.DestroyChildrenImmediate();
		foreach (RewardAction rewardAction in Singleton<ChestRunner>.Instance.RewardActions)
		{
			GameObject gameObject = RewardContentHelper.CreateCard(rewardAction);
			gameObject.SetActive(value: false);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			list.Add(new GameObjectAndAction
			{
				GO = gameObject,
				Action = rewardAction
			});
		}
		StartCoroutine(AnimateRewards(list));
	}

	public IEnumerator AnimateRewards(List<GameObjectAndAction> objects)
	{
		float elapsedTime = 0f;
		while (elapsedTime < 2f)
		{
			elapsedTime += Time.deltaTime;
			if (Input.GetMouseButtonDown(0))
			{
				elapsedTime = 2f;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		foreach (GameObjectAndAction @object in objects)
		{
			GameObjectAndAction ga = @object;
			GameObject card = ga.GO;
			if (card != null)
			{
				ga.Action.GiveFakeReward();
				card.SetActive(value: true);
			}
			float cardTime = 0f;
			while (cardTime < 0.6f)
			{
				cardTime += Time.deltaTime;
				if (Input.GetMouseButtonDown(0))
				{
					cardTime = 0.6f;
					yield return null;
				}
				else
				{
					yield return null;
				}
			}
		}
		Singleton<ChestRunner>.Instance.CompleteChestSequence();
	}
}
