using Big;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UITournamentTrophyProgressManager : MonoBehaviour
{
	[SerializeField]
	private Text m_medalsEarnedText;

	[SerializeField]
	private Slider m_topProgressSlider;

	[SerializeField]
	private Slider m_bottomProgressSlider;

	[SerializeField]
	private Text m_progressMedalsText;

	[SerializeField]
	private AnimationCurve m_progressCurve;

	[SerializeField]
	private float m_timeBeforeAnimation = 0.5f;

	[SerializeField]
	private float m_animationTime = 1f;

	[SerializeField]
	private GameObject[] m_steps = new GameObject[4];

	private TournamentTierConfig m_ttc;

	private void OnEnable()
	{
		m_ttc = Singleton<EconomyHelpers>.Instance.GetTournamentTierConfig(PlayerData.Instance.Trophies.Value + 1);
		if (m_ttc != null)
		{
			StartCoroutine(TrophyProgressSequence());
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator TrophyProgressSequence()
	{
		BigDouble amount = Singleton<EconomyHelpers>.Instance.GetTournamentPriceBracket(Singleton<TournamentRunner>.Instance.PlayerRank.Value).Rewards.First((RewardData rew) => rew.Type == RewardEnum.AddToMedals).Amount;
		m_medalsEarnedText.text = amount.ToString();
		m_topProgressSlider.gameObject.SetActive(value: false);
		m_bottomProgressSlider.gameObject.SetActive(value: false);
		m_progressMedalsText.gameObject.SetActive(value: false);
		yield return new WaitForSeconds(m_timeBeforeAnimation);
		float startAmount = (float)PlayerData.Instance.Medals.Value - amount.ToFloat();
		float startPoint = startAmount / (float)m_ttc.Requirement;
		float endAmount = PlayerData.Instance.Medals.Value;
		float endPoint = endAmount / (float)m_ttc.Requirement;
		m_topProgressSlider.value = startPoint;
		m_topProgressSlider.gameObject.SetActive(value: true);
		m_bottomProgressSlider.value = startPoint;
		m_bottomProgressSlider.gameObject.SetActive(value: true);
		m_progressMedalsText.text = (int)startAmount + "/" + m_ttc.Requirement;
		m_progressMedalsText.gameObject.SetActive(value: true);
		int activateAmount = Mathf.Min(Mathf.FloorToInt(startPoint / 0.25f), m_steps.Length);
		for (int i = 0; i < activateAmount; i++)
		{
			m_steps[i].SetActive(value: true);
		}
		float movedFor = 0f;
		while (movedFor <= m_animationTime)
		{
			movedFor += Time.deltaTime;
			float percent = Mathf.Clamp01(movedFor / m_animationTime);
			float curvePercent = m_progressCurve.Evaluate(percent);
			float currentValue = Mathf.Lerp(startPoint, endPoint, curvePercent);
			m_bottomProgressSlider.value = currentValue;
			m_progressMedalsText.text = (int)Mathf.Lerp(startAmount, endAmount, curvePercent) + "/" + m_ttc.Requirement;
			if (Mathf.Min(Mathf.FloorToInt(currentValue / 0.25f), m_steps.Length) > activateAmount)
			{
				int num = Mathf.FloorToInt(currentValue / 0.25f);
				m_steps[num - 1].SetActive(value: true);
				activateAmount = num;
			}
			yield return false;
		}
	}
}
