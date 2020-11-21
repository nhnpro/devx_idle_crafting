using Big;
using Spine.Unity;
using System;
using UniRx;

public class DrJellyController : Swipeable
{
	public const float MinAnimSpeed = 0.9f;

	public const float MaxAnimSpeed = 1.1f;

	private SkeletonAnimation m_animation;

	private AnimationStateMachine m_stateMachine;

	private bool initedOnce;

	private BossBlockController bbc;

	private void Start()
	{
		Init();
		bbc = GetComponentInParent<BossBlockController>();
		initedOnce = true;
	}

	private void OnEnable()
	{
		if (initedOnce)
		{
			m_stateMachine.NextState = 0;
		}
	}

	protected override void OnSwipe()
	{
		if (bbc != null)
		{
			bbc.SwipeEntered();
		}
	}

	private void Init()
	{
		m_animation = GetComponentInChildren<SkeletonAnimation>();
		m_stateMachine = new AnimationStateMachine(5, m_animation);
		AnimState animState = new AnimState(0);
		animState.AnimationNames = new string[1]
		{
			"DrJelly_Entry"
		};
		animState.Looping = false;
		animState.Speed.Set(0.9f, 1.1f);
		animState.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState.MixDuration = 0f;
		animState.AnimComplete = EnterComplete;
		m_stateMachine.Add(animState);
		AnimState animState2 = new AnimState(1);
		animState2.AnimationNames = new string[1]
		{
			"DrJelly_Idle"
		};
		animState2.Looping = true;
		animState2.Speed.Set(0.9f, 1.1f);
		animState2.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState2.MixDuration = 0.5f;
		m_stateMachine.Add(animState2);
		AnimState animState3 = new AnimState(2);
		animState3.AnimationNames = new string[1]
		{
			"DrJelly_Exit"
		};
		animState3.Looping = false;
		animState3.Speed.Set(0.9f, 1.1f);
		animState3.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState3.MixDuration = 0.5f;
		m_stateMachine.Add(animState3);
		AnimState animState4 = new AnimState(3);
		animState4.AnimationNames = new string[1]
		{
			"_Shooting"
		};
		animState4.Looping = false;
		animState4.Speed.Set(0.9f, 1.1f);
		animState4.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState4.AnimComplete = ShootingComplete;
		animState4.MixDuration = 0.5f;
		m_stateMachine.Add(animState4);
		AnimState animState5 = new AnimState(4);
		animState5.AnimationNames = new string[1]
		{
			"HitEffect"
		};
		animState5.Looping = false;
		animState5.Speed.Set(0.9f, 1.1f);
		animState5.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		m_stateMachine.Add(animState5);
		m_stateMachine.NextState = 0;
		Singleton<BossBattleRunner>.Instance.BossBattleResult.Skip(1).Subscribe(delegate
		{
			m_stateMachine.NextState = 2;
		}).AddTo(this);
		(from hp in Singleton<BossBattleRunner>.Instance.BossCurrentHP.ThrottleFirst(TimeSpan.FromSeconds(0.2)).Pairwise()
			where hp.Current < hp.Previous
			select hp).Subscribe(delegate
		{
			GetHit();
		}).AddTo(this);
	}

	protected void Update()
	{
		if (m_stateMachine != null)
		{
			m_stateMachine.Step();
		}
	}

	private void EnterComplete()
	{
		m_stateMachine.NextState = 1;
	}

	private void ShootingComplete()
	{
		m_stateMachine.NextState = 1;
	}

	public void Shoot()
	{
		m_stateMachine.NextState = 3;
	}

	public void GetHit()
	{
		m_stateMachine.PlayOverrideAnim(4);
	}
}
