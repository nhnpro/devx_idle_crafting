using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct UnityARFaceAnchorData
	{
		public IntPtr ptrIdentifier;

		public UnityARMatrix4x4 transform;

		public UnityARFaceGeometry faceGeometry;

		public IntPtr blendShapes;

		public string identifierStr => Marshal.PtrToStringAuto(ptrIdentifier);
	}
}
