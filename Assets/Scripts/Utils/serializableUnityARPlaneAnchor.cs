using System;
using System.Text;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace Utils
{
	[Serializable]
	public class serializableUnityARPlaneAnchor
	{
		public serializableUnityARMatrix4x4 worldTransform;

		public SerializableVector4 center;

		public SerializableVector4 extent;

		public ARPlaneAnchorAlignment planeAlignment;

		public byte[] identifierStr;

		public serializableUnityARPlaneAnchor(serializableUnityARMatrix4x4 wt, SerializableVector4 ctr, SerializableVector4 ext, ARPlaneAnchorAlignment apaa, byte[] idstr)
		{
			worldTransform = wt;
			center = ctr;
			extent = ext;
			planeAlignment = apaa;
			identifierStr = idstr;
		}

		public static implicit operator serializableUnityARPlaneAnchor(ARPlaneAnchor rValue)
		{
			serializableUnityARMatrix4x4 wt = rValue.transform;
			SerializableVector4 ctr = new SerializableVector4(rValue.center.x, rValue.center.y, rValue.center.z, 1f);
			SerializableVector4 ext = new SerializableVector4(rValue.extent.x, rValue.extent.y, rValue.extent.z, 1f);
			byte[] bytes = Encoding.UTF8.GetBytes(rValue.identifier);
			return new serializableUnityARPlaneAnchor(wt, ctr, ext, rValue.alignment, bytes);
		}

		public static implicit operator ARPlaneAnchor(serializableUnityARPlaneAnchor rValue)
		{
			ARPlaneAnchor result = default(ARPlaneAnchor);
			result.identifier = Encoding.UTF8.GetString(rValue.identifierStr);
			result.center = new Vector3(rValue.center.x, rValue.center.y, rValue.center.z);
			result.extent = new Vector3(rValue.extent.x, rValue.extent.y, rValue.extent.z);
			result.alignment = rValue.planeAlignment;
			result.transform = rValue.worldTransform;
			return result;
		}
	}
}
