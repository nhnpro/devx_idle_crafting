using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TestTextureGenerator
{
	private static Dictionary<int, Texture2D> m_cache = new Dictionary<int, Texture2D>();

	public static void SetTestTextures()
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren;
			foreach (Renderer renderer in array2)
			{
				Texture mainTexture = renderer.material.mainTexture;
				if (mainTexture != null)
				{
					Texture2D orCreateCheckerTexture = GetOrCreateCheckerTexture(mainTexture.width, mainTexture.height);
					renderer.material.mainTexture = orCreateCheckerTexture;
				}
			}
		}
	}

	public static void SetTestShaders()
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren;
			foreach (Renderer renderer in array2)
			{
				Shader shader = renderer.material.shader;
				if (shader != null)
				{
					renderer.material.shader = Shader.Find("FP/DiffuseWaveLit");
				}
			}
		}
	}

	public static Texture2D GetOrCreateCheckerTexture(int w, int h)
	{
		int key = w + h << 16;
		if (!m_cache.ContainsKey(key))
		{
			m_cache[key] = CreateCheckerTexture(w, h);
		}
		return m_cache[key];
	}

	private static Texture2D CreateCheckerTexture(int w, int h)
	{
		Texture2D texture2D = new Texture2D(w, h, TextureFormat.ARGB32, mipChain: false);
		int num = 4;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				Color color = (((j & num) ^ (i & num)) != 0) ? Color.green : Color.red;
				texture2D.SetPixel(j, i, color);
			}
		}
		texture2D.Apply();
		return texture2D;
	}
}
