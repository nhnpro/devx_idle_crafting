using Big;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[PropertyClass]
public class ChunkRunner : Singleton<ChunkRunner>
{
	private class BiomePos
	{
		public int Z;

		public GameObject Biome;
	}

	public bool vegetation;

	public ReactiveProperty<int> AllBlockAmount = new ReactiveProperty<int>();

	[PropertyInt]
	public ReadOnlyReactiveProperty<int> BlocksAmount;

	public ReactiveProperty<BlastWave> BlastWaveTriggered = Observable.Never<BlastWave>().ToReactiveProperty();

	public ReactiveProperty<bool> BlocksClearTriggered = Observable.Never<bool>().ToReactiveProperty();

	public ReadOnlyReactiveProperty<bool> CanBeHurt;

	public ReactiveProperty<BigDouble> GoldBlockReward;

	public ReactiveProperty<int> BlocksInChunk = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> GoldBlocks = new ReactiveProperty<int>(1);

	public ISubject<bool> ChapterCompleted = new Subject<bool>();

	public ISubject<bool> MoveForward = new Subject<bool>();

	public ISubject<bool> TryBossAgainTriggered = new Subject<bool>();

	public ISubject<bool> BaseCampTrigger = new Subject<bool>();

	public ReactiveProperty<BossBlockController> BossBlock = new ReactiveProperty<BossBlockController>();

	public ReactiveProperty<List<int>> TournamentChunk = new ReactiveProperty<List<int>>(new List<int>());

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> ShowNotification;

	[PropertyBool]
	public ReactiveProperty<bool> CustomLevel = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> ChunkMaxHealth = new ReactiveProperty<BigDouble>(1L);

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> CurrentChunkHealth = new ReactiveProperty<BigDouble>(1L);

	[PropertyString]
	public ReadOnlyReactiveProperty<string> ChunkHealthText;

	[PropertyFloat]
	public ReadOnlyReactiveProperty<float> ChunkProgress;

	private Collider[] m_aoeBuffer = new Collider[8];

	private List<BlockController> m_listBlocks = new List<BlockController>();

	private Queue<BiomePos> m_biomeQueue = new Queue<BiomePos>();

	private int m_raycastMask;

	public const int ChunksPerNode = 10;

	public const int NodesPerBiome = 1;

	public const int ChunksPerBoss = 5;

	public bool overflowChunk;

	private const float BossDistance = 0f;

	private long m_timeSinceLastHitSound;

	private long m_timeSinceLastDestroySound;

	public ChunkRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		m_raycastMask = 1 << LayerMask.NameToLayer("Blocks");
		SceneLoader instance = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		ShowNotification = (from _ in BlocksClearTriggered
			select true).TakeUntilDestroy(instance).ToSequentialReadOnlyReactiveProperty();
		BlocksAmount = (from amount in AllBlockAmount
			select (!(BossBlock.Value == null)) ? Mathf.Max(0, amount - 1) : amount).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from custom in CustomLevel
			where custom
			select custom).Subscribe(delegate
		{
			bind.CustomLevelNode.SetActive(value: true);
		}).AddTo(instance);
		(from custom in CustomLevel.Pairwise()
			where !custom.Current && custom.Previous
			select custom).Subscribe(delegate
		{
			bind.CustomLevelNode.SetActive(value: false);
			bind.CustomLevelFinishedNode.SetActive(value: true);
		}).AddTo(instance);
		(from custom in CustomLevel.Pairwise()
			where !custom.Current && !custom.Previous
			select custom).Subscribe(delegate
		{
			bind.CustomLevelNode.SetActive(value: false);
			bind.CustomLevelFinishedNode.SetActive(value: false);
		}).AddTo(instance);
		ChunkProgress = CurrentChunkHealth.CombineLatest(ChunkMaxHealth, (BigDouble curr, BigDouble max) => (curr / max).ToFloat()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		ChunkHealthText = CurrentChunkHealth.CombineLatest(ChunkMaxHealth, (BigDouble curr, BigDouble max) => BigString.ToString(curr) + " / " + BigString.ToString(max)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
	}

	public void PostInit()
	{
		SceneLoader root = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigePost
			select order).Do(delegate
		{
		}).Subscribe(delegate
		{
			ResetChunks();
			SpawnBaseCamp();
		}).AddTo(root);
		BlocksClearTriggered.Do(delegate
		{
		}).Subscribe(delegate
		{
			if (!IsNextChunkBoss(Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index))
			{
				SpawnChunkIncrease();
			}
			else if (PlayerData.Instance.BossFailedLastTime.Value)
			{
				SpawnChunkRetry();
			}
			else
			{
				SpawnBoss();
			}
		}).AddTo(root);
		(from _ in Singleton<BossSuccessRunner>.Instance.SequenceClosed
			where !ShouldGoToMap(Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index, PlayerData.Instance.LifetimeChunk.Value)
			select _).Do(delegate
		{
		}).Subscribe(delegate
		{
			SpawnChunkIncrease();
		}).AddTo(root);
		(from _ in Singleton<BossSuccessRunner>.Instance.SequenceClosed
			where ShouldGoToMap(Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index, PlayerData.Instance.LifetimeChunk.Value)
			select _).Do(delegate
		{
		}).Subscribe(delegate
		{
			root.StartCoroutine(MapSequenceRoutine());
		}).AddTo(root);
		(from win in Singleton<BossBattleRunner>.Instance.BossBattleResult.Skip(1)
			where !win
			select win).Delay(TimeSpan.FromSeconds(3.0)).Subscribe(delegate
		{
			if (PlayerData.Instance.BossFailedLastTime.Value)
			{
				SpawnChunkRetry();
			}
		}).AddTo(root);
		(from tuple in Singleton<BossBattleRunner>.Instance.BossBattleActive.CombineLatest(Singleton<BossBattleRunner>.Instance.BossCurrentHP, (bool active, BigDouble hp) => new
			{
				active,
				hp
			})
			where tuple.active
			select tuple).Subscribe(tuple =>
		{
			ChunkMaxHealth.Value = Singleton<BossBattleRunner>.Instance.BossFullHP.Value;
			CurrentChunkHealth.Value = tuple.hp;
		}).AddTo(root);
		(from active in Singleton<BossBattleRunner>.Instance.BossBattleActive.Pairwise()
			where !active.Current && active.Previous
			select active).Subscribe(delegate
		{
			CurrentChunkHealth.Value = BigDouble.ZERO;
		}).AddTo(root);
		BlocksClearTriggered.Subscribe(delegate
		{
			CurrentChunkHealth.Value = BigDouble.ZERO;
		}).AddTo(root);
		(from _ in (from _ in TryBossAgainTriggered
				select (from moving in Singleton<CameraMoveRunner>.Instance.IsCameraMoving
					where !moving
					select moving).Take(1)).Switch()
			where !Singleton<BossBattleRunner>.Instance.BossLevelActive.Value
			select _).Subscribe(delegate
		{
			SpawnBossRetry();
		}).AddTo(root);
		if (Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index == 0)
		{
			SpawnBaseCamp();
		}
		else
		{
			SpawnGameLoaded();
		}
		if (PlayerData.Instance.LifetimeChunk.Value == 0 && PersistentSingleton<GameSettings>.Instance.IntroSequenceOn)
		{
			BindingManager.Instance.MapPanel.SetActive(value: true);
			BindingManager.Instance.MapCamera.IntroCameraDrive();
		}
		Singleton<TntRunner>.Instance.TntTriggered.Subscribe(delegate(BigDouble damage)
		{
			CauseDamageToRandomBlocks(damage, (128L * Singleton<CumulativeBonusRunner>.Instance.BonusMult[12].Value).ToInt());
			Vector3 position = bind.CameraCtrl.transform.position;
			CauseBlastWave(position.x0z(), 10f);
		}).AddTo(root);
		HeroDamageRunner mainHeroDamage = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroDamageRunner(0);
		Singleton<AutoMineRunner>.Instance.DamageTriggered.Subscribe(delegate
		{
			CauseDamageToClosestBlockOrBoss(mainHeroDamage.Damage.Value * PersistentSingleton<GameSettings>.Instance.AutoMineMultiplier, bind.TornadoDamageNode.position);
		}).AddTo(root);
		GoldBlockReward = BlocksInChunk.CombineLatest(Singleton<WorldRunner>.Instance.CurrentBiomeConfig, Singleton<GoldFingerRunner>.Instance.GoldFingerActive, GoldBlocks, (int blocks, BiomeConfig cnfg, bool goldFinger, int golds) => (!goldFinger) ? (new BigDouble(blocks) * cnfg.BlockReward / golds) : (cnfg.BlockReward * 13.5)).TakeUntilDestroy(root).ToReactiveProperty();
		(from gf in Singleton<GoldFingerRunner>.Instance.GoldFingerActive.Pairwise()
			where !gf.Current && gf.Previous
			select gf).Subscribe(delegate
		{
			GoldBlockReward.SetValueAndForceNotify(Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value.BlockReward * 13.5);
		}).AddTo(root);
		CanBeHurt = Singleton<BossBattleRunner>.Instance.BossBattlePaused.CombineLatest(Singleton<PrestigeRunner>.Instance.SequenceDone, Singleton<WorldRunner>.Instance.MapSequence, (bool bossPause, bool prestigeDone, bool map) => !bossPause && prestigeDone && !map).TakeUntilDestroy(root).ToReadOnlyReactiveProperty();
		TickerService.MasterTicksFast.Subscribe(delegate(long ticks)
		{
			m_timeSinceLastHitSound += ticks;
			m_timeSinceLastDestroySound += ticks;
		}).AddTo(root);
		(from triggered in BlocksClearTriggered
			where triggered
			select triggered).Subscribe(delegate
		{
			overflowChunk = false;
		}).AddTo(root);
	}

	private IEnumerator MapSequenceRoutine()
	{
		Singleton<EnableObjectsRunner>.Instance.MapCloseButton.Value = false;
		Singleton<WorldRunner>.Instance.StartTransition();
		yield return new WaitForSeconds(1.5f);
		Singleton<WorldRunner>.Instance.GoToMap();
		ResetChunks();
		int completed = Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index / 10;
		Singleton<WorldRunner>.Instance.IncreaseChunk();
		int current = Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index / 10;
		ChapterCompleted.OnNext(value: true);
		yield return new WaitForSeconds(0.5f);
		Singleton<MapRunner>.Instance.GetOrCreateMapNodeRunner(completed).AnimateCompleted();
		Singleton<MapRunner>.Instance.GetOrCreateMapNodeRunner(current).AnimateTravel();
		SpawnGameLoaded();
	}

	private Vector3 GetHeroPosition(int heroIndex)
	{
		AbstractCreatureController orCreateCreature = Singleton<CreatureCollectionRunner>.Instance.GetOrCreateCreature(heroIndex);
		return orCreateCreature.GetTargetPosition();
	}

	public void ResetChunks()
	{
		CurrentChunkHealth.SetValueAndForceNotify(BigDouble.ZERO);
		m_biomeQueue.Clear();
		for (int num = BindingManager.Instance.BlockContainer.childCount - 1; num >= 0; num--)
		{
			BindingManager.Instance.BlockContainer.GetChild(num).gameObject.SetActive(value: false);
		}
		for (int num2 = BindingManager.Instance.BiomeContainer.childCount - 1; num2 >= 0; num2--)
		{
			UnityEngine.Object.DestroyImmediate(BindingManager.Instance.BiomeContainer.GetChild(num2).gameObject);
		}
	}

	public void TryToPlayHitAudioClipForBlock(string HitAudioClip)
	{
		if (new TimeSpan(m_timeSinceLastHitSound).TotalSeconds > 0.05)
		{
			AudioController.Instance.QueueEvent(new AudioEvent(HitAudioClip, AUDIOEVENTACTION.Play));
			m_timeSinceLastHitSound = 0L;
		}
	}

	public void TryToPlayDestroyAudioClipForBlock(string DestroyAudioClip)
	{
		if (new TimeSpan(m_timeSinceLastDestroySound).TotalSeconds > 0.05)
		{
			AudioController.Instance.QueueEvent(new AudioEvent(DestroyAudioClip, AUDIOEVENTACTION.Play));
			m_timeSinceLastDestroySound = 0L;
		}
	}

	public void RegisterBossBlock(BossBlockController block)
	{
		AllBlockAmount.Value++;
		BossBlock.Value = block;
	}

	public void UnregisterBossBlock(BossBlockController block)
	{
		AllBlockAmount.Value--;
		BossBlock.Value = null;
	}

	public void RegisterBlock(BlockController block)
	{
		AllBlockAmount.Value++;
		m_listBlocks.Add(block);
	}

	public void UnregisterBlock(BlockController block)
	{
		AllBlockAmount.Value--;
		m_listBlocks.Remove(block);
	}

	public void NotifyBlockDestroyed()
	{
		if (m_listBlocks.Count == 0 && BossBlock.Value == null)
		{
			BlocksClearTriggered.SetValueAndForceNotify(value: true);
		}
	}

	public void CauseBlastWave(Vector3 pos, float weight)
	{
		BlastWaveTriggered.SetValueAndForceNotify(new BlastWave
		{
			Pos = pos,
			Weight = weight
		});
	}

	public void StartAdventure()
	{
		if (m_listBlocks.Count == 0 && !(BossBlock.Value != null))
		{
			SpawnChunkIncrease();
		}
	}

	public void TryBossAgain()
	{
		if (PlayerData.Instance.BossFailedLastTime.Value && AllBlockAmount.Value > 0 && !(BossBlock.Value != null))
		{
			TryBossAgainTriggered.OnNext(value: true);
		}
	}

	private void CauseDamageToRandomBlocks(BigDouble damage, int spread)
	{
		if (m_listBlocks.Count == 0 && BossBlock.Value != null)
		{
			BossBlock.Value.CauseDamage(damage / 30L);
			return;
		}
		BigDouble damage2 = damage / spread;
		for (int i = 0; i < spread; i++)
		{
			if (m_listBlocks.Count <= 0)
			{
				break;
			}
			int index = UnityEngine.Random.Range(0, m_listBlocks.Count);
			m_listBlocks[index].CauseDamage(damage2);
		}
	}

	private void CauseDamageToClosestBlockOrBoss(BigDouble damage, Vector3 pos)
	{
		BlockController closestBlock = GetClosestBlock(pos);
		if (closestBlock == null)
		{
			BossBlockController value = BossBlock.Value;
			if (!(value == null))
			{
				value.CauseDamage(damage / 2L);
			}
		}
		else
		{
			Singleton<ChunkRunner>.Instance.overflowChunk = true;
			closestBlock.CauseDamageAndOverflowExcess(damage, PersistentSingleton<GameSettings>.Instance.OverflowDamageTickAmount);
		}
	}

	public void OverFlowExcessDamage(BigDouble damage, Vector3 pos, int ticksLeft)
	{
		if (overflowChunk)
		{
			BlockController closestBlock = GetClosestBlock(pos, PersistentSingleton<GameSettings>.Instance.OverflowDamageMaxJumpDistance);
			if (closestBlock != null)
			{
				closestBlock.CauseDamageAndOverflowExcess(damage, ticksLeft - 1);
			}
		}
	}

	public void CauseAoeDamage(BigDouble damage, Vector3 pos, float areaSize)
	{
		int num = Physics.OverlapSphereNonAlloc(pos, areaSize, m_aoeBuffer, m_raycastMask);
		List<BlockController> list = new List<BlockController>();
		for (int i = 0; i < num; i++)
		{
			BlockController component = m_aoeBuffer[i].GetComponent<BlockController>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		if (list.Count == 0)
		{
			BossBlockController value = BossBlock.Value;
			if (!(value == null))
			{
				value.CauseDamage(damage);
			}
		}
		else
		{
			SceneLoader.Instance.StartCoroutine(StaggerAoeDamage(list, damage));
		}
	}

	private IEnumerator StaggerAoeDamage(List<BlockController> blocks, BigDouble damage)
	{
		for (int i = 0; i < blocks.Count; i++)
		{
			if (blocks[i].IsActive())
			{
				blocks[i].CauseDamage(damage);
			}
			yield return null;
		}
	}

	public BlockController GetClosestBlock(Vector3 pos, int steps = 0)
	{
		float num = 1f;
		Collider[] array = Physics.OverlapSphere(pos, num, m_raycastMask);
		BlockController result = null;
		if (m_listBlocks.Count == 1)
		{
			result = m_listBlocks[0];
		}
		else if (steps == 0)
		{
			while (array.Length < 2 && m_listBlocks.Count > 0)
			{
				num += 1f;
				array = Physics.OverlapSphere(pos, num, m_raycastMask);
			}
		}
		else
		{
			for (int i = 0; i < steps; i++)
			{
				num += 1f;
				array = Physics.OverlapSphere(pos, num, m_raycastMask);
				if (array.Length > 0 && m_listBlocks.Count > 0)
				{
					break;
				}
			}
		}
		int num2 = -1;
		float num3 = float.MaxValue;
		for (int j = 0; j < array.Length; j++)
		{
			if (!(BossBlock.Value != null) || !(array[j] == BossBlock.Value.GetComponent<Collider>()))
			{
				float sqrMagnitude = (array[j].transform.position - pos).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					num2 = j;
				}
			}
		}
		if (num2 >= 0)
		{
			result = array[num2].GetComponent<BlockController>();
		}
		return result;
	}

	public void DebugDestroyAllBlocks()
	{
		BlockController[] array = m_listBlocks.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CauseDamage(new BigDouble(-1.0));
		}
	}

	private static bool IsNextChunkBoss(int chunk)
	{
		return chunk % 5 == 4;
	}

	public static bool IsLastChunkForBiome(int chunk)
	{
		int num = 10;
		return chunk % num == num - 1;
	}

	private static bool ShouldGoToMap(int chunk, int lifetimechunk)
	{
		return IsLastChunkForNode(chunk);
	}

	public static bool IsLastChunkForNode(int chunk)
	{
		return chunk % 10 == 9;
	}

	private void SpawnBoss()
	{
		Vector3 position = BindingManager.Instance.CameraCtrl.transform.position;
		Vector3 pos = new Vector3(position.x, 0f, position.z + 30f);
		GenerateBossBlocks(pos);
		UpdateBiomes(pos);
		MoveForward.OnNext(value: true);
	}

	private void SpawnBossRetry()
	{
		BlockController[] array = m_listBlocks.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DisolveBlock();
		}
		SpawnBoss();
	}

	private void SpawnBaseCamp()
	{
		Vector3 position = BindingManager.Instance.CameraCtrl.transform.position;
		UpdateBiomes(position.x0z(), BiomeStart.BaseCamp);
		BaseCampTrigger.OnNext(value: true);
	}

	private void SpawnChunkIncrease()
	{
		Vector3 position = BindingManager.Instance.CameraCtrl.transform.position;
		Vector3 pos = new Vector3(position.x, 0f, position.z + 30f);
		UpdateBiomes(pos, BiomeStart.Normal, 1);
		MoveForward.OnNext(value: true);
		Singleton<WorldRunner>.Instance.IncreaseChunk();
		GenerateBlocks(pos);
		BaseCampTrigger.OnNext(value: false);
	}

	private void SpawnChunkRetry()
	{
		Vector3 position = BindingManager.Instance.CameraCtrl.transform.position;
		Vector3 pos = new Vector3(position.x, 0f, position.z + 30f);
		UpdateBiomes(pos);
		MoveForward.OnNext(value: true);
		PlayerData.Instance.RetryLevelNumber.Value++;
		GenerateBlocks(pos);
	}

	public void SpawnGameLoaded()
	{
		Vector3 position = BindingManager.Instance.CameraCtrl.transform.position;
		Vector3 pos = position.x0z();
		UpdateBiomes(pos, BiomeStart.StartNode);
		GenerateBlocks(pos);
	}

	private void GenerateBlocks(Vector3 pos)
	{
		if (Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index < 20 && PlayerData.Instance.LifetimePrestiges.Value <= 0)
		{
			string chunkPrefabPath = GetChunkPrefabPath(Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index);
			GameObject gameObject = GameObjectExtensions.InstantiateFromResources(chunkPrefabPath, pos, Quaternion.identity);
			ChunkGenerator.GenerateFromPlaceholders(gameObject, BindingManager.Instance.BlockContainer);
			UnityEngine.Object.DestroyImmediate(gameObject);
			CalculateBlocksAndGoldBlocks();
		}
		else if (PersistentSingleton<ARService>.Instance.ARLevel.Value != null && (Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index % 5 == 2 || PersistentSingleton<ARService>.Instance.ShowCustomLevel.Value))
		{
			CustomLevel.Value = true;
			ChunkGenerator.GenerateFromCustomLevel(PersistentSingleton<ARService>.Instance.ARLevel.Value, pos, BindingManager.Instance.BlockContainer);
			PersistentSingleton<ARService>.Instance.ShowCustomLevel.Value = false;
		}
		else
		{
			CustomLevel.Value = false;
			ChunkGenerator.GenerateFromConfig(pos, Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index, PlayerData.Instance.LifetimePrestiges.Value, PlayerData.Instance.RetryLevelNumber.Value, BindingManager.Instance.BlockContainer, bossFight: false);
		}
		(from moving in Singleton<CameraMoveRunner>.Instance.IsCameraMoving
			where !moving
			select moving).Take(1).Subscribe(delegate
		{
			CalculateChunkHealth();
		}).AddTo(SceneLoader.Instance);
	}

	private void GenerateBossBlocks(Vector3 pos)
	{
		CustomLevel.Value = false;
		ChunkGenerator.GenerateFromConfig(pos, Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index, PlayerData.Instance.LifetimePrestiges.Value, PlayerData.Instance.RetryLevelNumber.Value, BindingManager.Instance.BlockContainer, bossFight: true);
		(from moving in Singleton<CameraMoveRunner>.Instance.IsCameraMoving.Skip(1)
			where !moving
			select moving).Take(1).Subscribe(delegate
		{
			CalculateChunkHealth();
		}).AddTo(SceneLoader.Instance);
	}

	private void CalculateBlocksAndGoldBlocks()
	{
		ChunkGeneratingConfig chunkGeneratingConfig = Singleton<EconomyHelpers>.Instance.GetChunkGeneratingConfig(Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index, bossFight: false);
		int maxBlocks = chunkGeneratingConfig.MaxBlocks;
		int num = 0;
		foreach (BlockController listBlock in m_listBlocks)
		{
			if (listBlock.Type == BlockType.Gold)
			{
				int num2 = listBlock.ApproximateSize();
				int num3 = 0;
				switch (num2)
				{
				case 1:
					num3 = 1;
					break;
				case 2:
					num3 = 9;
					break;
				case 4:
					num3 = 73;
					break;
				}
				num += num3;
			}
		}
		BlocksInChunk.SetValueAndForceNotify(maxBlocks);
		GoldBlocks.SetValueAndForceNotify(num);
	}

	private void CalculateChunkHealth()
	{
		BigDouble bigDouble = new BigDouble(0.0);
		for (int i = 0; i < m_listBlocks.Count; i++)
		{
			bigDouble += m_listBlocks[i].AllBlocksHitPoints;
		}
		CurrentChunkHealth.Value = bigDouble;
		ChunkMaxHealth.Value = bigDouble;
	}

	public void RemoveChunkHealth(BigDouble amount)
	{
		Singleton<ChunkRunner>.Instance.CurrentChunkHealth.Value = BigDouble.Max(BigDouble.ZERO, Singleton<ChunkRunner>.Instance.CurrentChunkHealth.Value - amount);
	}

	private void UpdateBiomes(Vector3 pos, BiomeStart biomeStart = BiomeStart.Normal, int chunkFix = 0)
	{
		bool flag = biomeStart == BiomeStart.BaseCamp;
		bool flag2 = biomeStart == BiomeStart.StartNode;
		int num = (!flag) ? (-1) : 0;
		for (int i = num; i < 2; i++)
		{
			int z = Mathf.RoundToInt(pos.z) + i * 30;
			if (m_biomeQueue.ToList().Find((BiomePos b) => z == b.Z) == null)
			{
				int index = Singleton<WorldRunner>.Instance.CurrentChunk.Value.Index;
				BiomeConfig value = Singleton<WorldRunner>.Instance.CurrentBiomeConfig.Value;
				GameObject biome = (flag && i == 0) ? CreateBasecamp() : ((!flag2 || i != 0) ? CreateBiome(z, index + i + chunkFix, value, startNode: false) : CreateBiome(z, index + i + chunkFix, value, startNode: true));
				m_biomeQueue.Enqueue(new BiomePos
				{
					Biome = biome,
					Z = z
				});
			}
		}
		while (m_biomeQueue.Count() > 3)
		{
			BiomePos biomePos = m_biomeQueue.Dequeue();
			UnityEngine.Object.DestroyObject(biomePos.Biome, 3f);
		}
	}

	public IEnumerator GeneratePrestigeChunks(Vector3 pos, int chunk, BiomeConfig biome, BiomeStart biomestart = BiomeStart.Normal)
	{
		for (int i = 0; i < 15; i++)
		{
			int z = Mathf.RoundToInt(pos.z) - (i + 2) * 30;
			CreateBiome(z, chunk - (i + 2), biome, startNode: false);
			yield return null;
		}
	}

	private GameObject CreateBasecamp()
	{
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(BindingManager.Instance.BiomeList.Basecamp, Vector3.zero, Quaternion.identity, null);
		gameObject.transform.SetParent(BindingManager.Instance.BiomeContainer, worldPositionStays: true);
		return gameObject;
	}

	private GameObject CreateBiome(int z, int relativeChunk, BiomeConfig biome, bool startNode)
	{
		Vector3 vector = new Vector3(0f, 0f, z);
		if (vegetation)
		{
			ChunkGenerator.GenerateVegetation(vector, BindingManager.Instance.BlockContainer);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(GetBiomePrefab(relativeChunk, biome, startNode), vector, Quaternion.identity);
		gameObject.transform.SetParent(BindingManager.Instance.BiomeContainer, worldPositionStays: true);
		return gameObject;
	}

	private GameObject GetBiomePrefab(int chunk, BiomeConfig biome, bool startNode)
	{
		PredictableRandom.SetSeed((uint)chunk, 0u);
		BiomeVariations biomeVariations = BindingManager.Instance.BiomeList.Biomes[biome.BiomeIndex];
		if (startNode)
		{
			if (Singleton<TournamentRunner>.Instance.TournamentActive.Value && biomeVariations.TournamentBiomes[0] != null)
			{
				List<int> value = TournamentChunk.Value;
				value.Add(chunk);
				TournamentChunk.SetValueAndForceNotify(value);
				return biomeVariations.TournamentBiomes[PredictableRandom.GetNextRangeInt(0, biomeVariations.TournamentBiomes.Length)];
			}
			return biomeVariations.StartBiomes[PredictableRandom.GetNextRangeInt(0, biomeVariations.StartBiomes.Length)];
		}
		if (chunk % 3 == 0 && Singleton<TournamentRunner>.Instance.TournamentActive.Value && biomeVariations.TournamentBiomes[0] != null)
		{
			List<int> value2 = TournamentChunk.Value;
			value2.Add(chunk);
			TournamentChunk.SetValueAndForceNotify(value2);
			return biomeVariations.TournamentBiomes[PredictableRandom.GetNextRangeInt(0, biomeVariations.TournamentBiomes.Length)];
		}
		return biomeVariations.InsertBiomes[PredictableRandom.GetNextRangeInt(0, biomeVariations.InsertBiomes.Length)];
	}

	private string GetChunkPrefabPath(int chunk)
	{
		return "Chunks/Chunk" + chunk;
	}

	public void DebugEnableBiomeRend(bool enable)
	{
		Renderer[] componentsInChildren = BindingManager.Instance.BiomeContainer.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = enable;
		}
	}
}
