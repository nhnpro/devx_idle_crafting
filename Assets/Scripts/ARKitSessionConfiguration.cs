using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct ARKitSessionConfiguration
	{
		public UnityARAlignment alignment;

		public bool getPointCloudData;

		public bool enableLightEstimation;

		public bool IsSupported
		{
			get
			{
				return IsARKitSessionConfigurationSupported();
			}
			private set
			{
			}
		}

		public ARKitSessionConfiguration(UnityARAlignment alignment = UnityARAlignment.UnityARAlignmentGravity, bool getPointCloudData = false, bool enableLightEstimation = false)
		{
			this.getPointCloudData = getPointCloudData;
			this.alignment = alignment;
			this.enableLightEstimation = enableLightEstimation;
		}

		[DllImport("__Internal")]
		private static extern bool IsARKitSessionConfigurationSupported();
	}
}
