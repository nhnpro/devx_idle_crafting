namespace UnityEngine.XR.iOS
{
	public struct UnityMarshalLightData
	{
		public LightDataType arLightingType;

		public UnityARLightEstimate arLightEstimate;

		public MarshalDirectionalLightEstimate arDirectonalLightEstimate;

		public UnityMarshalLightData(LightDataType ldt, UnityARLightEstimate ule, MarshalDirectionalLightEstimate mdle)
		{
			arLightingType = ldt;
			arLightEstimate = ule;
			arDirectonalLightEstimate = mdle;
		}

		public static implicit operator UnityARLightData(UnityMarshalLightData rValue)
		{
			UnityARDirectionalLightEstimate udle = null;
			if (rValue.arLightingType == LightDataType.DirectionalLightEstimate)
			{
				Vector4 primaryDirAndIntensity = rValue.arDirectonalLightEstimate.primaryDirAndIntensity;
				Vector3 direction = new Vector3(primaryDirAndIntensity.x, primaryDirAndIntensity.y, primaryDirAndIntensity.z);
				float[] sphericalHarmonicCoefficients = rValue.arDirectonalLightEstimate.SphericalHarmonicCoefficients;
				udle = new UnityARDirectionalLightEstimate(sphericalHarmonicCoefficients, direction, primaryDirAndIntensity.w);
			}
			return new UnityARLightData(rValue.arLightingType, rValue.arLightEstimate, udle);
		}
	}
}
