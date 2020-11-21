using Big;
using UniRx;

public static class PlayerGoalActionFactory
{
	public static PlayerGoalAction Create(PlayerGoalConfig config, PlayerGoalState state)
	{
		switch (config.Task)
		{
		case PlayerGoalTask.None:
			return null;
		case PlayerGoalTask.Never:
			return new PlayerGoalActionNever();
		case PlayerGoalTask.Skip:
			return new PlayerGoalActionSkip();
		case PlayerGoalTask.Triggered:
			return new PlayerGoalActionTriggered(state.CompletedStars, state.ClaimedStars, config.StarReq);
		default:
		{
			UniRx.IObservable<BigDouble> rxProperty = ReferenceTaskTarget(config.Task, config.Parameter);
			return new PlayerGoalActionBigDouble(rxProperty, state.ClaimedStars, config.StarReq);
		}
		}
	}

	public static UniRx.IObservable<BigDouble> ReferenceTaskTarget(PlayerGoalTask target, int parameter)
	{
		switch (target)
		{
		case PlayerGoalTask.AmountCoins:
			return PlayerData.Instance.LifetimeCoins;
		case PlayerGoalTask.AmountBlocks:
		{
			UniRx.IObservable<long> observable = from blocks in PlayerData.Instance.LifetimeBlocksDestroyed[0]
				select (blocks);
			for (int i = 1; i < PlayerData.Instance.LifetimeBlocksDestroyed.Count; i++)
			{
				observable.CombineLatest(PlayerData.Instance.LifetimeBlocksDestroyed[i], (long total, long blocks) => total + blocks);
			}
			return from blocks in observable
				select new BigDouble(blocks);
		}
		case PlayerGoalTask.HeroLevel:
			return from lvl in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(parameter).LifetimeLevel
				select new BigDouble(lvl);
		case PlayerGoalTask.NumCreatures:
			return from count in PlayerData.Instance.LifetimeCreatures
				select new BigDouble(count - 1);
		case PlayerGoalTask.NumPrestiges:
			return from count in PlayerData.Instance.LifetimePrestiges
				select new BigDouble(count);
		case PlayerGoalTask.AmountSkillUsed:
			return from count in Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner((SkillsEnum)parameter).LifetimeUsed
				select new BigDouble(count);
		case PlayerGoalTask.NumGears:
			return from unlocks in PlayerData.Instance.LifetimeGears
				select new BigDouble(unlocks);
		case PlayerGoalTask.AmountRelics:
			return from relics in PlayerData.Instance.LifetimeRelics
				select new BigDouble(relics);
		case PlayerGoalTask.HeroTier:
			return from lvl in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(parameter).Tier
				select new BigDouble(lvl);
		case PlayerGoalTask.Chunk:
			return from chunk in PlayerData.Instance.LifetimeChunk
				select new BigDouble(chunk);
		case PlayerGoalTask.OpenChests:
			return from chunk in PlayerData.Instance.LifetimeAllOpenedChests
				select new BigDouble(chunk);
		case PlayerGoalTask.TapBlocks:
			return from taps in PlayerData.Instance.LifetimeBlocksTaps
				select new BigDouble(taps);
		case PlayerGoalTask.StartAdventure:
			return from chunk in PlayerData.Instance.MainChunk
				where chunk > 0
				select chunk into _
				select new BigDouble(PlayerData.Instance.LifetimePrestiges.Value);
		default:
			return null;
		}
	}
}
