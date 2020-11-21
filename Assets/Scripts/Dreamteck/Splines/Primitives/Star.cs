using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	public class Star : SplinePrimitive
	{
		public float radius = 1f;

		public float depth = 0.5f;

		public int sides = 5;

		protected override void Generate()
		{
			base.Generate();
			type = Spline.Type.Linear;
			closed = true;
			CreatePoints(sides * 2 + 1, SplinePoint.Type.SmoothMirrored);
			float num = radius * depth;
			for (int i = 0; i < sides * 2; i++)
			{
				float num2 = (float)i / (float)(sides * 2);
				Vector3 position = Quaternion.AngleAxis(180f + 360f * num2, Vector3.forward) * Vector3.right * (((float)i % 2f != 0f) ? num : radius);
				points[i].SetPosition(position);
			}
			points[points.Length - 1] = points[0];
		}
	}
}
