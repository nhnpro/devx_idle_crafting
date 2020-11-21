using System;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace Utils
{
	[Serializable]
	public class serializableUnityARMatrix4x4
	{
		public SerializableVector4 column0;

		public SerializableVector4 column1;

		public SerializableVector4 column2;

		public SerializableVector4 column3;

		public serializableUnityARMatrix4x4(SerializableVector4 v0, SerializableVector4 v1, SerializableVector4 v2, SerializableVector4 v3)
		{
			column0 = v0;
			column1 = v1;
			column2 = v2;
			column3 = v3;
		}

		public static implicit operator serializableUnityARMatrix4x4(UnityARMatrix4x4 rValue)
		{
			return new serializableUnityARMatrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
		}

		public static implicit operator UnityARMatrix4x4(serializableUnityARMatrix4x4 rValue)
		{
			return new UnityARMatrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
		}

		public static implicit operator serializableUnityARMatrix4x4(Matrix4x4 rValue)
		{
			return new serializableUnityARMatrix4x4(rValue.GetColumn(0), rValue.GetColumn(1), rValue.GetColumn(2), rValue.GetColumn(3));
		}

		public static implicit operator Matrix4x4(serializableUnityARMatrix4x4 rValue)
		{
			return new Matrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
		}
	}
}
