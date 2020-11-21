using UnityEngine;

public static class MathUtils
{
	public static Vector3 ProjectToFloor(Ray ray, float height)
	{
		Vector3 direction = ray.direction;
		if (direction.y == 0f)
		{
			return ray.origin;
		}
		Vector3 direction2 = ray.direction;
		float num = 0f - direction2.x;
		Vector3 direction3 = ray.direction;
		float num2 = num / direction3.y;
		Vector3 direction4 = ray.direction;
		float num3 = 0f - direction4.z;
		Vector3 direction5 = ray.direction;
		float num4 = num3 / direction5.y;
		Vector3 vector = ray.origin - Vector3.up * height;
		return new Vector3(vector.x + num2 * vector.y, height, vector.z + num4 * vector.y);
	}
}
