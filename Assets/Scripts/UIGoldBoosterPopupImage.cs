using UnityEngine;

public class UIGoldBoosterPopupImage : MonoBehaviour
{
	protected void Start()
	{
		Debug.LogError($"GetPrefabPath {GetPrefabPath()}");
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		GoldBoosterRunner goldBoosterRunner = (GoldBoosterRunner)Singleton<PropertyManager>.Instance.GetContext("GoldBoosterRunner", base.transform);
		return "UI/GemBoosterProfiles/GemBooster." + (GoldBoosterEnum)goldBoosterRunner.GoldBoosterIndex;
	}
}
