using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	public class Line : SplinePrimitive
	{
		public bool mirror = true;

		public float length = 1f;

		public int segments = 1;

		protected override void Generate()
		{
			base.Generate();
			type = Spline.Type.Linear;
			closed = false;
			CreatePoints(segments + 1, SplinePoint.Type.SmoothMirrored);
			Quaternion rotation = Quaternion.Euler(base.rotation);
			Vector3 a = rotation * Vector3.forward;
			Vector3 a2 = Vector3.zero;
			if (mirror)
			{
				a2 = -a * length * 0.5f;
			}
			for (int i = 0; i < points.Length; i++)
			{
				points[i].position = a2 + a * length * ((float)i / (float)(points.Length - 1));
			}
		}
	}
}
