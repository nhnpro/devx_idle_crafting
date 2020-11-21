using Spine;
using Spine.Unity;
using System.Collections;
using UniRx;
using UnityEngine;

public class CreatureUIController : MonoBehaviour
{
	[SerializeField]
	private SkeletonGraphic m_graphic;

	[SerializeField]
	private int m_heroIndex;

	private void Start()
	{
		Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex).Tier.Subscribe(delegate(int tier)
		{
			SetAnimations(tier);
		}).AddTo(this);
	}

	private void SetAnimations(int tier)
	{
		m_graphic.AnimationState.SetAnimation(0, "Idle", loop: true);
		string animationName = "Level" + Mathf.Max(Mathf.Min(tier - 1, 3), 0).ToString() + "Ui";
		m_graphic.AnimationState.SetAnimation(1, animationName, loop: true);
	}

	public IEnumerator EvolutionAnimations()
	{
		int tier = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex).Tier.Value;
		m_graphic.AnimationState.SetAnimation(0, "Idle", loop: true);
		string animName = "Level" + Mathf.Min(tier - 2, 2).ToString();
		m_graphic.AnimationState.SetAnimation(1, animName, loop: true);
		yield return new WaitForSeconds(1.5f);
		string transName = "Transition_Level" + (tier - 2).ToString() + "to" + (tier - 1).ToString();
		TrackEntry trackEntry = m_graphic.AnimationState.SetAnimation(0, transName, loop: false);
		m_graphic.AnimationState.SetEmptyAnimation(1, 0f);
		yield return new WaitForSpineAnimationComplete(trackEntry);
		m_graphic.AnimationState.SetAnimation(0, "Idle", loop: true);
		string evolvedAnim = "Level" + Mathf.Min(tier - 1, 3).ToString();
		m_graphic.AnimationState.SetAnimation(1, evolvedAnim, loop: true);
	}
}
