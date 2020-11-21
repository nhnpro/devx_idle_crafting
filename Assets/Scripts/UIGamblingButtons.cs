using UnityEngine;

public class UIGamblingButtons : MonoBehaviour
{
	public void OnFirstCardGamble()
	{
		Singleton<GamblingRunner>.Instance.GambleForReward(0);
	}

	public void OnSecondCardGamble()
	{
		Singleton<GamblingRunner>.Instance.GambleForReward(1);
	}

	public void OnThirdCardGamble()
	{
		Singleton<GamblingRunner>.Instance.GambleForReward(2);
	}

	public void OnFourthCardGamble()
	{
		Singleton<GamblingRunner>.Instance.GambleForReward(3);
	}

	public void OnContinueToNext()
	{
		Singleton<GamblingRunner>.Instance.ContinueToNextLevel();
	}

	public void OnEmptyRewards()
	{
		Singleton<GamblingRunner>.Instance.EmptyRewards();
	}

	public void OnClaimRewards()
	{
		Singleton<GamblingRunner>.Instance.ClaimRewards();
	}

	public void OnPayForFail()
	{
		Singleton<GamblingRunner>.Instance.PayForFail();
	}
}
