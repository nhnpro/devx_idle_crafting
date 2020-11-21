using UniRx;

[PropertyClass]
public class TeamBoostRunner : Singleton<TeamBoostRunner>
{
	public SkillRunner Skill;

	[PropertyBool]
	public ReactiveProperty<bool> Active;

	public ReactiveProperty<float> DamageMult;

	public TeamBoostRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		Skill = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TeamBoost);
		DamageMult = (from active in Skill.Active
			select (!active) ? 1f : PersistentSingleton<GameSettings>.Instance.TeamBoostMultiplier).TakeUntilDestroy(instance).ToReactiveProperty();
		Active = (from active in Skill.Active
			select (active)).TakeUntilDestroy(instance).ToReactiveProperty();
	}
}
