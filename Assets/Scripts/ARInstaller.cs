using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ARInstaller : MonoBehaviour
{
	public const string AR_FILE = "/ARLevel.idl";

	public bool RestrictedMode = true;

	public static ARInstaller Instance
	{
		get;
		private set;
	}

	public float Scale
	{
		get
		{
			Vector3 localScale = ARBindingManager.Instance.World.transform.localScale;
			return localScale.x / 10f;
		}
	}

	protected void Awake()
	{
		Instance = this;
		Singleton<PropertyManager>.Construct();
		Singleton<PropertyManager>.Instance.AddRootContext(PlayerData.Instance);
		ARBindingManager.Construct();
		Singleton<ARRunner>.Construct();
		Singleton<AORunnerForAR>.Construct();
		Singleton<ARMaterialRunner>.Construct();
		Singleton<EnableObjectsRunner>.Construct();
	}

	protected void Start()
	{
		Singleton<PropertyManager>.Instance.InstallScene();
		Singleton<ARTutorialRunner>.Construct();
		LoadARLevel();
		LevelEditorControls.Instance.SetFirstRestrStep();
	}

	public static void ReleaseAll()
	{
		SingletonManager.ReleaseAll();
		ARBindingManager.Release();
	}

	public void OnExitAREditor()
	{
		SaveARLevel();
		PersistentSingleton<ARService>.Instance.ShowCustomLevel.SetValueAndForceNotify(value: true);
		Singleton<ARTutorialRunner>.Instance.CompleteARTutorial();
		PersistentSingleton<ARService>.Instance.LevelEditorEvent.SetValueAndForceNotify("Exited");
	}

	public void OnChangeEditor()
	{
		SaveARLevel();
	}

	public JSONObject SaveARLevel()
	{
		JSONObject jSONObject = BlocksToJSON(ARBindingManager.Instance.BlockContainer);
		Save(jSONObject);
		return jSONObject;
	}

	public void Save(JSONObject json)
	{
		try
		{
			File.WriteAllText(PersistentDataPath.Get() + "/ARLevel.idl", Encryptor.Encrypt(json.ToString()));
		}
		catch (IOException)
		{
			SaveLoad.ShowDiskFullDialog();
		}
	}

	public void LoadARLevel()
	{
		if (File.Exists(PersistentDataPath.Get() + "/ARLevel.idl"))
		{
			string cipherText = File.ReadAllText(PersistentDataPath.Get() + "/ARLevel.idl");
			string str = Encryptor.Decrypt(cipherText);
			JSONObject jSONObject = new JSONObject(str);
			if (jSONObject.HasField("Blocks"))
			{
				Transform blockContainer = ARBindingManager.Instance.BlockContainer;
				blockContainer.DestroyChildrenImmediate();
				List<JSONObject> list = jSONObject.GetField("Blocks").list;
				foreach (JSONObject item in list)
				{
					string text = item.asString("Type", () => string.Empty);
					int num = item.asInt("Size", () => 0);
					Vector3 localPosition = new Vector3(item.asFloat("X", () => 0f), item.asFloat("Y", () => 0f), item.asFloat("Z", () => 0f));
					Quaternion rotation = ARBindingManager.Instance.World.transform.rotation;
					GameObject gameObject = GameObjectExtensions.InstantiateFromResources("Blocks/AR/" + text + "Cube_" + num + "x" + num, Vector3.zero, rotation);
					Transform transform = gameObject.transform;
					Vector3 one = Vector3.one;
					Vector3 localScale = ARBindingManager.Instance.World.transform.localScale;
					transform.localScale = one * localScale.x;
					gameObject.transform.SetParent(blockContainer);
					gameObject.transform.localPosition = localPosition;
				}
			}
		}
	}

	public void LoadFromJSON(JSONObject json)
	{
		if (json.HasField("Blocks"))
		{
			Transform blockContainer = ARBindingManager.Instance.BlockContainer;
			blockContainer.DestroyChildrenImmediate();
			List<JSONObject> list = json.GetField("Blocks").list;
			foreach (JSONObject item in list)
			{
				string text = item.asString("Type", () => string.Empty);
				int num = item.asInt("Size", () => 0);
				Vector3 localPosition = new Vector3(item.asFloat("X", () => 0f), item.asFloat("Y", () => 0f), item.asFloat("Z", () => 0f));
				Quaternion rotation = ARBindingManager.Instance.World.transform.rotation;
				GameObject gameObject = GameObjectExtensions.InstantiateFromResources("Blocks/AR/" + text + "Cube_" + num + "x" + num, Vector3.zero, rotation);
				Transform transform = gameObject.transform;
				Vector3 one = Vector3.one;
				Vector3 localScale = ARBindingManager.Instance.World.transform.localScale;
				transform.localScale = one * localScale.x;
				gameObject.transform.SetParent(blockContainer);
				gameObject.transform.localPosition = localPosition;
			}
		}
	}

	public JSONObject BlocksToJSON(Transform blocks)
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("Amount", blocks.childCount);
		JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
		foreach (GameObject item in blocks.gameObject.Children())
		{
			JSONObject jSONObject3 = new JSONObject();
			jSONObject3.AddField("Type", item.name.Substring(0, item.name.Length - 15));
			jSONObject3.AddField("Size", int.Parse(item.name.Substring(item.name.Length - 8, 1)));
			JSONObject jSONObject4 = jSONObject3;
			Vector3 localPosition = item.transform.localPosition;
			jSONObject4.AddField("X", localPosition.x);
			JSONObject jSONObject5 = jSONObject3;
			Vector3 localPosition2 = item.transform.localPosition;
			jSONObject5.AddField("Y", localPosition2.y);
			JSONObject jSONObject6 = jSONObject3;
			Vector3 localPosition3 = item.transform.localPosition;
			jSONObject6.AddField("Z", localPosition3.z);
			jSONObject2.Add(jSONObject3);
		}
		jSONObject.AddField("Blocks", jSONObject2);
		return jSONObject;
	}
}
