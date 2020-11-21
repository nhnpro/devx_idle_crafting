using Big;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class JellyTrapController : Swipeable, IBlock
{
	private int swipesToUnfreeze = 15;

	private int swipesMade;

	[SerializeField]
	private int m_heroIndex;

	[SerializeField]
	private string DestroyAudioClip;

	[SerializeField]
	private string HitAudioClip;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private GameObject m_prefabHitParticles;

	[SerializeField]
	private GameObject m_prefabBreakParticles;

	private int m_approximateSize;

	private static int sm_lastParticleFrame = -1;

	private float m_timeLastSwipe;

	private void Awake()
	{
		Vector3 size = GetComponent<BoxCollider>().size;
		m_approximateSize = Mathf.RoundToInt(size.x);
	}

	private void Start()
	{
		(from active in Singleton<BossBattleRunner>.Instance.BossBattleActive
			where !active
			select active).Subscribe(delegate
		{
			UnFreeze();
		}).AddTo(this);
	}

	private void OnEnable()
	{
		m_timeLastSwipe = 0f;
	}

	protected override void OnSwipe()
	{
		m_timeLastSwipe = Time.realtimeSinceStartup;
		Singleton<BlockSwipeRunner>.Instance.BlockSwipe(this);
		swipesMade++;
		if (swipesMade >= swipesToUnfreeze)
		{
			DestroyBlock();
		}
		else
		{
			PlayDamageAnimation();
		}
	}

	private void PlayDamageAnimation()
	{
		m_animator.SetTrigger("GetHit");
		AudioController.Instance.QueueEvent(new AudioEvent(HitAudioClip, AUDIOEVENTACTION.Play));
		if (Singleton<QualitySettingsRunner>.Instance.GraphicsDetail.Value > 0 && Time.frameCount != sm_lastParticleFrame)
		{
			sm_lastParticleFrame = Time.frameCount;
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(m_prefabHitParticles);
			Vector3 b = Camera.main.transform.forward * m_approximateSize * -0.88f;
			orCreateGameObject.transform.position = base.transform.position + b;
			orCreateGameObject.transform.rotation = base.transform.rotation;
			orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
		}
	}

	private void DestroyBlock()
	{
		if (Singleton<QualitySettingsRunner>.Instance.GraphicsDetail.Value > 0 && Time.frameCount != sm_lastParticleFrame)
		{
			sm_lastParticleFrame = Time.frameCount;
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(m_prefabBreakParticles);
			orCreateGameObject.transform.position = base.transform.position;
			orCreateGameObject.transform.rotation = base.transform.rotation;
			orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
		}
		AudioController.Instance.QueueEvent(new AudioEvent(DestroyAudioClip, AUDIOEVENTACTION.Play));
		UnFreeze();
		swipesMade = 0;
	}

	private void UnFreeze()
	{
		Singleton<CreatureCollectionRunner>.Instance.GetOrCreateCreature(m_heroIndex).UnFreeze();
	}

	public int ApproximateSize()
	{
		return m_approximateSize;
	}

	public Vector3 Position()
	{
		return base.transform.position;
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void CauseDamage(BigDouble damage)
	{
	}

	public float TimeSinceLastSwipe()
	{
		return Time.realtimeSinceStartup - m_timeLastSwipe;
	}
}
