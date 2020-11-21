using UnityEngine;

namespace Dreamteck
{
	public static class LinearAlgebraUtility
	{
		public static Vector3 ProjectOnLine(Vector3 fromPoint, Vector3 toPoint, Vector3 project)
		{
			Vector3 vector = Vector3.Project(project - fromPoint, toPoint - fromPoint) + fromPoint;
			Vector3 rhs = toPoint - fromPoint;
			Vector3 lhs = vector - fromPoint;
			float num = Vector3.Dot(lhs, rhs);
			if (num > 0f)
			{
				if (lhs.sqrMagnitude <= rhs.sqrMagnitude)
				{
					return vector;
				}
				return toPoint;
			}
			return fromPoint;
		}

		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 vector = b - a;
			Vector3 lhs = value - a;
			return Vector3.Dot(lhs, vector) / Vector3.Dot(vector, vector);
		}
	}
}
