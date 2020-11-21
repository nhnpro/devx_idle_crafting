using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGoalContentManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_tutorialPrefab;

	[SerializeField]
	private GameObject m_goalPrefab;

	protected void Start()
	{
		base.transform.DestroyChildrenImmediate();
		SceneLoader.Instance.StartCoroutine(PopulateGoalContents());
	}

	private IEnumerator PopulateGoalContents()
	{
		Dictionary<string, object> pars = new Dictionary<string, object>();
		foreach (PlayerGoalRunner goalRunner2 in Singleton<PlayerGoalCollectionRunner>.Instance.PlayerGoalRunners)
		{
			pars.Clear();
			pars.Add("PlayerGoalRunner", goalRunner2);
			GameObject card2 = Singleton<PropertyManager>.Instance.Instantiate(m_goalPrefab, Vector3.zero, Quaternion.identity, pars);
			card2.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
		for (int i = PersistentSingleton<Economies>.Instance.TutorialGoals.Count - 1; i >= 0; i--)
		{
			PlayerGoalRunner goalRunner = Singleton<TutorialGoalCollectionRunner>.Instance.GetOrCreatePlayerGoalRunner(i);
			if (goalRunner.GoalConfig.GetShowInUI())
			{
				pars.Clear();
				pars.Add("PlayerGoalRunner", goalRunner);
				GameObject card = Singleton<PropertyManager>.Instance.Instantiate(m_tutorialPrefab, Vector3.zero, Quaternion.identity, pars);
				card.transform.SetParent(base.transform, worldPositionStays: false);
				yield return null;
			}
		}
	}

	protected void OnEnable()
	{
		Singleton<PlayerGoalCollectionRunner>.Instance.SeeAllPlayerGoals();
	}
}
