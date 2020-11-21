using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AORunner : Singleton<AORunner>
{
	private List<MeshVertexAO> m_meshes = new List<MeshVertexAO>();

	public AORunner()
	{
		SceneLoader.Instance.StartCoroutine(AORoutine());
	}

	public void Register(MeshVertexAO mesh)
	{
		m_meshes.Add(mesh);
	}

	public void Unregister(MeshVertexAO mesh)
	{
		m_meshes.Remove(mesh);
	}

	private int GetAORefereshRate()
	{
		float t = Mathf.Clamp01((FrameRateCounter.AvgFps - 30f) / 30f);
		return (int)Mathf.Lerp(1f, 30f, t);
	}

	private IEnumerator AORoutine()
	{
		int index = 0;
		while (true)
		{
			yield return null;
			if (!Singleton<QualitySettingsRunner>.Instance.HighDetails.Value)
			{
				continue;
			}
			int numUpdates = Mathf.Min(m_meshes.Count, GetAORefereshRate());
			int num = 0;
			while (num < numUpdates)
			{
				if (index >= m_meshes.Count)
				{
					index = 0;
				}
				MeshVertexAO meshVertexAO = m_meshes[index];
				meshVertexAO.UpdateAO(instant: false);
				num++;
				index++;
			}
		}
	}
}
