/*
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ForceUpdateService
{
	[CompilerGenerated]
	private static PopupMessage.OnMessagePopupComplete _003C_003Ef__mg_0024cache0;

	public static void ShowForceUpdateDialog()
	{
		PopupMessage.onMessagePopupComplete += OnForceUpdateDialogClose;
		PopupMessage.Create(PersistentSingleton<LocalizationService>.Instance.Text("ForceUpdate.Title"), PersistentSingleton<LocalizationService>.Instance.Text("ForceUpdate.Body"), PersistentSingleton<LocalizationService>.Instance.Text("ForceUpdate.Button"));
	}

	public static void OnForceUpdateDialogClose(MessageState state)
	{
		Application.OpenURL(PersistentSingleton<GameSettings>.Instance.AndroidUpdateUrl);
		ShowForceUpdateDialog();
	}
}
*/
