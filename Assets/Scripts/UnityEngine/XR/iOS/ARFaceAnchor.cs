using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public class ARFaceAnchor
	{
		private delegate void DictionaryVisitorHandler(IntPtr keyPtr, float value);

		private UnityARFaceAnchorData faceAnchorData;

		private static Dictionary<string, float> blendshapesDictionary;

		[CompilerGenerated]
		private static DictionaryVisitorHandler _003C_003Ef__mg_0024cache0;

		public string identifierStr => faceAnchorData.identifierStr;

		public Matrix4x4 transform
		{
			get
			{
				Matrix4x4 result = default(Matrix4x4);
				result.SetColumn(0, faceAnchorData.transform.column0);
				result.SetColumn(1, faceAnchorData.transform.column1);
				result.SetColumn(2, faceAnchorData.transform.column2);
				result.SetColumn(3, faceAnchorData.transform.column3);
				return result;
			}
		}

		public ARFaceGeometry faceGeometry => new ARFaceGeometry(faceAnchorData.faceGeometry);

		public Dictionary<string, float> blendShapes => GetBlendShapesFromNative(faceAnchorData.blendShapes);

		public ARFaceAnchor(UnityARFaceAnchorData ufad)
		{
			faceAnchorData = ufad;
			if (blendshapesDictionary == null)
			{
				blendshapesDictionary = new Dictionary<string, float>();
			}
		}

		[DllImport("__Internal")]
		private static extern void GetBlendShapesInfo(IntPtr ptrDic, DictionaryVisitorHandler handler);

		private Dictionary<string, float> GetBlendShapesFromNative(IntPtr blendShapesPtr)
		{
			blendshapesDictionary.Clear();
			GetBlendShapesInfo(blendShapesPtr, AddElementToManagedDictionary);
			return blendshapesDictionary;
		}

		[MonoPInvokeCallback(typeof(DictionaryVisitorHandler))]
		private static void AddElementToManagedDictionary(IntPtr keyPtr, float value)
		{
			string key = Marshal.PtrToStringAuto(keyPtr);
			blendshapesDictionary.Add(key, value);
		}
	}
}
