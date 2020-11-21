using System;
using UniRx;
using UnityEngine;

public class AutoMineRunner : Singleton<AutoMineRunner>
{
	public SkillRunner Skill;

	public ReadOnlyReactiveProperty<bool> DamageTriggered;

	private ReactiveProperty<double> m_elapsedTime = new ReactiveProperty<double>(0.0);

	public AutoMineRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Skill = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.AutoMine);
		DamageTriggered = (from _ in (from elapsed in m_elapsedTime
				where elapsed > (double)(1f / PersistentSingleton<GameSettings>.Instance.AutoMinePerSecond)
				select elapsed).Do(delegate
			{
				m_elapsedTime.Value -= 1f / PersistentSingleton<GameSettings>.Instance.AutoMinePerSecond;
			})
			select true).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from pair in Skill.Active.Pairwise()
			where pair.Previous && !pair.Current
			select pair).Subscribe(delegate
		{
			StartTornadoExitAnimation();
		}).AddTo(instance);
		(from active in Skill.Active
			where active
			select active).Subscribe(delegate
		{
			BindingManager.Instance.Tornado.SetActive(value: true);
		}).AddTo(instance);
		(from tornado in Skill.Active.CombineLatest(TickerService.MasterTicks, (bool active, long ticks) => new
			{
				active,
				ticks
			})
			where tornado.active
			select tornado).Subscribe(tornado =>
		{
			m_elapsedTime.Value += TimeSpan.FromTicks(tornado.ticks).TotalSeconds;
		}).AddTo(instance);
	}

	private void StartTornadoExitAnimation()
	{
		BindingManager.Instance.Tornado.GetComponent<Animator>().SetBool("ExitTornado", value: true);
	}
}
