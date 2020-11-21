using UnityEngine;

public static class MathExt
{
	public static Vector3 x0z(this Vector3 vec)
	{
		return new Vector3(vec.x, 0f, vec.z);
	}

	public static AxisAngle AngleBetween(Vector3 v0, Vector3 v1)
	{
		v0.Normalize();
		v1.Normalize();
		float value = Vector3.Dot(v0, v1);
		float angle = Mathf.Acos(Mathf.Clamp(value, -1f, 1f));
		Vector3 normalized = Vector3.Cross(v0, v1).normalized;
		AxisAngle result = default(AxisAngle);
		result.Axis = normalized;
		result.Angle = angle;
		return result;
	}
}
