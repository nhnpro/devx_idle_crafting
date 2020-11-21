namespace UnityEngine.XR.iOS
{
	internal struct internal_UnityARCamera
	{
		public UnityARMatrix4x4 worldTransform;

		public UnityARMatrix4x4 projectionMatrix;

		public ARTrackingState trackingState;

		public ARTrackingStateReason trackingReason;

		public UnityVideoParams videoParams;

		public UnityMarshalLightData lightData;

		public UnityARMatrix4x4 displayTransform;

		public uint getPointCloudData;
	}
}
