namespace UnityEngine.XR.iOS
{
	public struct ARHitTestResult
	{
		public ARHitTestResultType type;

		public double distance;

		public Matrix4x4 localTransform;

		public Matrix4x4 worldTransform;

		public string anchorIdentifier;

		public bool isValid;
	}
}
