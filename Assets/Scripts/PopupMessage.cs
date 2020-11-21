/*
using UnityEngine;

public class PopupMessage : MonoBehaviour
{
	public delegate void OnMessagePopupComplete(MessageState state);

	public string title;

	public string message;

	public string ok;

	public static event OnMessagePopupComplete onMessagePopupComplete;

	private void RaiseOnMessagePopupComplete(MessageState state)
	{
		if (PopupMessage.onMessagePopupComplete != null)
		{
			PopupMessage.onMessagePopupComplete(state);
		}
	}

	public static PopupMessage Create(string title, string message)
	{
		return Create(title, message, "Ok");
	}

	public static PopupMessage Create(string title, string message, string ok)
	{
		PopupMessage popupMessage = new GameObject("AndroidMessagePopup").AddComponent<PopupMessage>();
		popupMessage.title = title;
		popupMessage.message = message;
		popupMessage.ok = ok;
		popupMessage.Init();
		return popupMessage;
	}

	public void Init()
	{
		AndroidNative.showMessage(title, message, ok);
	}

	public void OnMessagePopUpCallBack(string buttonIndex)
	{
		RaiseOnMessagePopupComplete(MessageState.OK);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
*/
