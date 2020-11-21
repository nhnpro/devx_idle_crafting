using System.Collections.Generic;
using UniRx;

public class GiftRewardRunner : Singleton<GiftRewardRunner>
{
	public List<RewardAction> RewardActions = new List<RewardAction>();

	public GiftRewardRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<FacebookRunner>.Instance.GiftConsumeSuccess.Subscribe(delegate(FacebookGift gift)
		{
			GiveGift(gift);
		}).AddTo(instance);
		(from pair in Singleton<FacebookRunner>.Instance.GiftOpenInProgress.DelayFrame(1).Pairwise()
			where pair.Current == 0 && pair.Previous > 0
			select pair).Subscribe(delegate
		{
			ShowRewardOpening();
		}).AddTo(instance);
	}

	private void GiveGift(FacebookGift gift)
	{
		GiveRandomReward(gift.FromId);
	}

	private void GiveRandomReward(string friendId)
	{
		RewardData reward = PersistentSingleton<Economies>.Instance.Gifts.AllotObject();
		int numUnlockedHeroes = Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(PlayerData.Instance.LifetimeChunk.Value);
		RewardAction item = RewardFactory.ToRewardAction(reward, RarityEnum.Common, numUnlockedHeroes, friendId);
		RewardActions.Add(item);
	}

	private void ShowRewardOpening()
	{
		Singleton<FakeFundRunner>.Instance.CopyFunds();
		foreach (RewardAction rewardAction in RewardActions)
		{
			rewardAction.GiveReward();
		}
		BindingManager.Instance.GiftRewardParent.ShowInfo();
	}

	public void CompleteRewardSequence()
	{
		RewardActions.Clear();
	}
}
