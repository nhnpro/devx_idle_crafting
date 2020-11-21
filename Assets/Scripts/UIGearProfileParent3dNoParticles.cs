using UnityEngine;

public class UIGearProfileParent3dNoParticles : MonoBehaviour
{
	protected void Start()
	{
		Debug.LogError($"GetPrefabPath:{GetPrefabPath()}");
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		GearRunner gearRunner = (GearRunner)Singleton<PropertyManager>.Instance.GetContext("GearRunner", base.transform);
		return "UI/GearItemProfiles3DNoParticles/GearProfile." + gearRunner.GearIndex;
	}
}
