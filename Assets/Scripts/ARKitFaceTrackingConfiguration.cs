using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct ARKitFaceTrackingConfiguration
	{
		public UnityARAlignment alignment;

		public bool enableLightEstimation;

		public bool IsSupported
		{
			get
			{
				return IsARKitFaceTrackingConfigurationSupported();
			}
			private set
			{
			}
		}

		public ARKitFaceTrackingConfiguration(UnityARAlignment alignment = UnityARAlignment.UnityARAlignmentGravity, bool enableLightEstimation = false)
		{
			this.alignment = alignment;
			this.enableLightEstimation = enableLightEstimation;
		}

		[DllImport("__Internal")]
		private static extern bool IsARKitFaceTrackingConfigurationSupported();
	}
}
