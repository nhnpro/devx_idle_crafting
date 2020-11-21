using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	public class Ellipse : SplinePrimitive
	{
		public float xRadius = 1f;

		public float yRadius = 1f;

		protected override void Generate()
		{
			base.Generate();
			type = Spline.Type.Bezier;
			closed = true;
			CreatePoints(5, SplinePoint.Type.SmoothMirrored);
			points[0].position = Vector3.up * yRadius;
			points[0].SetTangentPosition(points[0].position + Vector3.right * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * xRadius);
			points[1].position = Vector3.left * xRadius;
			points[1].SetTangentPosition(points[1].position + Vector3.up * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * yRadius);
			points[2].position = Vector3.down * yRadius;
			points[2].SetTangentPosition(points[2].position + Vector3.left * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * xRadius);
			points[3].position = Vector3.right * xRadius;
			points[3].SetTangentPosition(points[3].position + Vector3.down * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * yRadius);
			points[4] = points[0];
		}
	}
}
