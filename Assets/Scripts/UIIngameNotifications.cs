using System.Collections.Generic;
using UnityEngine;

public class UIIngameNotifications : MonoBehaviour
{
	public GameObject CurrentNotification;

	public int CurrentHeroIndex = -1;

	public void InstantiatePlayerGoalNotification(PlayerGoalRunner playerGoal)
	{
		string path = "UI/Notifications/GoalCompletedNotification";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("PlayerGoalRunner", playerGoal);
		Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent(path, Vector3.zero, Quaternion.identity, dictionary, base.transform);
	}

	public void InstantiateMiniMilestoneNotification(HeroRunner heroMiniMilestone)
	{
		if (CurrentHeroIndex != heroMiniMilestone.HeroIndex || !(CurrentNotification != null) || !CurrentNotification.activeSelf)
		{
			string path = "UI/Notifications/MiniMilestoneNotification";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", heroMiniMilestone.HeroIndex);
			CurrentHeroIndex = heroMiniMilestone.HeroIndex;
			CurrentNotification = Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent(path, Vector3.zero, Quaternion.identity, dictionary, base.transform);
		}
	}

	public void InstantiateGenericNotification(string notification)
	{
		string path = "UI/Notifications/GenericNotification";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("StringValue", notification);
		Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent(path, Vector3.zero, Quaternion.identity, dictionary, base.transform);
	}

	public void InstantiatePerkNotification(PerkUnlockedInfo perk)
	{
		if (PersistentSingleton<Economies>.Instance.PerkMilestoneConfigs[perk.HeroIndex].Items[perk.PerkIndex].BonusType >= BonusTypeEnum.AllDamage)
		{
			string path = "UI/Notifications/PerkNotification";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", perk.HeroIndex);
			dictionary.Add("PerkIndex", perk.PerkIndex);
			Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent(path, Vector3.zero, Quaternion.identity, dictionary, base.transform);
		}
	}
}
