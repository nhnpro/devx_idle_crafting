using Big;
using System.Collections.Generic;
using UniRx;

[PropertyFormerlyAs("TamedCreatureRunner")]
[PropertyClass]
public class BossSuccessRunner : Singleton<BossSuccessRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> ApplyAvailable = new ReactiveProperty<bool>();

	public ISubject<bool> SequenceClosed = new Subject<bool>();

	public List<RewardAction> RewardActions = new List<RewardAction>();

	private HeroRunner m_hero;

	private ReadOnlyReactiveProperty<BigDouble> m_coinReward;

	private ReadOnlyReactiveProperty<BigDouble> m_relicReward;

	public BossSuccessRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from success in Singleton<BossBattleRunner>.Instance.BossSuccessNotification
			where success
			select success into _
			select Singleton<BossIndexRunner>.Instance.CurrentBossIndex.Value).Subscribe(delegate(int bossIndex)
		{
			StartRewardSequence(bossIndex);
		}).AddTo(instance);
		m_coinReward = (from chunk in Singleton<WorldRunner>.Instance.CurrentChunk
			select ChunkRunner.IsLastChunkForNode(chunk.Index)).Select(delegate(bool last)
		{
			BiomeConfig value = Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value;
			return (!last) ? value.MiniBossReward : value.BossReward;
		}).CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[2], (BigDouble coins, BigDouble mult) => coins * mult).CombineLatest(Singleton<DrJellyRunner>.Instance.DrJellyBattle, (BigDouble coins, bool dr) => (!dr) ? coins : (coins * PersistentSingleton<GameSettings>.Instance.DrJellyRewardMult))
			.TakeUntilDestroy(instance)
			.ToReadOnlyReactiveProperty();
		m_relicReward = (from reward in (from chunk in Singleton<WorldRunner>.Instance.CurrentChunk
				select Singleton<EconomyHelpers>.Instance.GetRelicsFromBoss(chunk.Index)).CombineLatest(PlayerData.Instance.BoostersEffect[2], (BigDouble relic, float mult) => relic * mult).CombineLatest(Singleton<DrJellyRunner>.Instance.DrJellyBattle, (BigDouble relics, bool dr) => (!dr) ? relics : (relics * PersistentSingleton<GameSettings>.Instance.DrJellyRewardMult))
			select new BigDouble(reward.ToLong())).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	private void StartRewardSequence(int heroIndex)
	{
		ApplyAvailable.Value = true;
		RewardActions.Clear();
		RewardData reward = new RewardData(RewardEnum.AddToCoins, m_coinReward.Value);
		RewardActions.Add(RewardFactory.ToRewardAction(reward, RarityEnum.Common));
		if (m_relicReward.Value > 0L)
		{
			RewardData reward2 = new RewardData(RewardEnum.AddToRelics, m_relicReward.Value);
			RewardActions.Add(RewardFactory.ToRewardAction(reward2, RarityEnum.Common));
		}
		BindingManager.Instance.BossSuccessManager.ShowReward(heroIndex);
	}

	public void AddToTeam()
	{
		GiveRewards();
		CloseSequence();
	}

	private void GiveRewards()
	{
		if (ApplyAvailable.Value)
		{
			ApplyAvailable.Value = false;
			foreach (RewardAction rewardAction in RewardActions)
			{
				rewardAction.GiveReward();
			}
		}
	}

	public void Collect()
	{
		GiveRewards();
		SceneLoader instance = SceneLoader.Instance;
		Observable.Return<bool>(value: true).Subscribe(delegate
		{
			CloseSequence();
		}).AddTo(instance);
	}

	private void CloseSequence()
	{
		SequenceClosed.OnNext(value: true);
		BindingManager.Instance.BossSuccessManager.transform.DestroyChildrenImmediate();
	}
}
