using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	public class Ngon : SplinePrimitive
	{
		public float radius = 1f;

		public int sides = 3;

		protected override void Generate()
		{
			base.Generate();
			type = Spline.Type.Linear;
			closed = true;
			CreatePoints(sides + 1, SplinePoint.Type.SmoothMirrored);
			for (int i = 0; i < sides; i++)
			{
				float num = (float)i / (float)sides;
				Vector3 position = Quaternion.AngleAxis(360f * num, Vector3.forward) * Vector3.right * radius;
				points[i].SetPosition(position);
			}
			points[points.Length - 1] = points[0];
		}
	}
}
