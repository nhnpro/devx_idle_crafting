using System.Collections.Generic;
using Unity.AutoTune;
using UnityEngine;

public class AutoTuneSample : MonoBehaviour
{
	public GameObject testParticleSystem;

	private bool _experimentStarted;

	private void Start()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("particlesRate", 100f);
		Dictionary<string, object> defaultValues = dictionary;
		AutoTune.Init("1", usePersistentPath: true, defaultValues);
		AutoTune.Fetch(GotSettings);
	}

	public void OnGUI()
	{
		if (_experimentStarted)
		{
			if (GUILayout.Button("End Experiment"))
			{
				AutoTune.GetPerfRecorder().EndExperiment();
				_experimentStarted = false;
			}
		}
		else if (GUILayout.Button("Start Experiment"))
		{
			AutoTune.GetPerfRecorder().BeginExperiment("MyExperiment");
			_experimentStarted = true;
		}
	}

	private void GotSettings(Dictionary<string, object> settings, int group)
	{
		ParticleSystem component = testParticleSystem.GetComponent<ParticleSystem>();
		var componentEmission = component.emission;
		componentEmission.rateOverTime = (float)settings["particlesRate"];
	}

	private void Update()
	{
	}
}
