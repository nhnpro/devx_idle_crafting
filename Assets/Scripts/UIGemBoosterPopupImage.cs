using UnityEngine;

public class UIGemBoosterPopupImage : MonoBehaviour
{
	protected void Start()
	{
		Debug.LogError($"GetPrefabPath {GetPrefabPath()}");
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		BoosterRunner boosterRunner = (BoosterRunner)Singleton<PropertyManager>.Instance.GetContext("BoosterRunner", base.transform);
		return "UI/GemBoosterProfiles/GemBooster." + (BoosterEnum)boosterRunner.BoosterIndex;
	}
}
