public class SkillRewardAction : RewardAction
{
	public SkillsEnum Skill;

	public SkillRewardAction(RewardData reward, RarityEnum rarity, SkillsEnum skill, string friendId)
		: base(reward, rarity, friendId)
	{
		Skill = skill;
	}

	public override void GiveReward()
	{
		SkillStateFactory.GetOrCreateSkillState(Skill).Amount.Value += base.Reward.Amount.ToInt();
	}

	public override void GiveFakeReward()
	{
	}
}
