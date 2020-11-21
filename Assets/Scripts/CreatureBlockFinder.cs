using UnityEngine;

public class CreatureBlockFinder : IBlockFinder
{
	private IBlock m_block;

	private CreatureController m_controller;

	public CreatureBlockFinder(CreatureController controller)
	{
		m_controller = controller;
	}

	public IBlock Get()
	{
		if (m_block == null || !m_block.IsActive())
		{
			m_block = null;
		}
		return m_block;
	}

	public IBlock GetOrFind()
	{
		if (m_block != null && m_block.IsActive())
		{
			return m_block;
		}
		if (Singleton<BossBattleRunner>.Instance.BossBattleActive.Value)
		{
			m_block = Singleton<ChunkRunner>.Instance.BossBlock.Value;
			return m_block;
		}
		int num = Random.Range(0, 12);
		Vector3 position = BindingManager.Instance.CameraCtrl.transform.position;
		float y = (!m_controller.IsGroundUnit()) ? 10f : 0f;
		float z = (!m_controller.IsGroundUnit()) ? (-6f) : (-8f);
		m_block = null;
		for (int i = 0; i < 12; i++)
		{
			Vector3 pos = position + new Vector3(-5.5f + (float)((num + i) % 12), y, z);
			m_block = Singleton<ChunkRunner>.Instance.GetClosestBlock(pos);
			if (m_block == null)
			{
				return null;
			}
			Vector3 pos2 = (!m_controller.IsGroundUnit()) ? (m_block.Position() + CreatureController.VisualOffset) : (m_block.Position().x0z() + CreatureController.VisualOffset);
			if (!Singleton<CreatureCollectionRunner>.Instance.CollidesAnyAliveExcept(pos2, m_controller.Config.HeroIndex))
			{
				return m_block;
			}
		}
		return m_block;
	}

	public void Clear()
	{
		m_block = null;
	}
}
