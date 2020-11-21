using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Unity.AutoTune.MiniJSON;
using Unity.Performance;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AutoTune
{
	public class AutoTune : MonoBehaviour
	{
		public enum Endpoint
		{
			Sandbox,
			Production
		}

		private class DeviceInfo
		{
			public string model;

			public int ram;

			public string cpu;

			public string gfx_name;

			public string gfx_vendor;

			public string device_id;

			public int cpu_count;

			public float dpi;

			public string screen;

			public string project_id;

			public int platform_id;

			public string os_ver;

			public int gfx_shader;

			public string gfx_ver;

			public int max_texture_size;

			public string app_build_version;

			public bool in_editor;

			public DeviceInfo(string projectId, string app_build_version)
			{
				project_id = projectId;
				this.app_build_version = app_build_version;
				model = GetDeviceModel();
				device_id = SystemInfo.deviceUniqueIdentifier;
				ram = SystemInfo.systemMemorySize;
				cpu = SystemInfo.processorType;
				cpu_count = SystemInfo.processorCount;
				gfx_name = SystemInfo.graphicsDeviceName;
				gfx_vendor = SystemInfo.graphicsDeviceVendor;
				screen = Screen.currentResolution.ToString();
				dpi = Screen.dpi;
				in_editor = false;
				if (Application.isEditor)
				{
					platform_id = 11;
					in_editor = true;
				}
				else
				{
					platform_id = (int)Application.platform;
				}
				os_ver = SystemInfo.operatingSystem;
				gfx_shader = SystemInfo.graphicsShaderLevel;
				gfx_ver = SystemInfo.graphicsDeviceVersion;
				max_texture_size = SystemInfo.maxTextureSize;
			}

			private string GetDeviceModel()
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
				string @static = androidJavaClass.GetStatic<string>("MANUFACTURER");
				string static2 = androidJavaClass.GetStatic<string>("MODEL");
				string static3 = androidJavaClass.GetStatic<string>("DEVICE");
				return $"{@static}/{static2}/{static3}";
			}
		}

		public delegate void AutoTuneCallback(Dictionary<string, object> settings, int group);

		private static string SandboxEndpoint = "https://test-auto-tune.uca.cloud.unity3d.com";

		private static string ProductionEndpoint = "https://prd-auto-tune.uca.cloud.unity3d.com";

		private static string CLIENT_DEFAULT_SEGMENT = "-1";

		private static int CLIENT_DEFAULT_GROUP = -1;

		private static string AutoTuneDir = "/unity.autotune";

		private static string SegmentConfigCacheFilePath = AutoTuneDir + "/segmentconfig.json";

		private static AutoTune _instance;

		private static PerfRecorder _prInstance;

		private string _projectId;

		private string _buildVersion;

		private SegmentConfig _clientDefaultConfig;

		private string _storePath;

		private Endpoint _endpoint;

		private SegmentConfig _cachedSegmentConfig;

		private bool _isPlayerOverride;

		private bool _updateNeeded;

		private bool _isError;

		private long _startTime;

		private long _requestTime;

		private float _fetchInGameTime;

		private AutoTuneCallback _callback;

		private DeviceInfo _deviceInfo;

		[RuntimeInitializeOnLoadMethod]
		private static AutoTune GetInstance()
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<AutoTune>();
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("AutoTune");
					_instance = gameObject.AddComponent<AutoTune>();
					_prInstance = gameObject.AddComponent<PerfRecorder>();
					gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}

		private void Awake()
		{
			if (!GetInstance().Equals(this))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public static void Init(string buildVersion, bool usePersistentPath, Dictionary<string, object> defaultValues, Endpoint endpoint = Endpoint.Sandbox)
		{
			if (string.IsNullOrEmpty(Application.cloudProjectId))
			{
				throw new Exception("You must enable Analytics to be able to use AutoTune");
			}
			GetInstance()._projectId = Application.cloudProjectId;
			GetInstance()._clientDefaultConfig = new SegmentConfig(CLIENT_DEFAULT_SEGMENT, CLIENT_DEFAULT_GROUP, defaultValues, "client_default");
			GetInstance()._storePath = ((!usePersistentPath) ? Application.temporaryCachePath : Application.persistentDataPath);
			GetInstance()._buildVersion = buildVersion;
			GetInstance()._endpoint = endpoint;
			GetInstance().LoadCacheSegmentConfig();
			_prInstance.SetBuildVersion(buildVersion);
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object o, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true));
		}

		public static void Fetch(AutoTuneCallback callback)
		{
			GetInstance().CleanUp();
			GetInstance()._callback = callback;
			GetInstance()._startTime = getCurrentTimestamp();
			GetInstance()._fetchInGameTime = Time.time;
			GetInstance().TryFetch();
		}

		public static PerfRecorder GetPerfRecorder()
		{
			return _prInstance;
		}

		public static int GetInt(string key, int defValue = 0)
		{
			return GetValue(key, defValue);
		}

		public static float GetFloat(string key, float defValue = 0f)
		{
			return GetValue(key, defValue);
		}

		public static string GetString(string key, string defValue = "")
		{
			return GetValue(key, defValue);
		}

		public static bool GetBool(string key, bool defValue = false)
		{
			return GetValue(key, defValue);
		}

		public static T GetValue<T>(string key, T defValue)
		{
			SegmentConfig cachedSegmentConfig = GetInstance()._cachedSegmentConfig;
			try
			{
				if (cachedSegmentConfig == null)
				{
					return defValue;
				}
				if (cachedSegmentConfig.settings.ContainsKey(key))
				{
					return (T)cachedSegmentConfig.settings[key];
				}
				return defValue;
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
				return defValue;
			}
		}

		public static void SetPlayerOverride(bool isPlayerOverride)
		{
			GetInstance()._isPlayerOverride = isPlayerOverride;
		}

		public void Update()
		{
			if (_updateNeeded && _callback != null)
			{
				SegmentConfig cachedSegmentConfig = _cachedSegmentConfig;
				DeviceInfo deviceInfo = _deviceInfo;
				try
				{
					long num = getCurrentTimestamp() - _startTime;
					float time = Time.time;
					_callback(cachedSegmentConfig.settings, cachedSegmentConfig.group_id);
					if (deviceInfo == null)
					{
						deviceInfo = new DeviceInfo(_projectId, _buildVersion);
					}
					AnalyticsResult analyticsResult = Analytics.CustomEvent("autotune.SegmentRequestInfo", new Dictionary<string, object>
					{
						{
							"segment_id",
							cachedSegmentConfig.segment_id
						},
						{
							"group_id",
							cachedSegmentConfig.group_id
						},
						{
							"error",
							_isError
						},
						{
							"player_override",
							_isPlayerOverride
						},
						{
							"request_latency",
							_requestTime
						},
						{
							"callback_latency",
							num
						},
						{
							"fetch_time",
							_fetchInGameTime
						},
						{
							"callback_time",
							time
						},
						{
							"model",
							deviceInfo.model
						},
						{
							"ram",
							deviceInfo.ram
						},
						{
							"cpu",
							deviceInfo.cpu
						},
						{
							"cpu_count",
							deviceInfo.cpu_count
						},
						{
							"gfx_name",
							deviceInfo.gfx_name
						},
						{
							"gfx_vendor",
							deviceInfo.gfx_vendor
						},
						{
							"screen",
							deviceInfo.screen
						},
						{
							"dpi",
							deviceInfo.dpi
						},
						{
							"gfx_ver",
							deviceInfo.gfx_ver
						},
						{
							"gfx_shader",
							deviceInfo.gfx_shader
						},
						{
							"max_texture_size",
							deviceInfo.max_texture_size
						},
						{
							"os_ver",
							deviceInfo.os_ver
						},
						{
							"platform_id",
							deviceInfo.platform_id
						},
						{
							"app_build_version",
							_buildVersion
						},
						{
							"plugin_version",
							AutoTuneMeta.version
						},
						{
							"project_id",
							deviceInfo.project_id
						},
						{
							"environment",
							_endpoint
						}
					});
				}
				catch (Exception msg)
				{
					UnityEngine.Debug.LogError(msg);
				}
				finally
				{
					_isError = false;
					_updateNeeded = false;
				}
			}
		}

		private void TryFetch()
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.UploadDataCompleted += wc_UploadDataCompleted;
				webClient.Headers.Add("Content-Type", "application/json");
				DeviceInfo deviceInfo = new DeviceInfo(_projectId, _buildVersion);
				string s = JsonUtility.ToJson(deviceInfo);
				_deviceInfo = deviceInfo;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				string str = (_endpoint != 0) ? ProductionEndpoint : SandboxEndpoint;
				Uri address = new Uri(str + "/v1/settings");
				try
				{
					webClient.UploadDataAsync(address, "POST", bytes);
				}
				catch (WebException msg)
				{
					UnityEngine.Debug.LogError("autotune error on web request");
					UnityEngine.Debug.LogError(msg);
				}
			}
		}

		private SegmentConfig ParseResponse(Dictionary<string, object> response)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<object> list = response["params"] as List<object>;
			string segment_id = (string)response["segment_id"];
			int group_id = (int)(long)response["group"];
			foreach (object item in list)
			{
				Dictionary<string, object> dictionary2 = item as Dictionary<string, object>;
				dictionary[(string)dictionary2["name"]] = dictionary2["value"];
			}
			return new SegmentConfig(segment_id, group_id, dictionary, "0");
		}

		private void wc_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				lock (this)
				{
					_isError = true;
					_updateNeeded = true;
				}
			}
			else if (e.Error != null)
			{
				UnityEngine.Debug.LogError(e.Error);
				lock (this)
				{
					_isError = true;
					_updateNeeded = true;
				}
			}
			else
			{
				try
				{
					string @string = Encoding.UTF8.GetString(e.Result);
					Dictionary<string, object> response = Json.Deserialize(@string) as Dictionary<string, object>;
					SegmentConfig segmentConfig = ParseResponse(response);
					lock (this)
					{
						if (_cachedSegmentConfig.config_hash != segmentConfig.config_hash)
						{
							CacheSegmentConfig(segmentConfig);
						}
						else
						{
							CacheSegmentConfig(segmentConfig);
						}
						_cachedSegmentConfig = segmentConfig;
						_updateNeeded = true;
					}
				}
				catch (Exception arg)
				{
					UnityEngine.Debug.LogError("autotune error parsing response: " + arg);
					lock (this)
					{
						_isError = true;
						_updateNeeded = true;
					}
				}
			}
			_requestTime = getCurrentTimestamp() - _startTime;
		}

		private void CacheSegmentConfig(SegmentConfig config)
		{
			string path = _storePath + AutoTuneDir;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string path2 = _storePath + SegmentConfigCacheFilePath;
			using (StreamWriter streamWriter = new StreamWriter(path2))
			{
				streamWriter.Write(config.ToJsonDictionary());
			}
		}

		private void LoadCacheSegmentConfig()
		{
			string text = _storePath + SegmentConfigCacheFilePath;
			if (!File.Exists(text))
			{
				_cachedSegmentConfig = _clientDefaultConfig;
			}
			else
			{
				try
				{
					using (StreamReader streamReader = new StreamReader(text))
					{
						string json = streamReader.ReadToEnd();
						_cachedSegmentConfig = SegmentConfig.fromJsonDictionary(json);
					}
				}
				catch (Exception ex)
				{
					_cachedSegmentConfig = _clientDefaultConfig;
					UnityEngine.Debug.LogError("autotune error processing cached config file: " + text + " , error: " + ex);
				}
			}
		}

		private void CleanUp()
		{
			_updateNeeded = false;
			_isError = false;
			_deviceInfo = null;
			_callback = null;
			_startTime = 0L;
		}

		private static long getCurrentTimestamp()
		{
			return DateTime.Now.Ticks / 10000;
		}
	}
}
