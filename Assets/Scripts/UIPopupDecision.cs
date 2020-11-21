using UnityEngine;

public class UIPopupDecision : MonoBehaviour
{
	[SerializeField]
	private string popup;

	public void OnPopupClose()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "close");
	}

	public void OnPopupAd()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "watchAd");
	}

	public void OnOfferClose()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "close_" + ((IAPProductEnum)PlayerData.Instance.CurrentBundleEnum.Value).ToString());
	}

	public void OnAcceptNotification()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "notification");
	}

	public void OnRateApp()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "rateApp");
	}

	public void OnGiveFeedback()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "giveFeedback");
	}

	public void OnXpromo()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "xpromo");
	}

	public void OnEnterAR()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "levelEditor_AR");
	}

	public void OnEnterSimpleEditor()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "levelEditor_Simple");
	}

	public void OnBuyLevelSkip()
	{
		PersistentSingleton<GameAnalytics>.Instance.PopupDecisions.Value = new PopupDecision(popup, "boughtSkip");
	}
}
