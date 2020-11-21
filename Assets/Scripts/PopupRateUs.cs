/*
using System;
using UnityEngine;

public class PopupRateUs : MonoBehaviour
{
	public delegate void OnRateUSPopupComplete(MessageState state);

	public string title;

	public string message;

	public string rate;

	public string remind;

	public string declined;

	public string appId;

	public static event OnRateUSPopupComplete onRateUSPopupComplete;

	private void RaiseOnOnRateUSPopupComplete(MessageState state)
	{
		if (PopupRateUs.onRateUSPopupComplete != null)
		{
			PopupRateUs.onRateUSPopupComplete(state);
		}
	}

	public static PopupRateUs Create()
	{
		return Create("Like the Game?", "Rate US");
	}

	public static PopupRateUs Create(string title, string message)
	{
		return Create(title, message, "Rate Now", "Ask me later", "No, thanks");
	}

	public static PopupRateUs Create(string title, string message, string rate, string remind, string declined)
	{
		PopupRateUs popupRateUs = new GameObject("AndroidRateUsPopUp").AddComponent<PopupRateUs>();
		popupRateUs.title = title;
		popupRateUs.message = message;
		popupRateUs.rate = rate;
		popupRateUs.remind = remind;
		popupRateUs.declined = declined;
		popupRateUs.Init();
		return popupRateUs;
	}

	public void Init()
	{
		// AndroidNative.showRateUsPopUP(title, message, rate, remind, declined);
	}

	public void OnRatePopUpCallBack(string buttonIndex)
	{
		switch (Convert.ToInt16(buttonIndex))
		{
		case 0:
			RaiseOnOnRateUSPopupComplete(MessageState.RATED);
			break;
		case 1:
			RaiseOnOnRateUSPopupComplete(MessageState.REMIND);
			break;
		case 2:
			RaiseOnOnRateUSPopupComplete(MessageState.DECLINED);
			break;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
*/
