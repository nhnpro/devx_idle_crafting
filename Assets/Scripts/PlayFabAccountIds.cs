public class PlayFabAccountIds
{
	public string PlayFabId;

	public string FacebookId;

	public string AndroidDeviceId;

	public string IosDeviceId;

	public string CustomId;

	public PlayFabAccountIds(JSONObject loginData)
	{
		JSONObject json = loginData.asJSONObject("InfoResultPayload").asJSONObject("AccountInfo");
		PlayFabId = json.asString("PlayFabId", () => string.Empty);
		AndroidDeviceId = json.asJSONObject("AndroidDeviceInfo").asString("AndroidDeviceId", () => string.Empty);
		IosDeviceId = json.asJSONObject("IosDeviceInfo").asString("IosDeviceId", () => string.Empty);
		CustomId = json.asJSONObject("CustomIdInfo").asString("CustomId", () => string.Empty);
		FacebookId = json.asJSONObject("FacebookInfo").asString("FacebookId", () => string.Empty);
	}
}
