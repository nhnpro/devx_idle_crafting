using Big;

public class PlayerGoalConfig
{
	public const int NumStars = 5;

	public string ID;

	public int Index;

	public string AppleID;

	public string GoogleID;

	public PlayerGoalTask Task;

	public int Parameter;

	public int HeroLevelReq;

	public BigDouble[] StarReq = new BigDouble[5];

	public string GoalText;

	public bool IsTutorialGoal;

	public bool GetShowInUI()
	{
		return Task != PlayerGoalTask.Never && Task != PlayerGoalTask.Skip && Task != PlayerGoalTask.None;
	}
}
