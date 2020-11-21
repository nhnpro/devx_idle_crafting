using Big;
using System.Collections;
using UniRx;
using UnityEngine;

public class TntRunner : Singleton<TntRunner>
{
	public SkillRunner Skill;

	private Transform TntBlock;

	public ReactiveProperty<BigDouble> TntTriggered = Observable.Never<BigDouble>().ToReactiveProperty();

	public ReadOnlyReactiveProperty<BigDouble> TntCurrentDamage;

	public TntRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		Skill = Singleton<SkillCollectionRunner>.Instance.GetOrCreateSkillRunner(SkillsEnum.TNT);
		TntCurrentDamage = (from dmg in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(0).Damage
			select dmg * PersistentSingleton<GameSettings>.Instance.TntDamageMultiplier).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from pair in Skill.Active.Pairwise()
			where !pair.Previous && pair.Current
			select pair).Subscribe(delegate
		{
			TriggerTnt();
		}).AddTo(instance);
	}

	public void TriggerTnt()
	{
		BindingManager instance = BindingManager.Instance;
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(instance.PrefabTntBlock, instance.CameraCtrl.transform.position + Vector3.up * 10f + new Vector3(Random.Range(-1, 2), 0f, Random.Range(-1, 2)), Quaternion.identity, null);
		TntBlock = gameObject.transform;
		instance.StartCoroutine(ExplodeTnt(instance, gameObject));
		gameObject.transform.SetParent(instance.BiomeContainer);
	}

	public IEnumerator ExplodeTnt(BindingManager root, GameObject tnt)
	{
		float wait = 2f;
		if (tnt.GetComponent<DisableAfter>().GetSeconds() > 0f)
		{
			wait = tnt.GetComponent<DisableAfter>().GetSeconds();
		}
		yield return new WaitForSeconds(wait);
		GameObject go = Singleton<PropertyManager>.Instance.Instantiate(root.PrefabTntExplosion, TntBlock.position, Quaternion.identity, null);
		UnityEngine.Object.Destroy(go, 3f);
		TntTriggered.SetValueAndForceNotify(TntCurrentDamage.Value);
	}
}
