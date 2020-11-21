using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIGoalNotificationManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_notificationPrefab;

	private Animator m_animator;

	protected void Start()
	{
		Singleton<TutorialGoalCollectionRunner>.Instance.DelayedCurrentGoal.Subscribe(delegate(PlayerGoalRunner runner)
		{
			SetupTutorialGoal(runner);
		}).AddTo(this);
		(from done in (from runner in Singleton<TutorialGoalCollectionRunner>.Instance.DelayedCurrentGoal
				select runner.ClaimAvailable.AsObservable()).Switch()
			where done
			select done).Subscribe(delegate
		{
			if (m_animator != null)
			{
				m_animator.SetTrigger("GoalCompleted");
			}
		}).AddTo(this);
	}

	private void SetupTutorialGoal(PlayerGoalRunner goalRunner)
	{
		base.transform.DestroyChildrenImmediate();
		if (goalRunner == null || !goalRunner.GoalConfig.GetShowInUI())
		{
			m_animator = null;
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("PlayerGoalRunner", goalRunner);
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_notificationPrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		m_animator = gameObject.GetComponent<Animator>();
	}
}
