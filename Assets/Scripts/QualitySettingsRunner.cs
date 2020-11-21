using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Performance;
using UnityEngine;

[PropertyClass]
public class QualitySettingsRunner : Singleton<QualitySettingsRunner>
{
	public ReactiveProperty<int> GraphicsDetail = new ReactiveProperty<int>(-1);

	[PropertyBool]
	public ReactiveProperty<bool> HighDetails = new ReactiveProperty<bool>(initialValue: true);

	[PropertyBool]
	public ReactiveProperty<bool> LowFPS = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> InitialHighGraphics = new ReactiveProperty<bool>(initialValue: true);

	[PropertyBool]
	public ReactiveProperty<bool> DisableSleep = new ReactiveProperty<bool>(initialValue: false);

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		GraphicsDetail.Value = PlayerPrefs.GetInt("GraphicQuality", 0);
		LowFPS.Value = (PlayerPrefs.GetInt("FrameRate", 60) == 30);
		GraphicsDetail.Subscribe(delegate(int q)
		{
			QualitySettings.SetQualityLevel(q, applyExpensiveChanges: true);
			HighDetails.Value = (q == 1);
		}).AddTo(instance);
	}

	private IEnumerator ExperimentRuotine(PerfRecorder perfRecorder, string name)
	{
		yield return new WaitForSeconds(3f);
		perfRecorder.BeginExperiment(name);
		yield return new WaitForSeconds(3f);
		perfRecorder.EndExperiment();
	}

	private void OnSettingsFetched(Dictionary<string, object> settings, int testGroup)
	{
	}

	public void ChangeFPS()
	{
		int @int = PlayerPrefs.GetInt("FrameRate", 60);
		if (@int == 60)
		{
			PlayerPrefs.SetInt("FrameRate", 30);
			Application.targetFrameRate = 30;
			LowFPS.Value = true;
		}
		else
		{
			PlayerPrefs.SetInt("FrameRate", 60);
			Application.targetFrameRate = 60;
			LowFPS.Value = false;
		}
	}

	public void ChangeQuality()
	{
		if (PlayerPrefs.GetInt("GraphicQuality", 0) == 0)
		{
			PlayerPrefs.SetInt("GraphicQuality", 1);
			GraphicsDetail.Value = 1;
		}
		else
		{
			PlayerPrefs.SetInt("GraphicQuality", 0);
			GraphicsDetail.Value = 0;
		}
	}

	public static void SetInitialLowQuality()
	{
		PlayerPrefs.SetInt("InitialGraphicQuality", 0);
		PlayerPrefs.SetInt("GraphicQuality", 0);
		QualitySettings.SetQualityLevel(0, applyExpensiveChanges: true);
	}

	public static void SetInitialHighQuality()
	{
		PlayerPrefs.SetInt("InitialGraphicQuality", 1);
		PlayerPrefs.SetInt("GraphicQuality", 1);
		QualitySettings.SetQualityLevel(1, applyExpensiveChanges: true);
	}
}
