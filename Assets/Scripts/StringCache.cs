using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class StringCache : PersistentSingleton<StringCache>
{
	public string baseDir = "Config";

	private Dictionary<string, string> cache = new Dictionary<string, string>();

	public string Get(string key)
	{
		if (!cache.ContainsKey(key))
		{
			FetchFromDisk(key);
		}
		return cache[key];
	}

	public void Cache(string key, string data)
	{
		GetOrCreateDirectory();
		cache[key] = data;
		string filePath = GetFilePath(key);
		File.WriteAllText(filePath, Encryptor.Encrypt(data));
	}

	private void FetchFromDisk(string key)
	{
		string filePath = GetFilePath(key);
		string value = (!File.Exists(filePath)) ? GetDefault(key) : Encryptor.Decrypt(File.ReadAllText(filePath));
		cache[key] = value;
	}

	public string GetDefault(string key, ConfigCollector cfgColl = null)
	{
		TextAsset textAsset = Resources.Load(baseDir + "/" + key) as TextAsset;
		if (textAsset == null)
		{
			return null;
		}
		if (!MessageSigner.Verify(textAsset.text))
		{
			throw new SystemException("Tampered content for: " + key);
		}
		return MessageSigner.Content(textAsset.text);
	}

	private string GetFilePath(string key)
	{
		return PersistentDataPath.Get() + "/" + baseDir + "/" + MakeValidFileName(key);
	}

	private static string MakeValidFileName(string name)
	{
		string arg = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
		string pattern = string.Format("([{0}]*\\.+$)|([{0}]+)", arg);
		return Regex.Replace(name, pattern, "_");
	}

	public void Clear()
	{
		PlayerData.Instance.StringCacheCrc32 = 0L;
		GetOrCreateDirectory().Delete(recursive: true);
		cache = new Dictionary<string, string>();
	}

	private DirectoryInfo GetOrCreateDirectory()
	{
		DirectoryInfo parent = Directory.GetParent(GetFilePath("foo"));
		if (!parent.Exists)
		{
			parent.Create();
		}
		return parent;
	}
}
