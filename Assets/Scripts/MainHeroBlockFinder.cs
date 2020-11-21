public class MainHeroBlockFinder : IBlockFinder
{
	private const float SinceSwipe = 0.5f;

	private IBlock m_block;

	private MainHeroController m_controller;

	public MainHeroBlockFinder(MainHeroController controller)
	{
		m_controller = controller;
	}

	public IBlock Get()
	{
		if (Singleton<BlockSwipeRunner>.Instance.TimeSinceLastSwipe() > 0.5f)
		{
			return null;
		}
		return m_block;
	}

	public IBlock GetOrFind()
	{
		if (m_block == null || m_block.TimeSinceLastSwipe() > 0.5f)
		{
			m_block = Singleton<BlockSwipeRunner>.Instance.GetClosestBlockInBufferPreferPrev(m_controller.transform.position, 0.5f);
		}
		return m_block;
	}

	public void Clear()
	{
		m_block = null;
	}
}
