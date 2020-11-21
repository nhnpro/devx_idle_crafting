public class ConfigCollector
{
	private JSONObject m_jsonCfg;

	public JSONObject Json
	{
		get;
		private set;
	}

	public ConfigCollector()
	{
		Json = JSONObject.obj;
		m_jsonCfg = JSONObject.obj;
		Json.SetField("Config", m_jsonCfg);
	}

	public void AddConfig(string key, string value)
	{
		m_jsonCfg.AddField(key, value);
	}

	public void AddSettings()
	{
	}
}
