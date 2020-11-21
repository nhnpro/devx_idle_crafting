using System.Collections.Generic;
using UniRx;

public class MapRunner : Singleton<MapRunner>
{
	public const int NodesPerChapter = 4;

	private List<MapNodeRunner> m_mapNodeRunners = new List<MapNodeRunner>();

	public static readonly string FirstMapNode = "MapNode0";

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigeInit
			select order).Subscribe(delegate
		{
			ResetNodes();
		}).AddTo(instance);
	}

	public MapNodeRunner GetMapNodeRunner(int chunk)
	{
		return m_mapNodeRunners[chunk];
	}

	public MapNodeRunner GetOrCreateMapNodeRunner(int node)
	{
		m_mapNodeRunners.EnsureSize(node, (int count) => new MapNodeRunner(count));
		return m_mapNodeRunners[node];
	}

	private void CompleteCurrentNode()
	{
		PlayerData.Instance.BiomeStarted.Value = false;
	}

	private void ResetNodes()
	{
		PlayerData.Instance.BiomeStarted.Value = true;
	}
}
