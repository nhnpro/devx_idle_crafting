using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class RestartSpineAnimOnEnable : MonoBehaviour
{
	private SkeletonAnimation m_animation;

	protected void Awake()
	{
		m_animation = GetComponent<SkeletonAnimation>();
	}

	protected void OnEnable()
	{
		if (m_animation.AnimationName != null)
		{
			m_animation.state.SetAnimation(0, m_animation.AnimationName, m_animation.loop);
		}
	}
}
