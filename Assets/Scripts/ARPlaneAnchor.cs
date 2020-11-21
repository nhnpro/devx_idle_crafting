namespace UnityEngine.XR.iOS
{
	public struct ARPlaneAnchor
	{
		public string identifier;

		public Matrix4x4 transform;

		public ARPlaneAnchorAlignment alignment;

		public Vector3 center;

		public Vector3 extent;
	}
}
