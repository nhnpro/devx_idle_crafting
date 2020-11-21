using UnityEngine;

public class UISkillProfileParent : MonoBehaviour
{
	protected void Start()
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		SkillRunner skillRunner = (SkillRunner)Singleton<PropertyManager>.Instance.GetContext("SkillRunner", base.transform);
		return "UI/SkillItemProfiles/SkillItem." + skillRunner.Skill;
	}
}
