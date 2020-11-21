using System.Collections.Generic;
using Unity.AutoTune.MiniJSON;

namespace Unity.AutoTune
{
	public class SegmentConfig
	{
		public string segment_id;

		public int group_id;

		public Dictionary<string, object> settings;

		public string config_hash;

		public SegmentConfig(string segment_id, int group_id, Dictionary<string, object> settings, string config_hash)
		{
			this.segment_id = segment_id;
			this.group_id = group_id;
			copySettings(settings);
			this.config_hash = config_hash;
		}

		public string ToJsonDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("segment_id", segment_id);
			dictionary.Add("group_id", group_id);
			dictionary.Add("settings", settings);
			dictionary.Add("config_hash", config_hash);
			return Json.Serialize(dictionary);
		}

		public static SegmentConfig fromJsonDictionary(string json)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			return new SegmentConfig((string)dictionary["segment_id"], (int)(long)dictionary["group_id"], dictionary["settings"] as Dictionary<string, object>, (string)dictionary["config_hash"]);
		}

		private void copySettings(Dictionary<string, object> settings)
		{
			this.settings = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> setting in settings)
			{
				object value = setting.Value;
				if (value is long)
				{
					this.settings[setting.Key] = (int)(long)value;
				}
				else if (value is double)
				{
					this.settings[setting.Key] = (float)(double)value;
				}
				else
				{
					this.settings[setting.Key] = value;
				}
			}
		}
	}
}
