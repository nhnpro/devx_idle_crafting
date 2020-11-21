using System.Collections.Generic;
using System.IO;
using UniRx;

public class ARService : PersistentSingleton<ARService>
{
	public const string AR_FILE = "/ARLevel.idl";

	public const int MinBlockAmount = 30;

	public const int MaxBlockAmount = 500;

	public ReactiveProperty<List<JSONObject>> ARLevel = Observable.Never<List<JSONObject>>().ToReactiveProperty();

	public ReactiveProperty<bool> ShowCustomLevel = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<string> LevelEditorEvent = Observable.Never<string>().ToReactiveProperty();

	public static bool iOSHasCameraPermission()
	{
		return false;
	}

	public void InitializeCustomLevels()
	{
		if (!base.Inited)
		{
			LoadARLevel("/ARLevel.idl");
			ShowCustomLevel.Subscribe(delegate
			{
				LoadARLevel("/ARLevel.idl");
			});
			base.Inited = true;
		}
	}

	public void DebugDeleteARLevel()
	{
		if (File.Exists(PersistentDataPath.Get() + "/ARLevel.idl"))
		{
			ARLevel.Value = null;
			File.Delete(PersistentDataPath.Get() + "/ARLevel.idl");
		}
	}

	private void LoadARLevel(string fileName)
	{
		if (File.Exists(PersistentDataPath.Get() + fileName))
		{
			string cipherText = File.ReadAllText(PersistentDataPath.Get() + "/ARLevel.idl");
			string str = Encryptor.Decrypt(cipherText);
			JSONObject jSONObject = new JSONObject(str);
			int num = jSONObject.asInt("Amount", () => 0);
			if (num < 30 || num > 500)
			{
				ARLevel.Value = null;
			}
			else if (jSONObject.HasField("Blocks"))
			{
				List<JSONObject> list = jSONObject.GetField("Blocks").list;
				ARLevel.Value = list;
			}
		}
	}

	public static bool HasCameraPermission()
	{
		return iOSHasCameraPermission();
	}
}
