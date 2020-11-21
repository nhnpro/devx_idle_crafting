using UnityEngine;
using UnityEngine.UI;

public class UITournamentManager : MonoBehaviour
{
	[SerializeField]
	private Text m_nameField;

	public void OnJoinTournament()
	{
		Singleton<TournamentRunner>.Instance.JoinTournament();
	}

	public void SetDisplayName()
	{
		PlayerData.Instance.DisplayName.SetValueAndForceNotify(m_nameField.text);
	}

	public void OnClaimTournamentRewards()
	{
		Singleton<TournamentRunner>.Instance.ClaimTournamentPrice();
	}

	public void OnShowAllTournamentRewards()
	{
		BindingManager.Instance.TournamentRewardsParent.ShowInfo();
	}

	public void OnTryToBuyTrophy()
	{
		Singleton<TournamentRunner>.Instance.TryToBuyTrophy();
	}
}
