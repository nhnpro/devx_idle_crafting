using UnityEngine;
using UnityEngine.UI;

public class UITournamentProgressParent : MonoBehaviour
{
	[SerializeField]
	private Slider m_progressSlider;

	[SerializeField]
	private Text m_progressMedalsText;

	[SerializeField]
	private GameObject[] m_steps = new GameObject[4];

	private void OnEnable()
	{
		TournamentTierConfig tournamentTierConfig = Singleton<EconomyHelpers>.Instance.GetTournamentTierConfig(PlayerData.Instance.Trophies.Value + 1);
		m_progressMedalsText.text = PlayerData.Instance.Medals.Value + "/" + tournamentTierConfig.Requirement;
		float num = (float)PlayerData.Instance.Medals.Value / (float)tournamentTierConfig.Requirement;
		m_progressSlider.value = num;
		int num2 = Mathf.Min(Mathf.FloorToInt(num / 0.25f), m_steps.Length);
		for (int i = 0; i < num2; i++)
		{
			m_steps[i].SetActive(value: true);
		}
	}
}
