public static class SkillStateFactory
{
	public static SkillState GetOrCreateSkillState(SkillsEnum skill)
	{
		PlayerData.Instance.SkillStates.EnsureSize((int)skill, (int _) => new SkillState());
		return PlayerData.Instance.SkillStates[(int)skill];
	}
}
