using UnityEngine;

public class UIGoalProfileParent : MonoBehaviour
{
	protected void Start()
	{
		Debug.LogError(GetPrefabPath());
		GameObject gameObject = GameObjectExtensions.InstantiateFromResources(GetPrefabPath());
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}

	private string GetPrefabPath()
	{
		PlayerGoalRunner playerGoalRunner = (PlayerGoalRunner)Singleton<PropertyManager>.Instance.GetContext("PlayerGoalRunner", base.transform);
		switch (playerGoalRunner.GoalConfig.Task)
		{
		case PlayerGoalTask.AmountSkillUsed:
			return "UI/GoalCardProfiles/GoalProfile." + playerGoalRunner.GoalConfig.Task + "." + (SkillsEnum)playerGoalRunner.GoalConfig.Parameter;
		case PlayerGoalTask.HeroLevel:
			return "UI/GoalCardProfiles/GoalProfile." + playerGoalRunner.GoalConfig.Task + "." + playerGoalRunner.GoalConfig.Parameter;
		default:
			return "UI/GoalCardProfiles/GoalProfile." + playerGoalRunner.GoalConfig.Task;
		}
	}
}
