namespace UnityEngine.XR.iOS
{
	public class UnityARMatrixOps
	{
		public static Vector3 GetPosition(Matrix4x4 matrix)
		{
			Vector3 result = matrix.GetColumn(3);
			result.z = 0f - result.z;
			return result;
		}

		public static Quaternion GetRotation(Matrix4x4 matrix)
		{
			Quaternion result = QuaternionFromMatrix(matrix);
			result.z = 0f - result.z;
			result.w = 0f - result.w;
			return result;
		}

		private static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
			result.x *= Mathf.Sign(result.x * (m[2, 1] - m[1, 2]));
			result.y *= Mathf.Sign(result.y * (m[0, 2] - m[2, 0]));
			result.z *= Mathf.Sign(result.z * (m[1, 0] - m[0, 1]));
			return result;
		}
	}
}
