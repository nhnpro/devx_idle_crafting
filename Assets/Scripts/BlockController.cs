using Big;
using DG.Tweening;
using System.Collections;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BlockController : Swipeable, IBlock
{
	[SerializeField]
	private GameObject m_prefabHitParticles;

	[SerializeField]
	private GameObject m_prefabBreakParticles;

	[SerializeField]
	private GameObject m_prefabChildTop;

	[SerializeField]
	private GameObject m_prefabChildBottom;

	[SerializeField]
	private float m_childOffset;

	[SerializeField]
	private string DestroyAudioClip;

	[SerializeField]
	private string HitAudioClip;

	private const int CrackMeshCycle = 4;

	private ReactiveProperty<BigDouble> HitPoints = new ReactiveProperty<BigDouble>(1L);

	private int m_crackMeshCycle;

	private Rigidbody m_rigidbody;

	private float m_timeLastSwipe;

	public Transform scaleTransform;

	private bool m_isInited;

	private static int sm_lastParticleFrame = -1;

	private int m_approximateSize;

	public BigDouble AllBlocksHitPoints = new BigDouble(0.0);

	[SerializeField]
	private MeshFilter m_damageMesh;

	private Vector3[] m_verticesOrig;

	private Vector3[] m_verticesNew;

	private const float DamageScale = 0.1f;

	private Vector3 m_dmgAnimUp;

	private Vector3 m_dmgAnimDown;

	public BlockType Type
	{
		get;
		private set;
	}

	public int CompareTo(BlockController another)
	{
		Vector3 position = base.transform.position;
		float z = position.z;
		Vector3 position2 = base.transform.position;
		float num = z + position2.y;
		Vector3 position3 = another.transform.position;
		float z2 = position3.z;
		Vector3 position4 = another.transform.position;
		if (num > z2 + position4.y)
		{
			return 1;
		}
		Vector3 position5 = base.transform.position;
		float z3 = position5.z;
		Vector3 position6 = base.transform.position;
		float num2 = z3 + position6.y;
		Vector3 position7 = another.transform.position;
		float z4 = position7.z;
		Vector3 position8 = another.transform.position;
		if (num2 < z4 + position8.y)
		{
			return -1;
		}
		return 0;
	}

	protected void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		m_damageMesh.mesh.MarkDynamic();
		Vector3 size = GetComponent<BoxCollider>().size;
		m_approximateSize = Mathf.RoundToInt(size.x);
		(from hp in HitPoints.Pairwise()
			where hp.Current < hp.Previous && hp.Previous > BigDouble.ZERO
			select hp).Subscribe(delegate(Pair<BigDouble> hp)
		{
			Singleton<ChunkRunner>.Instance.RemoveChunkHealth(hp.Previous - BigDouble.Max(BigDouble.ZERO, hp.Current));
		}).AddTo(this);
		float num = 1f - 0.2f / (float)m_approximateSize;
		float num2 = 1f + 0.2f / (float)m_approximateSize;
		m_dmgAnimUp = new Vector3(num, num2, num);
		m_dmgAnimDown = new Vector3(num2, num, num2);
	}

	protected void OnEnable()
	{
		Singleton<ChunkRunner>.Instance.RegisterBlock(this);
		m_timeLastSwipe = 0f;
	}

	public void Init(BlockType type)
	{
		Type = type;
		ResetDamageMesh();
		HitPoints.Value = GetFullHP();
		int num = 1;
		if (m_approximateSize == 2 && type != BlockType.TNT)
		{
			num = 9;
		}
		else if (m_approximateSize == 4)
		{
			num = 73;
		}
		AllBlocksHitPoints = num * HitPoints.Value;
		m_isInited = true;
	}

	protected void OnDisable()
	{
		if (Singleton<ChunkRunner>.Instance != null)
		{
			Singleton<ChunkRunner>.Instance.UnregisterBlock(this);
		}
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
		if (!m_isInited || !Singleton<ChunkRunner>.Instance.CanBeHurt.Value)
		{
			return;
		}
		if (damage < BigDouble.ZERO)
		{
			HitPoints.Value = BigDouble.ZERO;
		}
		else
		{
			HitPoints.Value -= damage;
		}
		if (HitPoints.Value <= BigDouble.ZERO)
		{
			if ((float)m_approximateSize > 1.01f || Random.Range(0f, 1f) < 0.05f)
			{
				Singleton<ChunkRunner>.Instance.CauseBlastWave(base.transform.position, m_approximateSize);
			}
			DestroyBlock();
		}
		else
		{
			PlayDamageAnimation();
			UpdateDamageMesh(HitPoints.Value);
		}
	}

	public void CauseDamageAndOverflowExcess(BigDouble damage, int ticksLeft)
	{
		if (damage > HitPoints.Value && ticksLeft > 0)
		{
			SceneLoader.Instance.StartCoroutine(StaggerOverFlowDamage(damage - HitPoints.Value, ticksLeft));
			CauseDamage(HitPoints.Value);
		}
		else
		{
			CauseDamage(damage);
		}
	}

	private IEnumerator StaggerOverFlowDamage(BigDouble damage, int ticksLeft)
	{
		yield return null;
		Singleton<ChunkRunner>.Instance.OverFlowExcessDamage(damage, base.transform.position, ticksLeft);
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
			Singleton<BlockSwipeRunner>.Instance.BlockSwipe(this);
			Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(0).EvaluateDamageAndCritical(out BigDouble damage, out bool critical);
			if (Type != BlockType.Gold && Type != BlockType.Diamond && Type != BlockType.TNT && Singleton<GoldFingerRunner>.Instance.GoldFingerActive.Value)
			{
				ReplaceWithGold();
				DisolveBlock();
			}
			if (PlayerData.Instance.LifetimePrestiges.Value > 0)
			{
				Singleton<ChunkRunner>.Instance.overflowChunk = true;
				CauseDamageAndOverflowExcess(damage, PersistentSingleton<GameSettings>.Instance.OverflowDamageTickAmount);
			}
			else
			{
				CauseDamage(damage);
			}
			if (critical)
			{
				PlayCriticalAnimation();
			}
		}
	}

	private BigDouble GetFullHP()
	{
		return BlockHelper.GetBlockHpMultiplier(Type) * Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value.BlockHP;
	}

	private BigDouble GetFullHP(BlockType type)
	{
		return BlockHelper.GetBlockHpMultiplier(type) * Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value.BlockHP;
	}

	private void PlayCriticalAnimation()
	{
		TextSlerpHelper.SlerpTextCrit("Crit", base.transform.position);
		BindingManager.Instance.ChunkProgressBar.SetTrigger("CriticalHit");
	}

	private void PlayDamageAnimation()
	{
		Singleton<ChunkRunner>.Instance.TryToPlayHitAudioClipForBlock(HitAudioClip);
		if (Singleton<EnableObjectsRunner>.Instance == null || Singleton<EnableObjectsRunner>.Instance.GameView.Value)
		{
			DOTween.Sequence().Append(scaleTransform.DOScale(m_dmgAnimUp, 0.067f)).Append(scaleTransform.DOScale(m_dmgAnimDown, 0.067f))
				.Append(scaleTransform.DOScale(Vector3.one, 0.067f));
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
	}

	private void UpdateDamageMesh(BigDouble hp)
	{
		m_crackMeshCycle++;
		if (m_crackMeshCycle >= 4)
		{
			m_crackMeshCycle = 0;
			RandomizeDamageMesh(1f, Random.Range(0, 100));
			Vector3 position = base.transform.position;
			if (position.y < 10f)
			{
				m_rigidbody.velocity = Vector3.up * Random.Range(2f, 6f);
			}
		}
	}

	public void DisolveBlock()
	{
		m_isInited = false;
		base.gameObject.SetActive(value: false);
	}

	private void ReplaceWithGold()
	{
		string text = base.gameObject.name.Substring(0, base.gameObject.name.Length - 7);
		string[] array = text.Split('_');
		string text2 = string.Empty;
		for (int i = 1; i < array.Length; i++)
		{
			text2 = text2 + "_" + array[i];
		}
		GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject("Blocks/GoldCube" + text2);
		BlockController component = orCreateGameObject.GetComponent<BlockController>();
		if (component != null)
		{
			component.Init(BlockType.Gold);
		}
		orCreateGameObject.transform.position = base.transform.position;
		orCreateGameObject.transform.rotation = Quaternion.identity;
		if (orCreateGameObject.transform.parent == null)
		{
			orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
		}
		Singleton<ChunkRunner>.Instance.RemoveChunkHealth(GetFullHP() - GetFullHP(BlockType.Gold));
	}

	private void DestroyBlock()
	{
		m_isInited = false;
		if (Singleton<EnableObjectsRunner>.Instance != null && Singleton<EnableObjectsRunner>.Instance.GameView.Value && Singleton<QualitySettingsRunner>.Instance.GraphicsDetail.Value > 0 && Time.frameCount != sm_lastParticleFrame)
		{
			sm_lastParticleFrame = Time.frameCount;
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(m_prefabBreakParticles);
			orCreateGameObject.transform.position = base.transform.position;
			orCreateGameObject.transform.rotation = base.transform.rotation;
			orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
		}
		Singleton<ChunkRunner>.Instance.TryToPlayHitAudioClipForBlock(DestroyAudioClip);
		GiveBlockReward();
		if (m_prefabChildTop != null)
		{
			SpawnChildren();
		}
		base.gameObject.SetActive(value: false);
		if (Type == BlockType.TNT)
		{
			SpawnTnt(base.transform);
		}
		Singleton<ChunkRunner>.Instance.NotifyBlockDestroyed();
	}

	private void SpawnTnt(Transform tntLocation)
	{
		GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(BindingManager.Instance.PrefabTntCubeBlock);
		orCreateGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		orCreateGameObject.transform.position = tntLocation.position;
	}

	private void GiveBlockReward()
	{
		BigDouble reward = Singleton<ChunkRunner>.Instance.GoldBlockReward.Value * Singleton<CumulativeBonusRunner>.Instance.BonusMult[1].Value * PlayerData.Instance.BoostersEffect[1].Value;
		if (Type == BlockType.Diamond)
		{
			reward = PersistentSingleton<GameSettings>.Instance.GemBlockReward;
		}
		CollectReward(reward);
		Singleton<FundRunner>.Instance.CountBlockDestroy(Type);
	}

	private void CollectReward(BigDouble reward)
	{
		if (Type == BlockType.Diamond)
		{
			Singleton<FundRunner>.Instance.AddGems(reward.ToInt(), "blockReward", "blocks");
		}
		else if (Type == BlockType.Gold)
		{
			Singleton<FundRunner>.Instance.AddCoins(reward);
		}
		if (Singleton<EnableObjectsRunner>.Instance == null || Singleton<EnableObjectsRunner>.Instance.GameView.Value)
		{
			switch (Type)
			{
			case BlockType.Grass:
				BindingManager.Instance.GrassTarget.SlerpFromWorld(base.transform.position);
				break;
			case BlockType.Dirt:
				BindingManager.Instance.DirtTarget.SlerpFromWorld(base.transform.position);
				break;
			case BlockType.Stone:
				BindingManager.Instance.StoneTarget.SlerpFromWorld(base.transform.position);
				break;
			case BlockType.Metal:
				BindingManager.Instance.MetalTarget.SlerpFromWorld(base.transform.position);
				break;
			case BlockType.Wood:
				BindingManager.Instance.WoodTarget.SlerpFromWorld(base.transform.position);
				break;
			case BlockType.Diamond:
				BindingManager.Instance.UIGemsTarget.SlerpFromWorld(base.transform.position);
				TextSlerpHelper.SlerpTextBigText(reward, base.transform.position);
				break;
			case BlockType.Gold:
				BindingManager.Instance.CoinsTarget.SlerpFromWorld(base.transform.position);
				TextSlerpHelper.SlerpTextBigText(reward, base.transform.position);
				break;
			}
		}
	}

	private void SpawnChildren()
	{
		for (int i = -1; i <= 1; i += 2)
		{
			for (int j = -1; j <= 1; j += 2)
			{
				SpawnChild(new Vector3((float)i * m_childOffset, 1f * m_childOffset, (float)j * m_childOffset), m_prefabChildTop);
			}
		}
		for (int k = -1; k <= 1; k += 2)
		{
			for (int l = -1; l <= 1; l += 2)
			{
				SpawnChild(new Vector3((float)k * m_childOffset, -1f * m_childOffset, (float)l * m_childOffset), m_prefabChildBottom);
			}
		}
	}

	private void SpawnChild(Vector3 pos, GameObject prefab)
	{
		GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(prefab);
		orCreateGameObject.transform.position = base.transform.position + pos;
		orCreateGameObject.transform.rotation = Quaternion.identity;
		orCreateGameObject.transform.SetParent(BindingManager.Instance.BlockContainer, worldPositionStays: true);
		orCreateGameObject.GetComponent<BlockController>().Init(Type);
		Rigidbody component = orCreateGameObject.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.velocity = Vector3.up * Random.Range(2f, 6f);
		}
	}

	private void ResetDamageMesh()
	{
		if (m_verticesOrig != null)
		{
			m_damageMesh.mesh.vertices = m_verticesOrig;
		}
	}

	private void RandomizeDamageMesh(float scale, int seed)
	{
		if (m_verticesOrig == null)
		{
			m_verticesOrig = m_damageMesh.mesh.vertices;
			m_verticesNew = m_damageMesh.mesh.vertices;
		}
		for (int i = 0; i < m_verticesOrig.Length; i++)
		{
			Vector3 vector = m_verticesOrig[i];
			PredictableRandom.SetSeed((uint)(vector.x * 100f * (float)seed), (uint)(vector.y * 100f + vector.z));
			m_verticesNew[i] = m_verticesOrig[i] + new Vector3(PredictableRandom.GetNextRangeFloat((0f - scale) * 0.1f, scale * 0.1f), PredictableRandom.GetNextRangeFloat((0f - scale) * 0.1f, scale * 0.1f), 0f);
		}
		m_damageMesh.mesh.vertices = m_verticesNew;
	}
}
