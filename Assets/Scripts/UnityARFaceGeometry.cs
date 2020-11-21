using System;

namespace UnityEngine.XR.iOS
{
	public struct UnityARFaceGeometry
	{
		public int vertexCount;

		public IntPtr vertices;

		public int textureCoordinateCount;

		public IntPtr textureCoordinates;

		public int triangleCount;

		public IntPtr triangleIndices;
	}
}
