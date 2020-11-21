using System;

namespace UnityEngine.XR.iOS
{
	[Serializable]
	public struct UnityVideoParams
	{
		public int yWidth;

		public int yHeight;

		public int screenOrientation;

		public float texCoordScale;

		public IntPtr cvPixelBufferPtr;
	}
}
