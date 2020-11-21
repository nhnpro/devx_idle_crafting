using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Unity.Performance
{
	public class Benchmarking : MonoBehaviour
	{
		private static readonly string k_PluginVersion = "v1.1.1";

		private static Benchmarking _instance;

		private static bool _applicationIsQuitting;

		private static Result _activeExperiment;

		private static float _deltaTimeMSSum;

		private static float _deltaTimeMSSqrSum;

		private static int _lastFrameRecorded;

		private static Dictionary<string, Result> _results = new Dictionary<string, Result>();

		private static readonly Dictionary<string, Stopwatch> m_GenericTimers = new Dictionary<string, Stopwatch>();

		private static string _experimentName;

		private static string m_ActiveTimerContext;

		public static IDataSource DataSource = new DefaultDataSource();

		private static IProfilerControl ProfilerControl = new DefaultProfilerControl();

		private static string profilerCapturePath
		{
			get;
			set;
		}

		public static string buildVersion
		{
			get;
			set;
		}

		public static string pluginVersion => k_PluginVersion;

		private static Dictionary<string, Result> Results => _results;

		[RuntimeInitializeOnLoadMethod]
		private static Benchmarking GetInstance()
		{
			if (_applicationIsQuitting)
			{
				return null;
			}
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<Benchmarking>();
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("Benchmarking");
					_instance = gameObject.AddComponent<Benchmarking>();
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

		private void OnApplicationQuit()
		{
			_applicationIsQuitting = true;
		}

		[Obsolete("Use pluginVersion propery instead.")]
		public static string getBuildVersion()
		{
			return buildVersion;
		}

		[Obsolete("Use SetBuildVersion(string) or buildVersion property instead.")]
		public static void setBuildVersion(string newBuildVersion)
		{
			buildVersion = newBuildVersion;
		}

		public static void SetBuildVersion(string buildVersion)
		{
			Benchmarking.buildVersion = buildVersion;
		}

		private static IEnumerable<string> getExperimentNames()
		{
			return _results.Keys;
		}

		public static Result BeginExperiment(string experimentName)
		{
			if (GetInstance() == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(experimentName))
			{
				UnityEngine.Debug.LogError("Unable to start experiment. Name cannot be null or empty.");
				return null;
			}
			if (_results.Count > 0)
			{
				EndExperiment();
				Finished();
			}
			_experimentName = experimentName;
			if (_results.ContainsKey(experimentName))
			{
				EndExperiment(experimentName);
				Finished(experimentName);
			}
			_results[experimentName] = new Result(experimentName);
			_deltaTimeMSSum = 0f;
			_deltaTimeMSSqrSum = 0f;
			_lastFrameRecorded = DataSource.frameCount;
			if (!string.IsNullOrEmpty(profilerCapturePath) && ProfilerControl != null && ProfilerControl.supported)
			{
				ProfilerControl.StartRecording(profilerCapturePath + experimentName);
			}
			return _results[experimentName];
		}

		public static Result EndExperiment()
		{
			if (_results.Count == 0)
			{
				return null;
			}
			return EndExperiment(_experimentName);
		}

		private static Result EndExperiment(string experimentName)
		{
			if (string.IsNullOrEmpty(experimentName))
			{
				UnityEngine.Debug.LogError("Unable to end experiment. Name cannot be null or empty.");
				return null;
			}
			if (!_results.ContainsKey(experimentName))
			{
				UnityEngine.Debug.LogErrorFormat("Unable to end experiment. Experiment with name '{0}' not found.", experimentName);
			}
			Result result = _results[experimentName];
			result.lastFrameNumber = DataSource.frameCount;
			result.realtimeAtStop = DataSource.realtimeSinceStartup;
			result.memoryUsageAtEnd = DataSource.memoryAllocated;
			if (ProfilerControl != null && ProfilerControl.recording)
			{
				ProfilerControl.StopRecording();
			}
			_experimentName = null;
			return result;
		}

		public static void Finished()
		{
			foreach (Result value in _results.Values)
			{
				value.sendFrameData();
			}
			_results.Clear();
		}

		private static void Finished(string experimentName)
		{
			_results[experimentName].sendFrameData();
			_results.Remove(experimentName);
		}

		public static void LoadScene(int sceneBuildIndex)
		{
			LoadScene(SceneManager.GetSceneAt(sceneBuildIndex).name);
		}

		public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode)
		{
			LoadScene(SceneManager.GetSceneAt(sceneBuildIndex).name, mode);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			return LoadSceneAsync(SceneManager.GetSceneAt(sceneBuildIndex).name);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
		{
			return LoadSceneAsync(SceneManager.GetSceneAt(sceneBuildIndex).name, mode);
		}

		public static void LoadScene(string sceneName)
		{
			GetInstance().DoLoadScene(sceneName, LoadSceneMode.Single);
		}

		public static void LoadScene(string sceneName, LoadSceneMode mode)
		{
			GetInstance().DoLoadScene(sceneName, mode);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName)
		{
			return GetInstance().DoLoadSceneAsync(sceneName, LoadSceneMode.Single);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode)
		{
			return GetInstance().DoLoadSceneAsync(sceneName, mode);
		}

		private void DoLoadScene(string sceneName, LoadSceneMode mode)
		{
			Stopwatch stopwatch = new Stopwatch();
			uint memoryAllocated = DataSource.memoryAllocated;
			stopwatch.Start();
			SceneManager.LoadScene(sceneName, mode);
			OnSceneLoaded(sceneName, stopwatch, memoryAllocated);
		}

		private AsyncOperation DoLoadSceneAsync(string sceneName, LoadSceneMode mode)
		{
			AsyncOperation asyncOperation = null;
			Stopwatch stopwatch = new Stopwatch();
			uint memoryAllocated = DataSource.memoryAllocated;
			stopwatch.Start();
			asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
			StartCoroutine(WaitWhileLoadingScene(asyncOperation, sceneName, stopwatch, memoryAllocated));
			return asyncOperation;
		}

		private IEnumerator WaitWhileLoadingScene(AsyncOperation asyncOp, string sceneName, Stopwatch stopWatch, uint memAllocAtStart)
		{
			yield return asyncOp;
			OnSceneLoaded(sceneName, stopWatch, memAllocAtStart);
		}

		private void OnSceneLoaded(string sceneName, Stopwatch stopWatch, uint memAllocAtStart)
		{
			stopWatch.Stop();
			double totalMilliseconds = stopWatch.Elapsed.TotalMilliseconds;
			long num = (long)DataSource.memoryAllocated - (long)memAllocAtStart;
			if (Debug.isDebugBuild)
			{
				if (totalMilliseconds > 3.4028234663852886E+38)
				{
					UnityEngine.Debug.LogWarning("Load time exceeded the measurable range.");
				}
				if (num > int.MaxValue)
				{
					UnityEngine.Debug.LogWarning("Memory delta exceeded the measurable range.");
				}
			}
			AnalyticsResult analyticsResult = SceneLoadEvent(sceneName, (float)totalMilliseconds, (int)num);
			if (!Debug.isDebugBuild)
			{
			}
		}

		public static void StartTimer(string context)
		{
			if (_applicationIsQuitting)
			{
				return;
			}
			if (string.IsNullOrEmpty(context))
			{
				UnityEngine.Debug.LogError("Unable to start timer. Context cannot be null or empty.");
				return;
			}
			if (m_GenericTimers.Count > 0)
			{
				StopTimer();
			}
			m_ActiveTimerContext = context;
			if (m_GenericTimers.ContainsKey(context))
			{
				string log = null;
				StopTimer(context, m_GenericTimers[context], out log);
				m_GenericTimers[context].Reset();
				if (!Debug.isDebugBuild)
				{
				}
			}
			else
			{
				Stopwatch value = new Stopwatch();
				m_GenericTimers.Add(context, value);
			}
			m_GenericTimers[context].Start();
			if (!Debug.isDebugBuild)
			{
			}
		}

		public static float StopTimer()
		{
			if (m_GenericTimers.Count == 0)
			{
				return -1f;
			}
			return StopTimer(m_ActiveTimerContext);
		}

		private static float StopTimer(string context)
		{
			float result = -1f;
			if (_applicationIsQuitting)
			{
				return result;
			}
			if (string.IsNullOrEmpty(context))
			{
				UnityEngine.Debug.LogError("Unable to start timer. Context cannot be null or empty.");
				return result;
			}
			if (!m_GenericTimers.ContainsKey(context))
			{
				UnityEngine.Debug.LogErrorFormat("Unable to stop timer. Timer with context '{0}' not found.", context);
				return result;
			}
			string log = null;
			result = StopTimer(context, m_GenericTimers[context], out log);
			m_GenericTimers.Remove(context);
			if (Debug.isDebugBuild)
			{
			}
			return result;
		}

		private static void StopAllTimers()
		{
			if (!_applicationIsQuitting && m_GenericTimers.Count != 0)
			{
				string str = string.Empty;
				if (Debug.isDebugBuild)
				{
					str = $"Stopping {m_GenericTimers.Count} active timers...";
				}
				foreach (KeyValuePair<string, Stopwatch> genericTimer in m_GenericTimers)
				{
					string log = null;
					StopTimer(genericTimer.Key, genericTimer.Value, out log);
					if (Debug.isDebugBuild)
					{
						str += "\n  " + log;
					}
				}
				m_GenericTimers.Clear();
				if (!Debug.isDebugBuild)
				{
				}
			}
		}

		private static float StopTimer(string context, Stopwatch timer, out string log)
		{
			log = null;
			timer.Stop();
			double totalMilliseconds = timer.Elapsed.TotalMilliseconds;
			if (Debug.isDebugBuild && totalMilliseconds > 3.4028234663852886E+38)
			{
				UnityEngine.Debug.LogWarning("The timer exceeded the measurable range.");
			}
			AnalyticsResult analyticsResult = GenericTimerEvent(context, (float)totalMilliseconds);
			if (Debug.isDebugBuild)
			{
				log = $"Timer with context '{context}' stopped ({analyticsResult}). Time elapsed in milliseconds: {totalMilliseconds}";
			}
			return (float)totalMilliseconds;
		}

		private static void Clear()
		{
			_results.Clear();
		}

		private static void LogWarning(string format, params object[] args)
		{
			UnityEngine.Debug.LogWarningFormat(format, args);
		}

		private void OnApplicationPause(bool paused)
		{
			foreach (Result value in _results.Values)
			{
				if (paused)
				{
					value.pauseTimer.Start();
				}
				else
				{
					value.pauseTimer.Stop();
				}
			}
			foreach (Stopwatch value2 in m_GenericTimers.Values)
			{
				if (paused)
				{
					value2.Stop();
				}
				else
				{
					value2.Start();
				}
			}
		}

		private void Update()
		{
			if (DataSource.frameCount != _lastFrameRecorded)
			{
				if (DataSource.frameCount != _lastFrameRecorded + 1)
				{
					LogWarning("{0} missing frames detected. The experiment results will be inaccurate. Please check that you are calling Benchmarking.Update every frame!", DataSource.frameCount - _lastFrameRecorded - 1);
				}
				_lastFrameRecorded = DataSource.frameCount;
				float num = DataSource.unscaledDeltaTimeSeconds * 1000f;
				foreach (Result value in _results.Values)
				{
					if (num < value.deltaTimeMSMin)
					{
						value.deltaTimeMSMin = num;
					}
					if (num > value.deltaTimeMSMax)
					{
						value.deltaTimeMSMax = num;
					}
					value.totalSamples++;
					_deltaTimeMSSum += num;
					_deltaTimeMSSqrSum += num * num;
					value.deltaTimeMSBins.AddValue(num);
					uint memoryAllocated = DataSource.memoryAllocated;
					value.memoryTotal += memoryAllocated;
					if (memoryAllocated > value.memoryUsageAtPeak)
					{
						value.memoryUsageAtPeak = memoryAllocated;
					}
				}
			}
		}

		private static AnalyticsResult CustomEvent(string name, IDictionary<string, object> data)
		{
			data.Add("build_version", buildVersion);
			data.Add("plugin_version", pluginVersion);
			AnalyticsResult analyticsResult = Analytics.CustomEvent(name, data);
			if (analyticsResult != 0)
			{
				UnityEngine.Debug.LogErrorFormat("An error occured when attempting to submit custom event '{0}' ({1}).", name, analyticsResult);
			}
			return analyticsResult;
		}

		private static AnalyticsResult GenericTimerEvent(string context, float timeInMilliseconds)
		{
			return CustomEvent("perfGenericTimer", new Dictionary<string, object>
			{
				{
					"context",
					context
				},
				{
					"time_elapsed",
					timeInMilliseconds
				}
			});
		}

		private static AnalyticsResult SceneLoadEvent(string sceneName, float loadTimeInMilliseconds, int memoryDeltaInBytes)
		{
			return CustomEvent("perfSceneLoad", new Dictionary<string, object>
			{
				{
					"scene_name",
					sceneName
				},
				{
					"load_time",
					loadTimeInMilliseconds
				},
				{
					"memory_delta_between_loads(bytes)",
					memoryDeltaInBytes
				}
			});
		}
	}
}
