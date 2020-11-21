namespace UnityEngine.XR.iOS
{
	public struct UnityARLightData
	{
		public LightDataType arLightingType;

		public UnityARLightEstimate arLightEstimate;

		public UnityARDirectionalLightEstimate arDirectonalLightEstimate;

		public UnityARLightData(LightDataType ldt, UnityARLightEstimate ule, UnityARDirectionalLightEstimate udle)
		{
			arLightingType = ldt;
			arLightEstimate = ule;
			arDirectonalLightEstimate = udle;
		}
	}
}
