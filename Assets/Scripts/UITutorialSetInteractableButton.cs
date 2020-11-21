using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UITutorialSetInteractableButton : AlwaysStartBehaviour
{
	[SerializeField]
	private string Step;

	[SerializeField]
	private TutorialSetActiveOp Op;

	[SerializeField]
	private float m_delay;

	private int m_stepIndex;

	private Button m_Button;

	public override void AlwaysStart()
	{
		m_Button = GetComponent<Button>();
		SceneLoader instance = SceneLoader.Instance;
		m_stepIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == Step);
		UniRx.IObservable<bool> observable = from step in PlayerData.Instance.TutorialStep
			select IsVisible(step);
		if (m_delay != 0f)
		{
			observable = observable.Delay(TimeSpan.FromSeconds(m_delay));
		}
		UniRx.IObservable<bool> left = from step in PlayerData.Instance.TutorialStep
			select IsVisible(step);
		left.CombineLatest(observable, (bool a, bool b) => a && b).DistinctUntilChanged().TakeUntilDestroy(instance)
			.SubscribeToInteractable(m_Button)
			.AddTo(this);
	}

	private bool IsVisible(int step)
	{
		switch (Op)
		{
		case TutorialSetActiveOp.Equals:
			return step == m_stepIndex;
		case TutorialSetActiveOp.GreaterOrEq:
			return step >= m_stepIndex;
		case TutorialSetActiveOp.Greater:
			return step > m_stepIndex;
		case TutorialSetActiveOp.LessOrEq:
			return step <= m_stepIndex;
		default:
			return false;
		}
	}

	public void OnNextTutorial()
	{
		Singleton<TutorialGoalCollectionRunner>.Instance.NextTutorialGoal();
	}
}
