using MiniJSON;
using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using UniRx;

public class AppsFlyerService : PersistentSingleton<AppsFlyerService>
{
	public void InitializeAppsFlyer()
	{
		if (!base.Inited)
		{
			AppsFlyer.initSDK("nr8SibwpFjcKGBQNpDdttd", "com.futureplay.minecraft");
			AppsFlyer.setCustomerUserId(PlayerData.Instance.PlayerId);
			AppsFlyer.trackAppLaunch();
			PlayerData.Instance.DaysRetained.Skip(1).Subscribe(delegate(int days)
			{
				AppsFlyer.trackRichEvent("Retained_Day", new Dictionary<string, string>
				{
					{
						"Day",
						days.ToString()
					}
				});
			});
			(from res in PersistentSingleton<AdService>.Instance.AdResults
				where res.result == AdService.V2PShowResult.Finished
				select res).Subscribe(delegate(AdWatched adWatched)
			{
				AppsFlyer.trackRichEvent("Ad_Watched", adWatched.asDictionary());
			});
			base.Inited = true;
		}
	}

	private void ServerSideValidation(IAPTransactionState state)
	{
		try
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(state.ReceiptRawData);
			string json = (string)dictionary["Payload"];
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)Json.Deserialize(json);
			string purchaseData = (string)dictionary2["json"];
			string signature = (string)dictionary2["signature"];
			AppsFlyer.validateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjAYZVERoP4E4HwFAtzZt3JbsfNsiJ+MihJ9bSMjStq0zFo6gXUMc0Ubl4TCgw69UYMt7pR+2ghbTdnR0UvJ/CKhLYpqHhZMp9nQUmGZS+L9Yyl6gAn7Z4xN3kdoVh0fIg8nu0rn3/7hQoUO+7yjZ8usAWubnxRbg4cLL1r5UFwPR+lE7r2rFPdbP3YdFodUfxGdRQVZNFZtZHb4dAW7bZP6sK/UncD49hYkfonkER9cYwJkFpEkXMIl/mxN8I8Vrdro11PSTfhNO9PxqabjZZwPICxSO1pYYPpFehONESezhIf8XPvJ/qMn3PlF/J14IsGOAFOzN3ul3xf67euRMHQIDAQAB", purchaseData, signature, state.Product.metadata.localizedPrice.ToString(), state.Product.metadata.isoCurrencyCode, new Dictionary<string, string>());
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Failed to send receipt validation event to Appsflyer: " + ex.Message);
		}
	}
}
