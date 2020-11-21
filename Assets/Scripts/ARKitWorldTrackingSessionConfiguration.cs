using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct ARKitWorldTrackingSessionConfiguration
	{
		public UnityARAlignment alignment;

		public UnityARPlaneDetection planeDetection;

		public bool getPointCloudData;

		public bool enableLightEstimation;

		public bool IsSupported
		{
			get
			{
				return IsARKitWorldTrackingSessionConfigurationSupported();
			}
			private set
			{
			}
		}

		public ARKitWorldTrackingSessionConfiguration(UnityARAlignment alignment = UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal, bool getPointCloudData = false, bool enableLightEstimation = false)
		{
			this.getPointCloudData = getPointCloudData;
			this.alignment = alignment;
			this.planeDetection = planeDetection;
			this.enableLightEstimation = enableLightEstimation;
		}

		[DllImport("__Internal")]
		private static extern bool IsARKitWorldTrackingSessionConfigurationSupported();
	}
}
