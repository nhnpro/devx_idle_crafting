using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AORunnerForAR : Singleton<AORunnerForAR>
{
	private List<MeshVertexAOForAR> m_meshes = new List<MeshVertexAOForAR>();

	public AORunnerForAR()
	{
		ARBindingManager.Instance.StartCoroutine(AORoutine());
	}

	public void Register(MeshVertexAOForAR mesh)
	{
		m_meshes.Add(mesh);
	}

	public void Unregister(MeshVertexAOForAR mesh)
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
			int numUpdates = Mathf.Min(m_meshes.Count, GetAORefereshRate());
			int num = 0;
			while (num < numUpdates)
			{
				if (index >= m_meshes.Count)
				{
					index = 0;
				}
				MeshVertexAOForAR meshVertexAOForAR = m_meshes[index];
				meshVertexAOForAR.UpdateAO(instant: false);
				num++;
				index++;
			}
		}
	}
}
