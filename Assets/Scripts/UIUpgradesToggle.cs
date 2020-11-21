using UniRx;
using UnityEngine;

public class UIUpgradesToggle : MonoBehaviour
{
	[SerializeField]
	private GameObject m_toggleOne;

	[SerializeField]
	private GameObject m_toggleBigOne;

	[SerializeField]
	private GameObject m_toggleTen;

	[SerializeField]
	private GameObject m_toggleBigTen;

	[SerializeField]
	private GameObject m_toggleMax;

	[SerializeField]
	private GameObject m_toggleBigMax;

	[SerializeField]
	private string StepWhenStopHelping;

	private int m_stepIndex;

	private ReactiveProperty<bool> m_holdHand;

	public void Start()
	{
		m_stepIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == StepWhenStopHelping);
		m_holdHand = (from step in PlayerData.Instance.TutorialStep
			select step < m_stepIndex).TakeUntilDestroy(SceneLoader.Instance).ToReactiveProperty();
	}

	public void OnEnableOne()
	{
		Singleton<UpgradeRunner>.Instance.SetUpgradeStep(UpgradeEnum.One);
		m_toggleOne.SetActive(value: true);
		m_toggleBigOne.SetActive(value: true);
		m_toggleTen.SetActive(value: false);
		m_toggleBigTen.SetActive(value: false);
		m_toggleMax.SetActive(value: false);
		m_toggleBigMax.SetActive(value: false);
	}

	public void OnEnableTen()
	{
		Singleton<UpgradeRunner>.Instance.SetUpgradeStep(UpgradeEnum.Ten);
		m_toggleOne.SetActive(value: false);
		m_toggleBigOne.SetActive(value: false);
		m_toggleTen.SetActive(value: true);
		m_toggleBigTen.SetActive(value: true);
		m_toggleMax.SetActive(value: false);
		m_toggleBigMax.SetActive(value: false);
	}

	public void OnEnableMax()
	{
		Singleton<UpgradeRunner>.Instance.SetUpgradeStep(UpgradeEnum.Max);
		m_toggleOne.SetActive(value: false);
		m_toggleBigOne.SetActive(value: false);
		m_toggleTen.SetActive(value: false);
		m_toggleBigTen.SetActive(value: false);
		m_toggleMax.SetActive(value: true);
		m_toggleBigMax.SetActive(value: true);
	}

	public void OnDisable()
	{
		if (m_holdHand.Value)
		{
			Singleton<UpgradeRunner>.Instance.SetUpgradeStep(UpgradeEnum.One);
			m_toggleOne.SetActive(value: true);
			m_toggleBigOne.SetActive(value: true);
			m_toggleTen.SetActive(value: false);
			m_toggleBigTen.SetActive(value: false);
			m_toggleMax.SetActive(value: false);
			m_toggleBigMax.SetActive(value: false);
		}
	}
}
