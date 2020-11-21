using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class MainHeroController : AbstractCreatureController
{
	private const float MoveEpsilon = 0.1f;

	private readonly Vector3 VisualOffset = new Vector3(-0.8f, 0f, -1.4f);

	private readonly Vector3 DrJellyVisualOffset = new Vector3(-0.8f, 0f, -7f);

	private AnimationStateMachine m_stateMachine;

	private int m_raycastMask;

	private MainHeroBlockFinder m_blockFinder;

	private HeroDamageRunner m_damageRunner;

	[FormerlySerializedAs("MainCreatureEnter")]
	[SerializeField]
	private string m_audioEventEnter;

	[FormerlySerializedAs("MainCreatureStartAttack")]
	[SerializeField]
	private string m_audioEventStartAttack;

	[FormerlySerializedAs("MainCreatureAttack")]
	[SerializeField]
	private string m_audioEventAttack;

	[SerializeField]
	private string m_audioEventCheer;

	public override void Init(int heroIndex, CreatureStateEnum defaultState)
	{
		Init(heroIndex);
		m_stateMachine = new AnimationStateMachine(9, m_animation);
		m_blockFinder = new MainHeroBlockFinder(this);
		m_damageRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(0);
		m_raycastMask = 1 << LayerMask.NameToLayer("Blocks");
		AnimState animState = new AnimState(0);
		animState.AnimationNames = new string[1]
		{
			"Enter"
		};
		animState.Looping = false;
		animState.Speed.Set(0.9f, 1.1f);
		animState.Enter = delegate
		{
			m_blockFinder.Clear();
			base.transform.position = GetEnterPosition();
			m_stateMachine.Skeleton.FlipX = false;
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventEnter, AUDIOEVENTACTION.Play));
		};
		animState.AnimComplete = EnterComplete;
		m_stateMachine.Add(animState);
		AnimState animState2 = new AnimState(1);
		animState2.AnimationNames = new string[4]
		{
			"Idle1",
			"Idle2",
			"Idle3",
			"Idle4"
		};
		animState2.Looping = true;
		animState2.Speed.Set(0.9f, 1.1f);
		animState2.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState2.Step = StepIdle;
		m_stateMachine.Add(animState2);
		AnimState animState3 = new AnimState(3);
		animState3.AnimationNames = new string[4]
		{
			"Idle1",
			"Idle2",
			"Idle3",
			"Idle4"
		};
		animState3.Looping = true;
		animState3.Speed.Set(0.9f, 1.1f);
		animState3.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		m_stateMachine.Add(animState3);
		AnimState animState4 = new AnimState(2);
		animState4.AnimationNames = new string[1]
		{
			"Cheering"
		};
		animState4.Looping = true;
		animState4.Speed.Set(0.9f, 1.1f);
		animState4.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventCheer, AUDIOEVENTACTION.Play));
		};
		animState4.Exit = delegate
		{
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventCheer, AUDIOEVENTACTION.Stop));
		};
		m_stateMachine.Add(animState4);
		AnimState animState5 = new AnimState(4);
		animState5.AnimationNames = new string[1]
		{
			"Move"
		};
		animState5.Looping = true;
		animState5.Speed.Set(0.9f, 1.1f);
		animState5.Enter = delegate
		{
			Vector3 vector = GetWantedPosition() - base.transform.position;
			m_stateMachine.Skeleton.FlipX = (vector.x + vector.z * 0.5f < 0f);
		};
		animState5.Step = StepMove;
		m_stateMachine.Add(animState5);
		AnimState animState6 = new AnimState(5);
		animState6.AnimationNames = new string[5]
		{
			"Attack1",
			"Attack2",
			"Attack3",
			"Attack4",
			"Attack5"
		};
		animState6.Looping = true;
		animState6.Speed.Set(0.9f, 1.1f);
		animState6.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState6.AnimComplete = AttackComplete;
		animState6.AnimEvent = AttackEvent;
		animState6.Step = AttackStep;
		m_stateMachine.Add(animState6);
		AnimState animState7 = new AnimState(7);
		animState7.AnimationNames = new string[1]
		{
			"Attack6"
		};
		animState7.Looping = true;
		animState7.Speed.Set(0.9f, 1.1f);
		animState7.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
		};
		animState7.AnimComplete = AttackComplete;
		animState7.AnimEvent = AttackEvent;
		m_stateMachine.Add(animState7);
		m_stateMachine.NextState = (int)defaultState;
		PlayerData.Instance.MainChunk.Subscribe(delegate
		{
			m_blockFinder.Clear();
		}).AddTo(this);
		(from pair in Singleton<CameraMoveRunner>.Instance.IsCameraMoving.Pairwise()
			select pair.Previous && !pair.Current into cameraStopped
			where cameraStopped
			select cameraStopped).Subscribe(delegate
		{
			m_stateMachine.NextState = 0;
		}).AddTo(this);
		Singleton<ChunkRunner>.Instance.BlocksClearTriggered.Subscribe(delegate
		{
			m_stateMachine.NextState = 3;
		}).AddTo(this);
		(from win in Singleton<BossBattleRunner>.Instance.BossBattleResult.Skip(1)
			where win
			select win).Subscribe(delegate
		{
			m_stateMachine.NextState = 2;
		}).AddTo(this);
		Singleton<ChunkRunner>.Instance.ChapterCompleted.Subscribe(delegate
		{
			m_stateMachine.NextState = 0;
		}).AddTo(this);
		(from active in Singleton<HammerTimeRunner>.Instance.Active.DelayFrame(1)
			select (!active) ? "Common" : "Gold").Subscribe(delegate(string skin)
		{
			m_animation.skeleton.SetSkin(skin);
			m_animation.skeleton.SetSlotsToSetupPose();
			m_animation.AnimationState.Apply(m_animation.skeleton);
		}).AddTo(this);
	}

	protected override IBlockFinder GetBlockFinder()
	{
		return m_blockFinder;
	}

	protected override Vector3 GetWantedPosition()
	{
		IBlock block = m_blockFinder.Get();
		if (block == null)
		{
			return base.transform.position;
		}
		Vector3 vector = block.Position().x0z() + VisualOffset;
		if (Singleton<DrJellyRunner>.Instance.DrJellyBattle.Value && Singleton<BossBattleRunner>.Instance.BossBattleActive.Value && block.ApproximateSize() == 4)
		{
			vector = block.Position().x0z() + DrJellyVisualOffset;
		}
		Ray ray = new Ray(vector + Vector3.up * 10f, Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, m_raycastMask))
		{
			vector = hitInfo.point;
		}
		return vector;
	}

	protected void Update()
	{
		m_stateMachine.Step();
	}

	private void StepIdle()
	{
		if (m_blockFinder.GetOrFind() == null || !SwipeInput.IsSwiping)
		{
			return;
		}
		if ((GetWantedPosition() - base.transform.position).sqrMagnitude < 0.1f)
		{
			if (m_damageRunner.CriticalTap.Value)
			{
				m_stateMachine.NextState = 7;
			}
			else
			{
				m_stateMachine.NextState = 5;
			}
		}
		else
		{
			m_stateMachine.NextState = 4;
		}
	}

	private void StepMove()
	{
		if (m_blockFinder.Get() == null)
		{
			m_stateMachine.QueueState = 1;
			return;
		}
		Vector3 vector = GetWantedPosition() - base.transform.position;
		if (vector.x < 0f)
		{
			m_stateMachine.Skeleton.FlipX = true;
		}
		Vector3 vector2 = GetWantedPosition() - base.transform.position;
		float num = 30f * Time.deltaTime;
		if (vector2.sqrMagnitude <= num * num)
		{
			base.transform.position = GetWantedPosition();
			if (m_damageRunner.CriticalTap.Value)
			{
				m_stateMachine.NextState = 7;
			}
			else
			{
				m_stateMachine.NextState = 5;
			}
		}
		else
		{
			Vector3 normalized = vector2.normalized;
			Vector3 vector3 = normalized * num;
			base.transform.position += vector3;
		}
	}

	private void AttackEvent(string evt)
	{
		if (evt == "Attack")
		{
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventAttack, AUDIOEVENTACTION.Play));
		}
		else if (evt == "StartAttack")
		{
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventStartAttack, AUDIOEVENTACTION.Play));
		}
	}

	private void EnterComplete()
	{
		Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(m_heroIndex).SceneEnterHappened();
		if (m_blockFinder.GetOrFind() == null)
		{
			m_stateMachine.NextState = 1;
		}
		else
		{
			m_stateMachine.NextState = 4;
		}
	}

	private void AttackStep()
	{
		if (m_damageRunner.CriticalTap.Value)
		{
			m_stateMachine.NextState = 7;
		}
	}

	private void AttackComplete()
	{
		if (m_blockFinder.GetOrFind() == null || !SwipeInput.IsSwiping)
		{
			m_stateMachine.NextState = 1;
		}
		else if ((GetWantedPosition() - base.transform.position).sqrMagnitude < 0.1f)
		{
			if (m_damageRunner.CriticalTap.Value)
			{
				m_stateMachine.NextState = 7;
			}
			else
			{
				m_stateMachine.NextState = 5;
			}
		}
		else
		{
			m_stateMachine.NextState = 4;
		}
	}

	public override void Freeze()
	{
	}

	public override void UnFreeze()
	{
	}
}
