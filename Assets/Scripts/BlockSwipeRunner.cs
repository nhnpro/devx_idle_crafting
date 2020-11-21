using Big;
using UniRx;
using UnityEngine;

[PropertyClass]
public class BlockSwipeRunner : Singleton<BlockSwipeRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> SwipeFoundKey = new ReactiveProperty<bool>();

	[PropertyBool]
	public ReactiveProperty<bool> SwipeFoundBerry = new ReactiveProperty<bool>();

	private int keyTutorialIndex;

	private int berryTutorialIndex;

	private float m_timeLastSwipe;

	public CircularBuffer<IBlock> m_blockBuffer = new CircularBuffer<IBlock>(8);

	private IBlock m_prevBlock;

	public BlockSwipeRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		PlayerData.Instance.MainChunk.Subscribe(delegate
		{
			ClearBuffer();
		}).AddTo(SceneLoader.Instance);
		keyTutorialIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == PersistentSingleton<GameSettings>.Instance.TutorialStepReqForKeys);
		berryTutorialIndex = PersistentSingleton<Economies>.Instance.TutorialGoals.FindIndex((PlayerGoalConfig goal) => goal.ID == PersistentSingleton<GameSettings>.Instance.TutorialStepReqForBerries);
	}

	private void ClearBuffer()
	{
		m_blockBuffer.Clear();
		m_prevBlock = null;
	}

	public void BossSwipe(IBlock block)
	{
		Vector3 pos = block.Position();
		m_timeLastSwipe = Time.realtimeSinceStartup;
		if (Singleton<TapBoostRunner>.Instance.Active.Value)
		{
			RandomDynamiteDrop(pos);
		}
		m_blockBuffer.Add(block);
	}

	public void BlockSwipe(IBlock block)
	{
		Vector3 pos = block.Position();
		m_timeLastSwipe = Time.realtimeSinceStartup;
		Singleton<FundRunner>.Instance.CountBlockTap();
		if (PlayerData.Instance.TutorialStep.Value >= keyTutorialIndex)
		{
			RandomKeyDrop(pos);
		}
		if (PlayerData.Instance.TutorialStep.Value >= berryTutorialIndex)
		{
			RandomBerryDrop(pos);
		}
		if (Singleton<TapBoostRunner>.Instance.Active.Value)
		{
			RandomDynamiteDrop(pos);
		}
		m_blockBuffer.Add(block);
	}

	public float TimeSinceLastSwipe()
	{
		return Time.realtimeSinceStartup - m_timeLastSwipe;
	}

	public IBlock GetClosestBlockInBufferPreferPrev(Vector3 pos, float timeSinceSwipe)
	{
		if (m_prevBlock != null && m_prevBlock.TimeSinceLastSwipe() <= timeSinceSwipe && m_blockBuffer.Contains(m_prevBlock))
		{
			return m_prevBlock;
		}
		m_prevBlock = null;
		float num = float.MaxValue;
		bool flag = false;
		for (int i = 0; i < m_blockBuffer.Size; i++)
		{
			IBlock block = m_blockBuffer.Buffer[i];
			if (block != null && !(block.TimeSinceLastSwipe() > timeSinceSwipe))
			{
				float sqrMagnitude = (block.Position() - pos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					m_prevBlock = block;
					flag |= m_prevBlock.IsActive();
				}
			}
		}
		return m_prevBlock;
	}

	private void RandomKeyDrop(Vector3 pos)
	{
		if (!(Random.Range(0f, 1f) > PersistentSingleton<GameSettings>.Instance.KeyChanceFromSwipe * PlayerData.Instance.BoostersEffect[4].Value))
		{
			SwipeFoundKey.SetValueAndForceNotify(value: true);
			Singleton<FundRunner>.Instance.AddKeys(1, "randomDrop");
			BindingManager.Instance.KeysTargetFromSwipe.SlerpFromWorld(pos);
		}
	}

	private void RandomBerryDrop(Vector3 pos)
	{
		if (!(Random.Range(0f, 1f) > PersistentSingleton<GameSettings>.Instance.BerryChanceFromSwipe * PlayerData.Instance.BoostersEffect[6].Value))
		{
			int numUnlockedHeroes = Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(PlayerData.Instance.LifetimeChunk.Value);
			int num = Random.Range(1, numUnlockedHeroes);
			SwipeFoundBerry.SetValueAndForceNotify(value: true);
			Singleton<FundRunner>.Instance.AddBerry(1, num, "randomDrop");
			GameObject gameObject = BindingManager.Instance.UIBerryTarget.SlerpFromWorldAndReturnParticle(pos);
			gameObject.GetComponentInChildren<BerryColor>().SetBerryColor(num % 5);
		}
	}

	private void RandomDynamiteDrop(Vector3 pos)
	{
		if (!(Random.Range(0f, 1f) > PersistentSingleton<GameSettings>.Instance.TapBoostDynamiteChance))
		{
			Singleton<TapBoostRunner>.Instance.InstantiateDynamite(pos);
		}
	}

	private void CollectCoins(BigDouble reward, Vector3 pos)
	{
		Singleton<FundRunner>.Instance.AddCoins(reward);
		BindingManager.Instance.CoinsTarget.SlerpFromWorld(pos);
	}
}
