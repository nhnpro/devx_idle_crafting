public static class SkillsEnumHelper
{
	public static bool IsDuration(SkillsEnum skill)
	{
		if (skill == SkillsEnum.TNT)
		{
			return false;
		}
		return true;
	}
}
