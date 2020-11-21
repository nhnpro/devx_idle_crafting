using UnityEngine;

public class UISkillProfileParent3D : MonoBehaviour
{
	protected void Start()
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		SkillRunner skillRunner = (SkillRunner)Singleton<PropertyManager>.Instance.GetContext("SkillRunner", base.transform);
		return "UI/SkillItemProfiles3D/SkillItem3D." + skillRunner.Skill;
	}
}
