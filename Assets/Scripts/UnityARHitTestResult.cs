using System;

namespace UnityEngine.XR.iOS
{
	public struct UnityARHitTestResult
	{
		public ARHitTestResultType type;

		public double distance;

		public Matrix4x4 localTransform;

		public Matrix4x4 worldTransform;

		public IntPtr anchor;

		public bool isValid;
	}
}
