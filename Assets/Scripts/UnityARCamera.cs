namespace UnityEngine.XR.iOS
{
	public struct UnityARCamera
	{
		public UnityARMatrix4x4 worldTransform;

		public UnityARMatrix4x4 projectionMatrix;

		public ARTrackingState trackingState;

		public ARTrackingStateReason trackingReason;

		public UnityVideoParams videoParams;

		public UnityARLightData lightData;

		public UnityARMatrix4x4 displayTransform;

		public Vector3[] pointCloudData;

		public UnityARCamera(UnityARMatrix4x4 wt, UnityARMatrix4x4 pm, ARTrackingState ats, ARTrackingStateReason atsr, UnityVideoParams uvp, UnityARLightData lightDat, UnityARMatrix4x4 dt, Vector3[] pointCloud)
		{
			worldTransform = wt;
			projectionMatrix = pm;
			trackingState = ats;
			trackingReason = atsr;
			videoParams = uvp;
			lightData = lightDat;
			displayTransform = dt;
			pointCloudData = pointCloud;
		}
	}
}
