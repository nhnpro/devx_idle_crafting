using System;
using UniRx;
using UnityEngine;

public class UITutorialMultiStringSetActive : AlwaysStartBehaviour
{
	[SerializeField]
	private string[] Steps;

	[SerializeField]
	private TutorialSetActiveOp Op;

	[SerializeField]
	private float m_delay;

	private int[] m_stepIndexes;

	public override void AlwaysStart()
	{
		SceneLoader instance = SceneLoader.Instance;
		m_stepIndexes = new int[Steps.Length];
		int i;
		for (i = 0; i < Steps.Length; i++)
		{
			m_stepIndexes[i] = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == Steps[i]);
		}
		UniRx.IObservable<bool> observable = from step in PlayerData.Instance.TutorialStep
			select IsVisible(step);
		if (m_delay != 0f)
		{
			observable = observable.Delay(TimeSpan.FromSeconds(m_delay));
		}
		UniRx.IObservable<bool> left = from step in PlayerData.Instance.TutorialStep
			select IsVisible(step);
		left.CombineLatest(observable, (bool a, bool b) => a && b).DistinctUntilChanged().TakeUntilDestroy(instance)
			.SubscribeToActiveUntilNull(base.gameObject)
			.AddTo(this);
	}

	private bool IsVisible(int step)
	{
		for (int i = 0; i < m_stepIndexes.Length; i++)
		{
			if (i < m_stepIndexes.Length - 1)
			{
				if (step == m_stepIndexes[i])
				{
					return true;
				}
				continue;
			}
			switch (Op)
			{
			case TutorialSetActiveOp.Equals:
				return step == m_stepIndexes[i];
			case TutorialSetActiveOp.GreaterOrEq:
				return step >= m_stepIndexes[i];
			case TutorialSetActiveOp.Greater:
				return step > m_stepIndexes[i];
			case TutorialSetActiveOp.LessOrEq:
				return step <= m_stepIndexes[i];
			}
		}
		return false;
	}

	public void OnNextTutorial()
	{
		Singleton<TutorialGoalCollectionRunner>.Instance.NextTutorialGoal();
	}
}
