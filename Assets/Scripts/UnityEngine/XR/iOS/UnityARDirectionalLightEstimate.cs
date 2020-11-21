namespace UnityEngine.XR.iOS
{
	public class UnityARDirectionalLightEstimate
	{
		public Vector3 primaryLightDirection;

		public float primaryLightIntensity;

		public float[] sphericalHarmonicsCoefficients;

		public UnityARDirectionalLightEstimate(float[] SHC, Vector3 direction, float intensity)
		{
			sphericalHarmonicsCoefficients = SHC;
			primaryLightDirection = direction;
			primaryLightIntensity = intensity;
		}
	}
}
