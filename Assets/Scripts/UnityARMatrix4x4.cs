namespace UnityEngine.XR.iOS
{
	public struct UnityARMatrix4x4
	{
		public Vector4 column0;

		public Vector4 column1;

		public Vector4 column2;

		public Vector4 column3;

		public UnityARMatrix4x4(Vector4 c0, Vector4 c1, Vector4 c2, Vector4 c3)
		{
			column0 = c0;
			column1 = c1;
			column2 = c2;
			column3 = c3;
		}
	}
}
