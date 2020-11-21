using System.Runtime.CompilerServices;
using UnityEngine;

public static class AcceptOurTermsService
{
	/*[CompilerGenerated]
	private static PopupDialog.OnDialogPopupComplete _003C_003Ef__mg_0024cache0;

	public static void ShowTOSAndPPPopup()
	{
		PopupDialog.onDialogPopupComplete += OnTOSAndPPPopupClose;
		PopupDialog.Create(PersistentSingleton<LocalizationService>.Instance.Text("System.PrivacyPolicy.Title"), PersistentSingleton<LocalizationService>.Instance.Text("System.PrivacyPolicy.Notice"), PersistentSingleton<LocalizationService>.Instance.Text("System.PrivacyPolicy.Agree"), PersistentSingleton<LocalizationService>.Instance.Text("System.PrivacyPolicy.TermsOfService"));
	}
	*/

	public static void OnTOSAndPPPopupClose(MessageState state)
	{
		switch (state)
		{
		case MessageState.YES:
			PlayerData.Instance.AcceptedVersion = PersistentSingleton<GameSettings>.Instance.MinimumAcceptedVersion;
			break;
		case MessageState.NO:
			Application.OpenURL(PersistentSingleton<GameSettings>.Instance.TermsOfServicePage);
			// ShowTOSAndPPPopup();
			break;
		}
	}

	public static void ShowTOSAndPPPopup()
	{
		Debug.LogError("ShowTOSAndPPPopup");
	}
}
