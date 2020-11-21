using UnityEngine;

public class UIOpenTournamentComingUp : MonoBehaviour
{
	public void OnOpenTournamentComingUpOrAvailable()
	{
		if (Singleton<TournamentRunner>.Instance.TournamentAccessable.Value)
		{
			BindingManager.Instance.TournamentAvailableParent.ShowInfo();
		}
		else
		{
			BindingManager.Instance.TournamentComingUpParent.ShowInfo();
		}
	}
}
