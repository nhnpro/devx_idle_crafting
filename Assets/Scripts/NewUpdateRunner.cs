using UnityEngine;

public class NewUpdateRunner : Singleton<NewUpdateRunner>
{
	public NewUpdateRunner()
	{
		if (IsNewUpdate())
		{
			BindingManager.Instance.NewUpdatePopup.SetActive(value: true);
		}
		PlayerData.Instance.LastSeenGameVersion = Application.version;
	}

	private bool IsNewUpdate()
	{
		return ParseVersionToHash(PlayerData.Instance.LastSeenGameVersion) < ParseVersionToHash(Application.version);
	}

	private int ParseVersionToHash(string version)
	{
		string[] array = version.Split('.');
		if (array.Length < 2)
		{
			return 0;
		}
		int result = 0;
		int result2 = 0;
		int.TryParse(array[0], out result);
		int.TryParse(array[1], out result2);
		return (result << 16) + result2;
	}
}
