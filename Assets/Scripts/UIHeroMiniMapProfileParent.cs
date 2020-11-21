using UnityEngine;

public class UIHeroMiniMapProfileParent : MonoBehaviour
{
	protected void Start()
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		HeroRunner heroRunner = (HeroRunner)Singleton<PropertyManager>.Instance.GetContext("HeroRunner", base.transform);
		return "UI/HeroCardMiniMapProfiles/HeroProfile." + heroRunner.HeroIndex;
	}
}
