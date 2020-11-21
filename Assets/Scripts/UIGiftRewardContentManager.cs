using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGiftRewardContentManager : MonoBehaviour
{
	[SerializeField]
	private Button m_button;

	protected void OnEnable()
	{
		StartCoroutine(PopulateRewards());
	}

	private IEnumerator PopulateRewards()
	{
		yield return null;
		List<GameObjectAndAction> objects = new List<GameObjectAndAction>();
		base.transform.DestroyChildrenImmediate();
		foreach (RewardAction action in Singleton<GiftRewardRunner>.Instance.RewardActions)
		{
			GameObject card = RewardContentHelper.CreateCard(action);
			card.SetActive(value: false);
			card.transform.SetParent(base.transform, worldPositionStays: false);
			objects.Add(new GameObjectAndAction
			{
				GO = card,
				Action = action
			});
			yield return null;
		}
		StartCoroutine(AnimateRewards(objects));
		m_button.interactable = false;
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
		Singleton<GiftRewardRunner>.Instance.CompleteRewardSequence();
		m_button.interactable = true;
	}

	public void OnSendGiftBack()
	{
		Singleton<FacebookRunner>.Instance.GiftBackPlayers(PersistentSingleton<LocalizationService>.Instance.Text("SendGift.Message"), PersistentSingleton<LocalizationService>.Instance.Text("SendGift.Title"));
	}
}
