using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
	public class ARFaceGeometry
	{
		private UnityARFaceGeometry uFaceGeometry;

		public int vertexCount => uFaceGeometry.vertexCount;

		public int triangleCount => uFaceGeometry.triangleCount;

		public int textureCoordinateCount => uFaceGeometry.textureCoordinateCount;

		public Vector3[] vertices => MarshalVertices(uFaceGeometry.vertices, vertexCount);

		public Vector2[] textureCoordinates => MarshalTexCoords(uFaceGeometry.textureCoordinates, textureCoordinateCount);

		public int[] triangleIndices => MarshalIndices(uFaceGeometry.triangleIndices, triangleCount);

		public ARFaceGeometry(UnityARFaceGeometry ufg)
		{
			uFaceGeometry = ufg;
		}

		private Vector3[] MarshalVertices(IntPtr ptrFloatArray, int vertCount)
		{
			int num = vertCount * 4;
			float[] array = new float[num];
			Marshal.Copy(ptrFloatArray, array, 0, num);
			Vector3[] array2 = new Vector3[vertCount];
			for (int i = 0; i < num; i++)
			{
				array2[i / 4].x = array[i++];
				array2[i / 4].y = array[i++];
				array2[i / 4].z = 0f - array[i++];
			}
			return array2;
		}

		private int[] MarshalIndices(IntPtr ptrIndices, int triCount)
		{
			int num = triCount * 3;
			short[] array = new short[num];
			Marshal.Copy(ptrIndices, array, 0, num);
			int[] array2 = new int[num];
			for (int i = 0; i < num; i += 3)
			{
				array2[i] = array[i];
				array2[i + 1] = array[i + 2];
				array2[i + 2] = array[i + 1];
			}
			return array2;
		}

		private Vector2[] MarshalTexCoords(IntPtr ptrTexCoords, int texCoordCount)
		{
			int num = texCoordCount * 2;
			float[] array = new float[num];
			Marshal.Copy(ptrTexCoords, array, 0, num);
			Vector2[] array2 = new Vector2[texCoordCount];
			for (int i = 0; i < num; i++)
			{
				array2[i / 2].x = array[i++];
				array2[i / 2].y = array[i];
			}
			return array2;
		}
	}
}
