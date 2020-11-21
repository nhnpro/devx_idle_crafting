using System;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace Utils
{
	[Serializable]
	public class serializableUnityARLightData
	{
		public LightDataType whichLight;

		public serializableSHC lightSHC;

		public SerializableVector4 primaryLightDirAndIntensity;

		public float ambientIntensity;

		public float ambientColorTemperature;

		private serializableUnityARLightData(UnityARLightData lightData)
		{
			whichLight = lightData.arLightingType;
			if (whichLight == LightDataType.DirectionalLightEstimate)
			{
				lightSHC = lightData.arDirectonalLightEstimate.sphericalHarmonicsCoefficients;
				Vector3 primaryLightDirection = lightData.arDirectonalLightEstimate.primaryLightDirection;
				float primaryLightIntensity = lightData.arDirectonalLightEstimate.primaryLightIntensity;
				primaryLightDirAndIntensity = new SerializableVector4(primaryLightDirection.x, primaryLightDirection.y, primaryLightDirection.z, primaryLightIntensity);
			}
			else
			{
				ambientIntensity = lightData.arLightEstimate.ambientIntensity;
				ambientColorTemperature = lightData.arLightEstimate.ambientColorTemperature;
			}
		}

		public static implicit operator serializableUnityARLightData(UnityARLightData rValue)
		{
			return new serializableUnityARLightData(rValue);
		}

		public static implicit operator UnityARLightData(serializableUnityARLightData rValue)
		{
			UnityARDirectionalLightEstimate udle = null;
			UnityARLightEstimate ule = new UnityARLightEstimate(rValue.ambientIntensity, rValue.ambientColorTemperature);
			if (rValue.whichLight == LightDataType.DirectionalLightEstimate)
			{
				udle = new UnityARDirectionalLightEstimate(direction: new Vector3(rValue.primaryLightDirAndIntensity.x, rValue.primaryLightDirAndIntensity.y, rValue.primaryLightDirAndIntensity.z), SHC: rValue.lightSHC, intensity: rValue.primaryLightDirAndIntensity.w);
			}
			return new UnityARLightData(rValue.whichLight, ule, udle);
		}
	}
}
