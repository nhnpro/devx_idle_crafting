using UnityEngine;

public class UITutorialSetComplete : MonoBehaviour
{
	[SerializeField]
	private string Step;

	private int m_stepIndex;

	public void Start()
	{
		m_stepIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == Step);
	}

	public void OnNextTutorial()
	{
		Singleton<TutorialGoalCollectionRunner>.Instance.CurrentGoal.Value.GoalAction.Complete();
	}
}
