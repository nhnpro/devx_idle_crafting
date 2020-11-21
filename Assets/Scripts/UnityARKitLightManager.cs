using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.iOS;

public class UnityARKitLightManager : MonoBehaviour
{
	private Light[] lightsInScene;

	private SphericalHarmonicsL2 shl;

	private void Start()
	{
		lightsInScene = FindAllLights();
		shl = default(SphericalHarmonicsL2);
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateLightEstimations;
	}

	private void OnDestroy()
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateLightEstimations;
	}

	private Light[] FindAllLights()
	{
		return UnityEngine.Object.FindObjectsOfType<Light>();
	}

	private void UpdateLightEstimations(UnityARCamera camera)
	{
		if (camera.lightData.arLightingType == LightDataType.LightEstimate)
		{
			UpdateBasicLightEstimation(camera.lightData.arLightEstimate);
		}
		else if (camera.lightData.arLightingType == LightDataType.DirectionalLightEstimate)
		{
			UpdateDirectionalLightEstimation(camera.lightData.arDirectonalLightEstimate);
		}
	}

	private void UpdateBasicLightEstimation(UnityARLightEstimate uarle)
	{
		Light[] array = lightsInScene;
		foreach (Light light in array)
		{
			float ambientIntensity = uarle.ambientIntensity;
			light.intensity = ambientIntensity / 1000f;
			light.colorTemperature = uarle.ambientColorTemperature;
		}
	}

	private void UpdateDirectionalLightEstimation(UnityARDirectionalLightEstimate uardle)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				shl[i, j] = uardle.sphericalHarmonicsCoefficients[i * 9 + j];
			}
		}
		if (LightmapSettings.lightProbes != null)
		{
			int count = LightmapSettings.lightProbes.count;
			if (count > 0)
			{
				SphericalHarmonicsL2[] bakedProbes = LightmapSettings.lightProbes.bakedProbes;
				for (int k = 0; k < count; k++)
				{
					bakedProbes[k] = shl;
				}
			}
		}
		RenderSettings.ambientProbe = shl;
		RenderSettings.ambientMode = AmbientMode.Custom;
	}
}
