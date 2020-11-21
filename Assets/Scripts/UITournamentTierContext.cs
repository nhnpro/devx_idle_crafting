using UnityEngine;
using UnityEngine.UI;

public class UITournamentTierContext : MonoBehaviour
{
	private enum Placement
	{
		Title,
		Description
	}

	[SerializeField]
	private TournamentTier m_tier;

	[SerializeField]
	private Placement m_placement;

	private void OnEnable()
	{
		Text component = base.gameObject.GetComponent<Text>();
		TournamentTier tier = m_tier;
		if (m_tier == TournamentTier.Invalid)
		{
			tier = (TournamentTier)PlayerData.Instance.Trophies.Value;
		}
		if (m_placement == Placement.Title)
		{
			component.text = PersistentSingleton<LocalizationService>.Instance.Text("UI.Tournament.Trophies." + tier.ToString());
			return;
		}
		TournamentTierConfig tournamentTierConfig = Singleton<EconomyHelpers>.Instance.GetTournamentTierConfig((int)tier);
		component.text = BonusTypeHelper.GetAttributeText(tournamentTierConfig.Bonus.BonusType, tournamentTierConfig.Bonus.Amount);
	}
}
