namespace UnityEngine.XR.iOS
{
	public struct ARCamera
	{
		public Matrix4x4 worldTransform;

		public Vector3 eulerAngles;

		public ARTrackingQuality trackingQuality;

		public Vector3 intrinsics_row1;

		public Vector3 intrinsics_row2;

		public Vector3 intrinsics_row3;

		public ARSize imageResolution;
	}
}
