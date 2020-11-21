using System;
using UniRx;
using UnityEngine;

public class UITutorialActivate : AlwaysStartBehaviour
{
	[SerializeField]
	private string Step;

	[SerializeField]
	private float m_delay;

	public override void AlwaysStart()
	{
		SceneLoader instance = SceneLoader.Instance;
		int stepIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == Step);
		UniRx.IObservable<bool> observable = from step in PlayerData.Instance.TutorialStep
			select step == stepIndex;
		if (m_delay != 0f)
		{
			observable = observable.Delay(TimeSpan.FromSeconds(m_delay));
		}
		UniRx.IObservable<bool> left = from step in PlayerData.Instance.TutorialStep
			select step == stepIndex;
		(from turn in left.CombineLatest(observable, (bool a, bool b) => a && b).DistinctUntilChanged()
			where turn
			select turn).TakeUntilDestroy(instance).SubscribeToActiveUntilNull(base.gameObject).AddTo(this);
	}
}
