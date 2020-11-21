using Big;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CollectableRewardRunner : Singleton<CollectableRewardRunner>
{
	private const int maxRewardAmount = 6;

	private int spawnedRewards;

	private CollectableSpot[] spots;

	private List<WeightedObject<int>> rewards = new List<WeightedObject<int>>();

	public CollectableRewardRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		rewards.Add(new WeightedObject<int>
		{
			Value = 0,
			Weight = 50f
		});
		rewards.Add(new WeightedObject<int>
		{
			Value = 1,
			Weight = 50f
		});
		rewards.Add(new WeightedObject<int>
		{
			Value = 2,
			Weight = 50f
		});
		rewards.Add(new WeightedObject<int>
		{
			Value = 3,
			Weight = 10f
		});
		(from gears in PlayerData.Instance.LifetimeGears
			where gears > 0
			select gears).Take(1).Subscribe(delegate
		{
			SpawnCollectableRewards(3);
		}).AddTo(instance);
	}

	private void SpawnCollectableRewards(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			SpawnCollectableReward();
		}
		Observable.Return(value: true).Delay(TimeSpan.FromMinutes(UnityEngine.Random.Range(2, 10))).Take(1)
			.Subscribe(delegate
			{
				SpawnCollectableRewards(1);
			})
			.AddTo(SceneLoader.Instance);
	}

	private void SpawnCollectableReward()
	{
		spots = BindingManager.Instance.UI.gameObject.GetComponentsInChildren<CollectableSpot>(includeInactive: true);
		if (spawnedRewards > 6)
		{
			return;
		}
		spawnedRewards++;
		List<CollectableSpot> list = new List<CollectableSpot>();
		if (spots.Length <= 0)
		{
			return;
		}
		CollectableSpot[] array = spots;
		foreach (CollectableSpot collectableSpot in array)
		{
			if (collectableSpot.transform.childCount == 0)
			{
				list.Add(collectableSpot);
			}
		}
		if (list.Count > 0)
		{
			Transform transform = list[UnityEngine.Random.Range(0, list.Count)].transform;
			GameObject gameObject = GameObjectExtensions.InstantiateFromResources("Collectables/StationaryReward_0");
			gameObject.transform.SetParent(transform, worldPositionStays: false);
		}
	}

	public void CollectChestReward(Transform start)
	{
		Singleton<FundRunner>.Instance.AddNormalChests(1, "collectableReward");
		BindingManager.Instance.UIChestsTarget.SlerpFromHud(start.position);
	}

	public void CollectRandomReward(Transform start)
	{
		spawnedRewards--;
		int num = rewards.AllotObject();
		Vector3 position = start.position;
		switch (num)
		{
		case 0:
		{
			BigDouble bigDouble = Singleton<WorldRunner>.Instance.MainBiomeConfig.Value.BlockReward * 100L;
			Singleton<FundRunner>.Instance.AddCoins(bigDouble);
			BindingManager.Instance.UICoinsTarget.SlerpFromHud(position);
			TextSlerpHelper.SlerpUITextBigText(bigDouble, position);
			break;
		}
		case 1:
			Singleton<FundRunner>.Instance.AddKeys(1, "collectableReward");
			BindingManager.Instance.UIKeysTarget.SlerpFromHud(position);
			TextSlerpHelper.SlerpUITextBigText(new BigDouble(1.0), position);
			break;
		case 2:
			Singleton<FundRunner>.Instance.AddGems(1, "collectableReward", "rewards");
			BindingManager.Instance.UIGemsTarget.SlerpFromHud(position);
			TextSlerpHelper.SlerpUITextBigText(new BigDouble(1.0), position);
			break;
		case 3:
			Singleton<FundRunner>.Instance.AddNormalChests(1, "collectableReward");
			BindingManager.Instance.UIChestsTarget.SlerpFromHud(position);
			break;
		}
	}
}
