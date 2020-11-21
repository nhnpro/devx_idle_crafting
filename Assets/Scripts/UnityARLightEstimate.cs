using System;

namespace UnityEngine.XR.iOS
{
	[Serializable]
	public struct UnityARLightEstimate
	{
		public float ambientIntensity;

		public float ambientColorTemperature;

		public UnityARLightEstimate(float intensity, float temperature)
		{
			ambientIntensity = intensity;
			ambientColorTemperature = temperature;
		}
	}
}
