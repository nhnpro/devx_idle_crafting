using Big;
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class BossBlockController : Swipeable, IBlock
{
	private const int BlockSize = 4;

	[SerializeField]
	private GameObject m_prefabDamageParticles;

	[SerializeField]
	private GameObject m_prefabDestroyParticles;

	[SerializeField]
	private string DestroyAudioClip;

	private Animator m_animator;

	private bool m_isInited;

	private float m_timeLastSwipe;

	protected void Awake()
	{
		m_animator = GetComponent<Animator>();
	}

	protected void Start()
	{
		Singleton<PropertyManager>.Instance.InstallContexts(base.transform, null);
	}

	public void Init()
	{
		m_isInited = true;
		m_animator.SetTrigger("Init");
		(from killed in Singleton<BossBattleRunner>.Instance.BossBattleResult.Skip(1).TakeUntilDisable(this)
			where killed
			select killed).Subscribe(delegate
		{
			DestroyBlock();
		}).AddTo(this);
		(from killed in Singleton<BossBattleRunner>.Instance.BossBattleResult.Skip(1).TakeUntilDisable(this)
			where !killed
			select killed).Do(delegate
		{
			m_animator.SetTrigger("Failed");
		}).Delay(TimeSpan.FromSeconds(3.0)).Subscribe(delegate
		{
			DestroyFailed();
		})
			.AddTo(this);
	}

	protected void OnEnable()
	{
		Singleton<ChunkRunner>.Instance.RegisterBossBlock(this);
	}

	protected void OnDisable()
	{
		Singleton<ChunkRunner>.Instance.UnregisterBossBlock(this);
	}

	public int ApproximateSize()
	{
		return 4;
	}

	public Vector3 Position()
	{
		return base.transform.position;
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public float TimeSinceLastSwipe()
	{
		return Time.realtimeSinceStartup - m_timeLastSwipe;
	}

	protected override void OnSwipe()
	{
		if (m_isInited)
		{
			m_timeLastSwipe = Time.realtimeSinceStartup;
			Singleton<BlockSwipeRunner>.Instance.BossSwipe(this);
			Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(0).EvaluateDamageAndCritical(out BigDouble damage, out bool critical);
			damage *= PersistentSingleton<GameSettings>.Instance.BossSwipeDamageMult;
			CauseDamage(damage);
			if (critical)
			{
				PlayCriticalAnimation();
			}
		}
	}

	public void CauseDamage(BigDouble damage)
	{
		if (m_isInited && Singleton<ChunkRunner>.Instance.CanBeHurt.Value && !((double)Singleton<BossBattleRunner>.Instance.BossHealthNormalized.Value <= 0.0) && Singleton<BossBattleRunner>.Instance.BossBattleActive.Value)
		{
			m_animator.SetTrigger("Hit");
			Singleton<BossBattleRunner>.Instance.CauseDamage(damage);
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(m_prefabDamageParticles);
			orCreateGameObject.transform.position = base.transform.position;
			orCreateGameObject.transform.rotation = base.transform.rotation;
			orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
		}
	}

	private void DestroyBlock()
	{
		Singleton<ChunkRunner>.Instance.CauseBlastWave(base.transform.position, 10f);
		AudioController.Instance.QueueEvent(new AudioEvent(DestroyAudioClip, AUDIOEVENTACTION.Play));
		m_isInited = false;
		base.gameObject.SetActive(value: false);
		GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(m_prefabDestroyParticles);
		orCreateGameObject.transform.position = base.transform.position;
		orCreateGameObject.transform.rotation = base.transform.rotation;
		orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
	}

	private void DestroyFailed()
	{
		m_isInited = false;
		base.gameObject.SetActive(value: false);
	}

	private void PlayCriticalAnimation()
	{
		TextSlerpHelper.SlerpTextCrit("Crit", base.transform.position);
		BindingManager.Instance.ChunkProgressBar.SetTrigger("CriticalHit");
	}
}
