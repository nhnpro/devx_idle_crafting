using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitializationManager : MonoBehaviour
{
	[SerializeField]
	private Slider ProgressSlider;

	private AsyncOperation m_asynLoading;

	protected void Start()
	{
		if (GooglePlayServicesChecker.UpToDate())
		{
			StartCoroutine(MainLoader());
		}
		else
		{
			(from p in Observable.EveryApplicationPause()
				where !p && GooglePlayServicesChecker.UpToDate()
				select p).Take(1).Subscribe(delegate
			{
				StartCoroutine(MainLoader());
			}).AddTo(this);
		}
		ProgressSlider.value = 0f;
	}

	protected void Update()
	{
		if (m_asynLoading != null)
		{
			ProgressSlider.value = m_asynLoading.progress;
		}
	}

	private IEnumerator MainLoader()
	{
		InitializeEnvironmentVariables();
		yield return null;
		LoadGameSettings();
		if (ClearCacheIfOld())
		{
			SceneLoadHelper.LoadInitScene();
			yield break;
		}
		StartCoroutine(FetchConfigsRoutine());
		ConnectivityService.StartUp();
		ServerTimeService.StartUp();
		PersistentSingleton<SaveLoad>.Instance.Load(PlayerData.Instance);
		PersistentSingleton<SessionService>.Instance.StartUp();
		PersistentSingleton<MainSaver>.Instance.StartUp();
		while (!ConfigLoader.Fetched)
		{
			yield return null;
		}
		if (ConfigLoader.OverrideCacheWithFirebaseConfigs())
		{
			UnityEngine.Debug.LogWarning("Override configs and restart game");
			SceneLoadHelper.LoadInitScene();
			yield break;
		}
		PersistentSingleton<LocalizationService>.Instance.ParseLocalization();
		if (IsTooOld())
		{
			Debug.LogError("Show Force Update");
			// ForceUpdateService.ShowForceUpdateDialog();
			yield break;
		}
		if (HaventAcceptedYet())
		{
			AcceptOurTermsService.ShowTOSAndPPPopup();
		}
		PersistentSingleton<Economies>.Instance.LoadConfigs();
		InitializeGraphicsQuality();
		yield return null;
		PersistentSingleton<FlooredProvidersFactory>.Instance.Init();
		yield return null;
		PersistentSingleton<AdService>.Instance.Init();
		yield return null;
		PersistentSingleton<ARService>.Instance.InitializeCustomLevels();
		yield return null;
		PersistentSingleton<IAPService>.Instance.InitializePurchasing();
		yield return null;
		PersistentSingleton<AppsFlyerService>.Instance.InitializeAppsFlyer();
		yield return null;
		PersistentSingleton<AnalyticsService>.Instance.InitializeAnalytics();
		yield return null;
		PersistentSingleton<FacebookAPIService>.Instance.InitializeFB();
		yield return null;
		PersistentSingleton<GlobalAnalyticsParameters>.Instance.InitializeGlobalAnalyticsParameters();
		PersistentSingleton<GameAnalytics>.Instance.InitializeGameAnalytics();
		yield return null;
		PersistentSingleton<NotificationRunner>.Instance.InitializeNotifications();
		yield return null;
		while (HaventAcceptedYet())
		{
			yield return null;
		}
		if (SceneLoader.InitialScene == string.Empty)
		{
			m_asynLoading = SceneManager.LoadSceneAsync("Main");
		}
		else
		{
			m_asynLoading = SceneManager.LoadSceneAsync(SceneLoader.InitialScene);
		}
		yield return m_asynLoading;
	}

	private IEnumerator FetchConfigsRoutine()
	{
		yield return ConfigLoader.FetchFirebaseConfigs();
	}

	private void LoadGameSettings()
	{
		string text = PersistentSingleton<StringCache>.Instance.Get("Settings");
		if (text != null)
		{
			GameSettings.JSONToSettings(JSONObject.Create(text), PersistentSingleton<GameSettings>.Instance);
		}
	}

	private void InitializeGraphicsQuality()
	{
		if (PlayerPrefs.GetInt("InitialGraphicQuality", -1) != -1)
		{
			return;
		}
		GpuPerfData gpuPerfData = PersistentSingleton<Economies>.Instance.GpuPerf.Find((GpuPerfData gpu) => gpu.Gpu == SystemInfo.graphicsDeviceName);
		if (gpuPerfData != null)
		{
			if (gpuPerfData.Perf == GpuPerfEnum.High)
			{
				QualitySettingsRunner.SetInitialHighQuality();
			}
			else
			{
				QualitySettingsRunner.SetInitialLowQuality();
			}
		}
		else if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || SystemInfo.systemMemorySize > 3000)
		{
			QualitySettingsRunner.SetInitialHighQuality();
		}
		else
		{
			QualitySettingsRunner.SetInitialLowQuality();
		}
	}

	private void InitializeEnvironmentVariables()
	{
		Application.targetFrameRate = PlayerPrefs.GetInt("FrameRate", 60);
		Screen.sleepTimeout = -1;
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		AndroidJNIHelper.debug = false;
	}

	private bool ClearCacheIfOld()
	{
		if (PersistentSingleton<GameSettings>.Instance.DataVersion >= 3)
		{
			return false;
		}
		PersistentSingleton<StringCache>.Instance.Clear();
		return true;
	}

	private string GetVersion()
	{
		return Application.version;
	}

	private bool IsTooOld()
	{
		return false;
		Version v = new Version(GetVersion());
		Version v2 = new Version(PersistentSingleton<GameSettings>.Instance.MinimumSupportedVersion);
		return v < v2;
	}

	private bool HaventAcceptedYet()
	{
		return false;//PlayerData.Instance.AcceptedVersion < PersistentSingleton<GameSettings>.Instance.MinimumAcceptedVersion;
	}
}
