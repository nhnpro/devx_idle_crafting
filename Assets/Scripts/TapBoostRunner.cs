using Big;
using UniRx;
using UnityEngine;

public class TapBoostRunner : Singleton<TapBoostRunner>
{
	public SkillRunner Skill;

	public ReadOnlyReactiveProperty<BigDouble> DynamiteCurrentDamage;

	public ReactiveProperty<bool> Active = new ReactiveProperty<bool>(initialValue: false);

	public TapBoostRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Skill = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TapBoost);
		Active = (from active in Skill.Active
			select (active)).TakeUntilDestroy(instance).ToReactiveProperty();
		DynamiteCurrentDamage = (from dmg in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(0).Damage
			select dmg * PersistentSingleton<GameSettings>.Instance.TapBoostDynamiteDamageMultiplier).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void InstantiateDynamite(Vector3 pos)
	{
		GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(BindingManager.Instance.PrefabDynamite);
		orCreateGameObject.transform.position = pos + new Vector3(0.5f, 1f, -0.5f);
	}
}
