using System;
using System.IO;
using UnityEngine;

namespace Dreamteck
{
	public static class ResourceUtility
	{
		public static string FindFolder(string dir, string folderPattern)
		{
			if (folderPattern.StartsWith("/"))
			{
				folderPattern = folderPattern.Substring(1);
			}
			if (!dir.EndsWith("/"))
			{
				dir += "/";
			}
			if (folderPattern == string.Empty)
			{
				return string.Empty;
			}
			string[] array = folderPattern.Split('/');
			if (array.Length == 0)
			{
				return string.Empty;
			}
			string text = string.Empty;
			try
			{
				string[] directories = Directory.GetDirectories(dir);
				foreach (string text2 in directories)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(text2);
					if (directoryInfo.Name == array[0])
					{
						text = text2;
						string text3 = FindFolder(text2, string.Join("/", array, 1, array.Length - 1));
						if (text3 != string.Empty)
						{
							text = text3;
							break;
						}
					}
				}
				if (!(text == string.Empty))
				{
					return text;
				}
				string[] directories2 = Directory.GetDirectories(dir);
				int num = 0;
				while (true)
				{
					if (num >= directories2.Length)
					{
						return text;
					}
					string dir2 = directories2[num];
					text = FindFolder(dir2, string.Join("/", array));
					if (text != string.Empty)
					{
						break;
					}
					num++;
				}
				return text;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
				return string.Empty;
			}
		}

		public static Texture2D LoadTexture(string dreamteckPath, string textureFileName)
		{
			string text = Application.dataPath + "/Dreamteck/" + dreamteckPath;
			if (!Directory.Exists(text))
			{
				text = FindFolder(Application.dataPath, "Dreamteck/" + dreamteckPath);
				if (!Directory.Exists(text))
				{
					return null;
				}
			}
			if (!File.Exists(text + "/" + textureFileName))
			{
				return null;
			}
			byte[] data = File.ReadAllBytes(text + "/" + textureFileName);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.name = textureFileName;
			texture2D.LoadImage(data);
			return texture2D;
		}

		public static Texture2D LoadTexture(string path)
		{
			if (!File.Exists(path))
			{
				return null;
			}
			byte[] data = File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(1, 1);
			FileInfo fileInfo = new FileInfo(path);
			texture2D.name = fileInfo.Name;
			texture2D.LoadImage(data);
			return texture2D;
		}
	}
}
