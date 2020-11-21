using Big;
using System;
using UniRx;
using UnityEngine;

[PropertyClass]
public class BossBattleRunner : Singleton<BossBattleRunner>
{
	public ReactiveProperty<long> ElapsedTime = new ReactiveProperty<long>(85536000000000L);

	[PropertyInt]
	public ReactiveProperty<int> BattleSecondsLeft = new ReactiveProperty<int>(0);

	[PropertyFloat]
	public ReadOnlyReactiveProperty<float> BattleSecondsLeftNormalized;

	[PropertyFloat]
	public ReadOnlyReactiveProperty<float> BossHealthNormalized;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossSuccessNotification;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossFailedNotification;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossBattleActive;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossPreludeActive;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossFailedActive;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossBattlePending;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> BossLevelActive;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> TryBossAvailable;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> BossCurrentHP = new ReactiveProperty<BigDouble>(1.0);

	[PropertyBigDouble]
	public ReadOnlyReactiveProperty<BigDouble> BossFullHP;

	public ReadOnlyReactiveProperty<bool> BossBattleResult;

	public ReadOnlyReactiveProperty<int> BossMaxDuration;

	[PropertyBool]
	public ReactiveProperty<bool> BossBattlePaused = new ReactiveProperty<bool>(initialValue: false);

	public BossBattleRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		BossMaxDuration = (from duration in Singleton<CumulativeBonusRunner>.Instance.BonusMult[7]
			select PersistentSingleton<GameSettings>.Instance.BossDurationSeconds + duration.ToInt()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		UniRx.IObservable<long> left2 = (from dead in (from health in BossCurrentHP
				select health <= BigDouble.ZERO).DistinctUntilChanged()
			select (!dead) ? TickerService.MasterTicks : Observable.Never<long>()).Switch();
		(from tuple in left2.CombineLatest(BossBattlePaused, (long ticks, bool paused) => new
			{
				ticks,
				paused
			})
			where !tuple.paused
			select tuple).Subscribe(tuple =>
		{
			if (ElapsedTime.Value < 864000000000L)
			{
				ElapsedTime.Value += tuple.ticks;
			}
		}).AddTo(instance);
		BattleSecondsLeft = (from left in ElapsedTime.CombineLatest(BossMaxDuration, (long elapsed, int dur) => dur - (int)(elapsed / 10000000))
			select Mathf.Max(0, left)).TakeUntilDestroy(instance).ToReactiveProperty();
		BattleSecondsLeftNormalized = (from secs in BattleSecondsLeft
			select (float)secs / (float)BossMaxDuration.Value).ToReadOnlyReactiveProperty();
		UniRx.IObservable<bool> source = from secs in BattleSecondsLeft.Skip(1)
			select secs <= 0 into ranOut
			where ranOut
			select ranOut;
		UniRx.IObservable<bool> observable = from killed in (from health in BossCurrentHP
				select health <= BigDouble.ZERO).DistinctUntilChanged()
			where killed
			select killed;
		observable.Subscribe(delegate
		{
			PlayerData.Instance.BossFailedLastTime.Value = false;
			PersistentSingleton<MainSaver>.Instance.PleaseSave("boss_killed_chunk_" + Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index + "_prestige_" + PlayerData.Instance.LifetimePrestiges.Value);
		}).AddTo(instance);
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			select order == PrestigeOrder.PrestigeStart).Subscribe(delegate
		{
			if (BossBattleActive.Value)
			{
				ElapsedTime.Value = 85536000000000L;
			}
			PlayerData.Instance.BossFailedLastTime.Value = false;
		}).AddTo(instance);
		(from seq in Singleton<WorldRunner>.Instance.MapSequence
			where seq
			select seq).Subscribe(delegate
		{
			PlayerData.Instance.BossFailedLastTime.Value = false;
		}).AddTo(instance);
		UniRx.IObservable<bool> observable2 = (from pair in Singleton<ChunkRunner>.Instance.AllBlockAmount.Pairwise()
			select pair.Current == 1 && pair.Previous > 1).CombineLatest(Singleton<ChunkRunner>.Instance.BossBlock, (bool cleared, BossBlockController boss) => cleared && boss != null).StartWith(value: false);
		(from activated in observable2
			where activated
			select activated).Subscribe(delegate
		{
			StartCountdown();
		}).AddTo(instance);
		BossBattleActive = (from secs in BattleSecondsLeft
			select secs > 0).CombineLatest(observable2, (bool time, bool block) => time && block).DistinctUntilChanged().TakeUntilDestroy(instance)
			.ToReadOnlyReactiveProperty();
		BossLevelActive = (from boss in Singleton<ChunkRunner>.Instance.BossBlock
			select boss != null).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from pair in BossLevelActive.Pairwise()
			select pair.Current && !pair.Previous into start
			where start
			select start).Subscribe(delegate
		{
			StartBossLevel();
		}).AddTo(instance);
		BossPreludeActive = BossLevelActive.CombineLatest(Singleton<ChunkRunner>.Instance.AllBlockAmount, (bool level, int blocks) => level && blocks > 1).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BossFailedActive = BossLevelActive.CombineLatest(BossPreludeActive, BossBattleActive, (bool level, bool prelude, bool battle) => level && !prelude && !battle).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TryBossAvailable = PlayerData.Instance.BossFailedLastTime.CombineLatest(BossLevelActive, (bool failed, bool active) => failed && !active).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BossBattlePending = TryBossAvailable.CombineLatest(BossLevelActive, (bool avail, bool level) => avail || level).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BossSuccessNotification = (from _ in observable
			select Observable.Merge(new UniRx.IObservable<bool>[2]
			{
				Observable.Return<bool>(value: true),
				Observable.Return<bool>(value: false).Delay(TimeSpan.FromSeconds(10.0))
			})).Switch().TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BossFailedNotification = (from _ in source
			select Observable.Merge(new UniRx.IObservable<bool>[2]
			{
				Observable.Return<bool>(value: true),
				Observable.Return<bool>(value: false).Delay(TimeSpan.FromSeconds(10.0))
			})).Switch().TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BossBattleResult = (from _ in source
			select false).Merge(observable).StartWith(value: false).ToSequentialReadOnlyReactiveProperty();
		BossFullHP = (from chunk in Singleton<WorldRunner>.Instance.CurrentChunk
			select ChunkRunner.IsLastChunkForNode(chunk.Index)).Select(delegate(bool last)
		{
			BiomeConfig value = Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value;
			return (!last) ? value.MiniBossHP : value.BossHP;
		}).CombineLatest(Singleton<DrJellyRunner>.Instance.DrJellyBattle, (BigDouble hp, bool dr) => (!dr) ? hp : (hp * PersistentSingleton<GameSettings>.Instance.DrJellyHpMult)).TakeUntilDestroy(instance)
			.ToReadOnlyReactiveProperty();
		BossHealthNormalized = (from hp in BossFullHP
			select (!(hp > new BigDouble(1.0))) ? new BigDouble(1.0) : hp).CombineLatest(BossCurrentHP, (BigDouble full, BigDouble current) => (current / full).ToFloat()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from result in BossBattleResult.Skip(1)
			where !result
			select result).Subscribe(delegate
		{
			AudioController.Instance.QueueEvent(new AudioEvent("BossSequenceFailed", AUDIOEVENTACTION.Play));
		}).AddTo(instance);
		(from result in BossBattleResult.Skip(1)
			where result
			select result).Subscribe(delegate
		{
			PlayerData.Instance.RetryLevelNumber.Value = 0;
		}).AddTo(instance);
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			BossBattleResult.Subscribe(delegate(bool result)
			{
				PersistentSingleton<GameAnalytics>.Instance.BossBattleResult.Value = result;
			}).AddTo(instance);
		}
	}

	public void DebugKillBoss()
	{
		CauseDamage(BossFullHP.Value);
	}

	private void StartBossLevel()
	{
		PlayerData.Instance.BossFailedLastTime.Value = true;
	}

	private void StartCountdown()
	{
		ElapsedTime.Value = 0L;
		BossCurrentHP.Value = BossFullHP.Value;
	}

	public bool CauseDamage(BigDouble damage)
	{
		BossCurrentHP.Value -= damage;
		return BossCurrentHP.Value <= BigDouble.ZERO;
	}

	public void LoseBossBattle()
	{
		ElapsedTime.Value = 85536000000000L;
	}

	public void PauseBossBattle()
	{
		BossBattlePaused.Value = true;
	}

	public void UnpauseBossBattle()
	{
		BossBattlePaused.Value = false;
	}
}
