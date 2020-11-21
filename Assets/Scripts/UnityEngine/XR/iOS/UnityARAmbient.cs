namespace UnityEngine.XR.iOS
{
	public class UnityARAmbient : MonoBehaviour
	{
		private Light l;

		public void Start()
		{
			l = GetComponent<Light>();
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateLightEstimation;
		}

		private void UpdateLightEstimation(UnityARCamera camera)
		{
			if (camera.lightData.arLightingType == LightDataType.LightEstimate)
			{
				float ambientIntensity = camera.lightData.arLightEstimate.ambientIntensity;
				l.intensity = ambientIntensity / 1000f;
				l.colorTemperature = camera.lightData.arLightEstimate.ambientColorTemperature;
			}
		}

		private void OnDestroy()
		{
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateLightEstimation;
		}
	}
}
