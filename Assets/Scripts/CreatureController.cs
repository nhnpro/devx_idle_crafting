using Spine;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : AbstractCreatureController
{
	public static readonly Vector3 VisualOffset = new Vector3(-0.8f, 0f, -1.4f);

	private AnimationStateMachine m_stateMachine;

	private CreatureBlockFinder m_blockFinder;

	private Bone m_projectileBone;

	private float Speed = 10f;

	[FormerlySerializedAs("CreatureEnter")]
	[SerializeField]
	private string m_audioEventEnter;

	[FormerlySerializedAs("CreatureAttack")]
	[SerializeField]
	private string m_audioEventAttack;

	[SerializeField]
	private string m_audioEventCheer;

	[SerializeField]
	private string m_audioEventCaptured;

	[SerializeField]
	private GameObject m_trappedCube;

	public override void Init(int heroIndex, CreatureStateEnum defaultState)
	{
		Init(heroIndex);
		m_stateMachine = new AnimationStateMachine(9, m_animation);
		m_blockFinder = new CreatureBlockFinder(this);
		m_projectileBone = m_stateMachine.Skeleton.FindBone("root");
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
		animState2.AnimationNames = new string[1]
		{
			"Idle"
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
		animState3.AnimationNames = new string[1]
		{
			"Idle"
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
		animState5.Speed.Set(1.53f, 1.87000012f);
		animState5.Enter = delegate
		{
			Vector3 vector = GetWantedPosition() - base.transform.position;
			m_stateMachine.Skeleton.FlipX = (vector.x + vector.z * 0.5f < 0f);
		};
		animState5.Step = StepMove;
		m_stateMachine.Add(animState5);
		AnimState animState6 = new AnimState(5);
		animState6.AnimationNames = new string[1]
		{
			"Attack"
		};
		animState6.Looping = true;
		animState6.Speed.Set(1.61999989f, 1.98f);
		animState6.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = false;
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventAttack, AUDIOEVENTACTION.Play));
		};
		animState6.AnimComplete = AttackComplete;
		animState6.AnimEvent = AttackEvent;
		m_stateMachine.Add(animState6);
		AnimState animState7 = new AnimState(6);
		animState7.AnimationNames = new string[1]
		{
			"Captured"
		};
		animState7.Looping = true;
		animState7.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = true;
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventCaptured, AUDIOEVENTACTION.Play));
		};
		animState7.Exit = delegate
		{
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventCaptured, AUDIOEVENTACTION.Stop));
		};
		m_stateMachine.Add(animState7);
		AnimState animState8 = new AnimState(8);
		animState8.AnimationNames = new string[4]
		{
			"Level0",
			"Level1",
			"Level2",
			"Level3"
		};
		animState8.Looping = true;
		animState8.Enter = delegate
		{
			m_stateMachine.Skeleton.FlipX = true;
		};
		m_stateMachine.Add(animState8);
		m_stateMachine.NextState = (int)defaultState;
		PlayerData.Instance.MainChunk.Subscribe(delegate
		{
			m_blockFinder.Clear();
		}).AddTo(this);
		(from pair in Singleton<CameraMoveRunner>.Instance.IsCameraMoving.Pairwise()
			select pair.Previous && !pair.Current into cameraStopped
			where cameraStopped
			select cameraStopped into _
			where m_stateMachine.CurrentState != 6
			select _).Subscribe(delegate
		{
			m_stateMachine.NextState = 0;
		}).AddTo(this);
		Singleton<ChunkRunner>.Instance.BlocksClearTriggered.Subscribe(delegate
		{
			m_stateMachine.NextState = 3;
		}).AddTo(this);
		(from win in Singleton<BossBattleRunner>.Instance.BossBattleResult
			where win
			select win).Subscribe(delegate
		{
			m_stateMachine.NextState = 2;
		}).AddTo(this);
		(from _ in Singleton<ChunkRunner>.Instance.ChapterCompleted
			where m_stateMachine.CurrentState != 6
			select _).Subscribe(delegate
		{
			m_stateMachine.NextState = 0;
		}).AddTo(this);
		(from seq in Singleton<WorldRunner>.Instance.MapSequence.Pairwise()
			where !seq.Current && seq.Previous
			select seq).DelayFrame(1).Subscribe(delegate
		{
			m_stateMachine.NextState = 0;
		}).AddTo(this);
		Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex).Tier.Subscribe(delegate(int tier)
		{
			m_stateMachine.PlayOverrideAnimOrdered(8, Mathf.Max(Mathf.Min(tier - 1, 3), 0));
		}).AddTo(this);
		(from tier in Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(m_heroIndex).Tier
			select Mathf.Max(Mathf.Min(tier, 4), 1)).Subscribe(delegate(int evoLvl)
		{
			switch (evoLvl)
			{
			case 1:
				Speed = 15f;
				break;
			case 2:
				Speed = 30f;
				break;
			case 3:
				Speed = 50f;
				break;
			case 4:
				Speed = 80f;
				break;
			}
		}).AddTo(this);
	}

	protected override IBlockFinder GetBlockFinder()
	{
		return m_blockFinder;
	}

	protected override Vector3 GetWantedPosition()
	{
		if (Singleton<BossBattleRunner>.Instance.BossBattleActive.Value)
		{
			return GetBossPosition();
		}
		IBlock block = m_blockFinder.Get();
		if (block == null)
		{
			return base.transform.position;
		}
		return CalculateWantedPosition(block.Position());
	}

	public bool IsGroundUnit()
	{
		HeroCategory category = base.Config.Category;
		if (category == HeroCategory.AirMelee || category == HeroCategory.AirRanged)
		{
			return false;
		}
		return true;
	}

	public Vector3 CalculateWantedPosition(Vector3 blockPos)
	{
		switch (base.Config.Category)
		{
		case HeroCategory.GroundRanged:
			return GetEnterPosition();
		case HeroCategory.AirMelee:
			return blockPos + VisualOffset;
		case HeroCategory.AirRanged:
			return GetEnterPosition();
		default:
			return blockPos.x0z() + VisualOffset;
		}
	}

	private Vector3 RandomSearchPosition()
	{
		return BindingManager.Instance.CameraCtrl.transform.position + new Vector3(-5.5f + (float)Random.Range(0, 12), 0f, -5f);
	}

	protected void Update()
	{
		if (m_stateMachine != null)
		{
			m_stateMachine.Step();
		}
	}

	private void StepIdle()
	{
		if (m_blockFinder.GetOrFind() != null)
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
		float num = Speed * Time.deltaTime;
		if (vector2.sqrMagnitude <= num * num)
		{
			base.transform.position = GetWantedPosition();
			m_stateMachine.QueueState = 5;
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
			HeroCategory category = base.Config.Category;
			if (category == HeroCategory.GroundRanged || category == HeroCategory.AirRanged)
			{
				SendProjectile();
			}
			else
			{
				Singleton<DamageRunner>.Instance.HeroHit(m_heroIndex, m_blockFinder.Get());
			}
		}
	}

	private void SendProjectile()
	{
		Vector3 position = m_animation.transform.TransformPoint(new Vector3(m_projectileBone.worldX, m_projectileBone.worldY, 0f));
		IBlock block = m_blockFinder.Get();
		if (block == null)
		{
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject("Projectiles/Projectile0");
			orCreateGameObject.transform.position = position;
			orCreateGameObject.GetComponent<ProjectileController>().Init(m_heroIndex, block, block.ApproximateSize());
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

	private void AttackComplete()
	{
		if (m_blockFinder.Get() == null)
		{
			if (m_blockFinder.GetOrFind() == null)
			{
				m_stateMachine.NextState = 1;
			}
			else
			{
				m_stateMachine.NextState = 4;
			}
		}
	}

	public override void Freeze()
	{
		if (m_trappedCube != null)
		{
			m_stateMachine.NextState = 6;
			m_trappedCube.SetActive(value: true);
		}
	}

	public override void UnFreeze()
	{
		if (m_trappedCube != null)
		{
			m_stateMachine.NextState = 1;
			Singleton<DrJellyRunner>.Instance.UnfrozenCompanionIndex.SetValueAndForceNotify(m_heroIndex);
			m_trappedCube.SetActive(value: false);
		}
	}
}
