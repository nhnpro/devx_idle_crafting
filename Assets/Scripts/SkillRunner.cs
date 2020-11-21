using Big;
using System;
using UniRx;
using UnityEngine;

[PropertyClass]
public class SkillRunner
{
	[PropertyBool]
	public ReactiveProperty<bool> Available;

	[PropertyBool]
	public ReactiveProperty<bool> Active;

	[PropertyBool]
	public ReactiveProperty<bool> Locked;

	[PropertyBool]
	public ReactiveProperty<bool> Cooldown;

	[PropertyBool]
	public ReactiveProperty<bool> OutOfStock;

	[PropertyBool]
	public ReactiveProperty<bool> CanAffordPurchase;

	[PropertyInt]
	public ReactiveProperty<int> Cost = new ReactiveProperty<int>(100);

	[PropertyInt]
	public ReactiveProperty<int> Amount;

	[PropertyInt]
	public ReactiveProperty<int> CooldownSeconds = Observable.Never<int>().ToReactiveProperty();

	[PropertyInt]
	public ReactiveProperty<int> SecondsLeft;

	[PropertyInt]
	public ReactiveProperty<int> LevelReq = new ReactiveProperty<int>();

	[PropertyString]
	public ReactiveProperty<string> LocalizedName;

	[PropertyString]
	public ReactiveProperty<string> LocalizedDesc;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> AdAvailable;

	public ReactiveProperty<int> LifetimeUsed;

	public ReadOnlyReactiveProperty<SkillsEnum> UnlockTriggered;

	public ReadOnlyReactiveProperty<int> MaxDuration;

	private SkillConfig m_skillConfig;

	private SkillState m_skillState;

	public SkillsEnum Skill
	{
		get;
		private set;
	}

	public SkillRunner(SkillsEnum skill)
	{
		SceneLoader instance = SceneLoader.Instance;
		Skill = skill;
		m_skillState = SkillStateFactory.GetOrCreateSkillState(skill);
		m_skillConfig = PersistentSingleton<Economies>.Instance.Skills.Find((SkillConfig s) => s.Name == Skill.ToString());
		Cost.Value = PersistentSingleton<GameSettings>.Instance.SkillPurchaseCosts[(int)Skill];
		LevelReq.Value = m_skillConfig.LevelReq;
		LifetimeUsed = m_skillState.LifetimeUsed;
		Locked = (from lvl in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(0).LifetimeLevel
			select lvl < m_skillConfig.LevelReq).TakeUntilDestroy(instance).ToReactiveProperty();
		Amount = m_skillState.Amount.CombineLatest(Locked, (int amount, bool locked) => (!locked) ? amount : 0).TakeUntilDestroy(instance).ToReactiveProperty();
		UnlockTriggered = (from pair in Locked.Pairwise()
			where pair.Previous && !pair.Current
			select pair into _
			select Skill).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		m_skillState.CooldownTimeStamp.Subscribe(delegate
		{
			if (!Locked.Value)
			{
				UpdateCooldown();
			}
		}).AddTo(instance);
		CanAffordPurchase = (from gems in PlayerData.Instance.Gems
			select gems >= Cost.Value).TakeUntilDestroy(instance).ToReactiveProperty();
		MaxDuration = (from duration in Singleton<CumulativeBonusRunner>.Instance.BonusMult[(int)(8 + Skill)]
			select SkillsEnumHelper.IsDuration(Skill) ? duration.ToInt() : 0 into duration
			select m_skillConfig.DurationSeconds + duration).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		TickerService.MasterTicks.Subscribe(delegate(long ticks)
		{
			if (m_skillState.ElapsedTime.Value < 1728000000000L)
			{
				m_skillState.ElapsedTime.Value += ticks;
				UpdateCooldown();
			}
		}).AddTo(instance);
		SecondsLeft = m_skillState.ElapsedTime.CombineLatest(MaxDuration, (long elapsed, int dur) => Mathf.Max(0, dur - (int)(elapsed / 10000000))).DistinctUntilChanged().TakeUntilDestroy(instance)
			.ToReactiveProperty();
		Active = (from secs in SecondsLeft
			select secs > 0).TakeUntilDestroy(instance).ToReactiveProperty();
		(from act in Active.Pairwise()
			where !act.Current && act.Previous
			select act).Subscribe(delegate
		{
			m_skillState.CooldownTimeStamp.Value = ServerTimeService.NowTicks();
		}).AddTo(instance);
		(from act in Active.Pairwise()
			where act.Current && act.Previous
			select act).Subscribe(delegate
		{
			if (PersistentSingleton<GameAnalytics>.Instance != null)
			{
				PersistentSingleton<GameAnalytics>.Instance.SkillUsed.Value = Skill;
			}
		}).AddTo(instance);
		Cooldown = (from secs in CooldownSeconds
			select secs > 0).CombineLatest(Active, (bool cooldown, bool active) => cooldown && !active).TakeUntilDestroy(instance).ToReactiveProperty();
		UniRx.IObservable<bool> observable = Active.CombineLatest(Cooldown, (bool active, bool cooldown) => !active && !cooldown).CombineLatest(Locked, (bool noTimer, bool locked) => noTimer && !locked);
		Available = observable.TakeUntilDestroy(instance).ToReactiveProperty();
		OutOfStock = observable.CombineLatest(Amount, (bool poss, int amount) => poss && amount <= 0).TakeUntilDestroy(instance).ToReactiveProperty();
		LocalizedName = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Skill.Name." + Skill.ToString()));
		LocalizedDesc = new ReactiveProperty<string>(PersistentSingleton<LocalizationService>.Instance.Text("Skill.Name.Desc." + Skill.ToString()));
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			select order == PrestigeOrder.PrestigeStart).Subscribe(delegate
		{
			m_skillState.CooldownTimeStamp.Value = 0L;
			m_skillState.ElapsedTime.Value = 1728000000000L;
		}).AddTo(instance);
		(from amount in Amount.Pairwise()
			where amount.Current > amount.Previous
			select amount).Subscribe(delegate
		{
			ResetCooldown();
		}).AddTo(instance);
		UpdateCooldown();
		AdAvailable = Singleton<AdRunner>.Instance.AdReady.TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFinished
			where ad.ToString() == Skill.ToString()
			select ad).Subscribe(delegate
		{
			SkillAdFinished();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementSkipped
			where ad.ToString() == Skill.ToString()
			select ad).Subscribe(delegate
		{
			SkillAdSkipped();
		}).AddTo(instance);
		(from ad in Singleton<AdRunner>.Instance.AdPlacementFailed
			where ad.ToString() == Skill.ToString()
			select ad).Subscribe(delegate
		{
			SkillAdSkipped();
		}).AddTo(instance);
	}

	public void UpdateCooldown()
	{
		if (ServerTimeService.NowTicks() - m_skillState.CooldownTimeStamp.Value < 0)
		{
			m_skillState.CooldownTimeStamp.Value = ServerTimeService.NowTicks();
		}
		int b = m_skillConfig.CoolDownSeconds - (int)TimeSpan.FromTicks(ServerTimeService.NowTicks() - m_skillState.CooldownTimeStamp.Value).TotalSeconds;
		CooldownSeconds.Value = Mathf.Max(0, b);
	}

	public void ResetCooldown()
	{
		m_skillState.CooldownTimeStamp.Value = 0L;
	}

	public void ActivateSkill()
	{
		if (!Active.Value)
		{
			m_skillState.ElapsedTime.Value = 0L;
			m_skillState.CooldownTimeStamp.Value = ServerTimeService.NowTicks() + TimeSpan.FromSeconds(MaxDuration.Value).Ticks;
			m_skillState.LifetimeUsed.Value++;
		}
	}

	public void BuyMore(SkillsEnum skill)
	{
		if (PlayerData.Instance.Gems.Value < Cost.Value)
		{
			NotEnoughGems();
			BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
		}
		else
		{
			Singleton<FundRunner>.Instance.RemoveGems(Cost.Value, skill.ToString(), "skills");
			AddAmount(PersistentSingleton<GameSettings>.Instance.SkillPurchaseAmount);
		}
	}

	public void NotEnoughGems()
	{
		int missingGems = Cost.Value - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}

	public void AddAmount(int amount)
	{
		m_skillState.Amount.Value += amount;
	}

	public void WatchAd()
	{
		if (Enum.IsDefined(typeof(AdPlacement), Skill.ToString()))
		{
			Singleton<AdRunner>.Instance.ShowAd((AdPlacement)Enum.Parse(typeof(AdPlacement), Skill.ToString(), ignoreCase: true));
		}
	}

	private void SkillAdFinished()
	{
		ResetCooldown();
	}

	private void SkillAdSkipped()
	{
		BindingManager.Instance.SkillInfoManager.ShowInfo(Skill);
	}
}
