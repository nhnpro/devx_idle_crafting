using UnityEngine;

public class UIGearProfileParentGrayscale : MonoBehaviour
{
	protected void Start()
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		GearRunner gearRunner = (GearRunner)Singleton<PropertyManager>.Instance.GetContext("GearRunner", base.transform);
		return "UI/GearItemProfilesGrayscale/GearProfile." + gearRunner.GearIndex;
	}
}
