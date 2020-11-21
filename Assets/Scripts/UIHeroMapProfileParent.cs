using UnityEngine;

public class UIHeroMapProfileParent : MonoBehaviour
{
	protected void Start()
	{
		Debug.LogError($"GetPrefabPath {GetPrefabPath()}");
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		HeroRunner heroRunner = (HeroRunner)Singleton<PropertyManager>.Instance.GetContext("HeroRunner", base.transform);
		return "UI/HeroCardMapProfiles/HeroProfile." + heroRunner.HeroIndex;
	}
}
