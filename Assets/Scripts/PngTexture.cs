using System;
using System.IO;
using UnityEngine;

public static class PngTexture
{
	public static Texture2D LoadPNGAsTexture2D(string fileName, int width, int height)
	{
		Texture2D texture2D = null;
		string path = PersistentDataPath.Get() + "/" + fileName;
		if (File.Exists(path))
		{
			byte[] data;
			try
			{
				data = File.ReadAllBytes(path);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning("Caught serializationException: " + ex.Message + " while reading PNG file.");
				return texture2D;
			}
			texture2D = new Texture2D(width, height);
			texture2D.LoadImage(data);
		}
		return texture2D;
	}

	public static bool SaveTexture2DAsPNG(Texture2D tex, string fileName)
	{
		byte[] bytes = tex.EncodeToPNG();
		string path = PersistentDataPath.Get() + "/" + fileName;
		try
		{
			File.WriteAllBytes(path, bytes);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Caught serializationException: " + ex.Message + " while writing PNG data.");
			return false;
		}
		return true;
	}
}
