using UnityEngine;

public class UICloudSync : MonoBehaviour
{
	public void OnLocalSave()
	{
		Singleton<CloudSyncRunner>.Instance.UploadSaveToCloud(PlayerData.Instance, "sync_local", null, null);
	}

	public void OnCloudSave()
	{
		PersistentSingleton<PlayFabService>.Instance.ClearCmdQueue();
		Singleton<CloudSyncRunner>.Instance.ApplyCloudSaveAndRestart();
	}
}
