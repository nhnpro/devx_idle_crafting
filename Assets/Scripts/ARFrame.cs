using System;

namespace UnityEngine.XR.iOS
{
	public struct ARFrame
	{
		public double timestamp;

		public IntPtr capturedImage;

		public ARCamera camera;

		private ARLightEstimate lightEstimate;
	}
}
