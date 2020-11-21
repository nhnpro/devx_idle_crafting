using UniRx;

public class GoldFingerRunner : Singleton<GoldFingerRunner>
{
	public SkillRunner Skill;

	public ReactiveProperty<bool> GoldFingerActive;

	public GoldFingerRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Skill = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.Goldfinger);
		GoldFingerActive = (from active in Skill.Active
			select (active)).TakeUntilDestroy(instance).ToReactiveProperty();
	}
}
