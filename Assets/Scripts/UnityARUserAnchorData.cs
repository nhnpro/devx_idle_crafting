using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public struct UnityARUserAnchorData
	{
		public IntPtr ptrIdentifier;

		public UnityARMatrix4x4 transform;

		public string identifierStr => Marshal.PtrToStringAuto(ptrIdentifier);

		public static UnityARUserAnchorData UnityARUserAnchorDataFromGameObject(GameObject go)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(go.transform.position, go.transform.rotation, go.transform.localScale);
			UnityARUserAnchorData result = default(UnityARUserAnchorData);
			result.transform.column0 = matrix4x.GetColumn(0);
			result.transform.column1 = matrix4x.GetColumn(1);
			result.transform.column2 = matrix4x.GetColumn(2);
			result.transform.column3 = matrix4x.GetColumn(3);
			return result;
		}
	}
}
