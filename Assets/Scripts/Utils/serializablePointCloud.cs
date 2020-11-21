using System;
using UnityEngine;

namespace Utils
{
	[Serializable]
	public class serializablePointCloud
	{
		public byte[] pointCloudData;

		public serializablePointCloud(byte[] inputPoints)
		{
			pointCloudData = inputPoints;
		}

		public static implicit operator serializablePointCloud(Vector3[] vecPointCloud)
		{
			if (vecPointCloud != null)
			{
				byte[] array = new byte[vecPointCloud.Length * 4 * 3];
				for (int i = 0; i < vecPointCloud.Length; i++)
				{
					int num = i * 3;
					Buffer.BlockCopy(BitConverter.GetBytes(vecPointCloud[i].x), 0, array, num * 4, 4);
					Buffer.BlockCopy(BitConverter.GetBytes(vecPointCloud[i].y), 0, array, (num + 1) * 4, 4);
					Buffer.BlockCopy(BitConverter.GetBytes(vecPointCloud[i].z), 0, array, (num + 2) * 4, 4);
				}
				return new serializablePointCloud(array);
			}
			return new serializablePointCloud(null);
		}

		public static implicit operator Vector3[](serializablePointCloud spc)
		{
			if (spc.pointCloudData != null)
			{
				int num = spc.pointCloudData.Length / 12;
				Vector3[] array = new Vector3[num];
				for (int i = 0; i < num; i++)
				{
					int num2 = i * 3;
					array[i].x = BitConverter.ToSingle(spc.pointCloudData, num2 * 4);
					array[i].y = BitConverter.ToSingle(spc.pointCloudData, (num2 + 1) * 4);
					array[i].z = BitConverter.ToSingle(spc.pointCloudData, (num2 + 2) * 4);
				}
				return array;
			}
			return null;
		}
	}
}
