using UnityEngine;

public class UIPerkProfileParentGrayscale : MonoBehaviour
{
	protected void Start()
	{
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		HeroRunner heroRunner = (HeroRunner)Singleton<PropertyManager>.Instance.GetContext("HeroRunner", base.transform);
		PerkRunner perkRunner = (PerkRunner)Singleton<PropertyManager>.Instance.GetContext("PerkRunner", base.transform);
		BonusMultConfig bonusMultConfig = PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[heroRunner.HeroIndex].Items[perkRunner.PerkIndex];
		return "UI/PerkPanelsGrayscale/PerkPanel." + bonusMultConfig.BonusType;
	}
}
