using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[Serializable]
	public class Spline
	{
		public enum Direction
		{
			Forward = 1,
			Backward = -1
		}

		public enum Type
		{
			Hermite,
			BSpline,
			Bezier,
			Linear
		}

		public SplinePoint[] points = new SplinePoint[0];

		[SerializeField]
		private bool closed;

		public Type type = Type.Bezier;

		public AnimationCurve customValueInterpolation;

		public AnimationCurve customNormalInterpolation;

		[Range(0f, 0.9999f)]
		public double precision = 0.89999997615814209;

		private Vector3[] hermitePoints = new Vector3[4];

		public bool isClosed
		{
			get
			{
				return closed && points.Length >= 4;
			}
			set
			{
			}
		}

		public double moveStep
		{
			get
			{
				if (type == Type.Linear)
				{
					return 1f / (float)(points.Length - 1);
				}
				return 1f / (float)(iterations - 1);
			}
			set
			{
			}
		}

		public int iterations
		{
			get
			{
				if (type == Type.Linear)
				{
					return points.Length;
				}
				return DMath.CeilInt(1.0 / ((1.0 - precision) / (double)(points.Length - 1))) + 1;
			}
			set
			{
			}
		}

		public Spline(Type t)
		{
			type = t;
			points = new SplinePoint[0];
		}

		public Spline(Type t, double p)
		{
			type = t;
			precision = p;
			points = new SplinePoint[0];
		}

		public float CalculateLength(double from = 0.0, double to = 1.0, double resolution = 1.0)
		{
			if (points.Length == 0)
			{
				return 0f;
			}
			resolution = DMath.Clamp01(resolution);
			if (resolution == 0.0)
			{
				return 0f;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			if (to < from)
			{
				to = from;
			}
			double num = from;
			Vector3 b = EvaluatePosition(num);
			float num2 = 0f;
			do
			{
				num = DMath.Move(num, to, moveStep / resolution);
				Vector3 vector = EvaluatePosition(num);
				num2 += (vector - b).magnitude;
				b = vector;
			}
			while (num != to);
			return num2;
		}

		public double Project(Vector3 point, int subdivide = 3, double from = 0.0, double to = 1.0)
		{
			if (points.Length == 0)
			{
				return 0.0;
			}
			if (closed && from == 0.0 && to == 1.0)
			{
				double closestPoint = GetClosestPoint(subdivide, point, from, to, Mathf.RoundToInt(Mathf.Max(iterations / points.Length, 10)) * 5);
				if (closestPoint < moveStep)
				{
					double closestPoint2 = GetClosestPoint(subdivide, point, 0.5, to, Mathf.RoundToInt(Mathf.Max(iterations / points.Length, 10)) * 5);
					if (Vector3.Distance(point, EvaluatePosition(closestPoint2)) < Vector3.Distance(point, EvaluatePosition(closestPoint)))
					{
						return closestPoint2;
					}
				}
				return closestPoint;
			}
			return GetClosestPoint(subdivide, point, from, to, Mathf.RoundToInt(Mathf.Max(iterations / points.Length, 10)) * 5);
		}

		public bool Raycast(out RaycastHit hit, out double hitPercent, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			double num = from;
			Vector3 vector = EvaluatePosition(num);
			hitPercent = 0.0;
			if (resolution == 0.0)
			{
				hit = default(RaycastHit);
				hitPercent = 0.0;
				return false;
			}
			do
			{
				double a = num;
				num = DMath.Move(num, to, moveStep / resolution);
				Vector3 vector2 = EvaluatePosition(num);
				if (Physics.Linecast(vector, vector2, out hit, layerMask, hitTriggers))
				{
					double t = (hit.point - vector).sqrMagnitude / (vector2 - vector).sqrMagnitude;
					hitPercent = DMath.Lerp(a, num, t);
					return true;
				}
				vector = vector2;
			}
			while (num != to);
			return false;
		}

		public bool RaycastAll(out RaycastHit[] hits, out double[] hitPercents, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			double num = from;
			Vector3 vector = EvaluatePosition(num);
			List<RaycastHit> list = new List<RaycastHit>();
			List<double> list2 = new List<double>();
			if (resolution == 0.0)
			{
				hits = new RaycastHit[0];
				hitPercents = new double[0];
				return false;
			}
			bool result = false;
			do
			{
				double a = num;
				num = DMath.Move(num, to, moveStep / resolution);
				Vector3 vector2 = EvaluatePosition(num);
				RaycastHit[] array = Physics.RaycastAll(vector, vector2 - vector, Vector3.Distance(vector, vector2), layerMask, hitTriggers);
				for (int i = 0; i < array.Length; i++)
				{
					result = true;
					double t = (array[i].point - vector).sqrMagnitude / (vector2 - vector).sqrMagnitude;
					list2.Add(DMath.Lerp(a, num, t));
					list.Add(array[i]);
				}
				vector = vector2;
			}
			while (num != to);
			hits = list.ToArray();
			hitPercents = list2.ToArray();
			return result;
		}

		public Vector3 EvaluatePosition(double percent)
		{
			if (points.Length == 0)
			{
				return Vector3.zero;
			}
			Vector3 point = default(Vector3);
			EvaluatePosition(ref point, percent);
			return point;
		}

		public SplineResult Evaluate(double percent)
		{
			SplineResult result = new SplineResult();
			Evaluate(result, percent);
			return result;
		}

		public void Evaluate(SplineResult result, double percent)
		{
			if (points.Length == 0)
			{
				result = new SplineResult();
				return;
			}
			percent = DMath.Clamp01(percent);
			if (closed && points.Length <= 2)
			{
				closed = false;
			}
			if (points.Length == 1)
			{
				result.position = points[0].position;
				result.normal = points[0].normal;
				result.direction = Vector3.forward;
				result.size = points[0].size;
				result.color = points[0].color;
				result.percent = percent;
				return;
			}
			double num = (double)(points.Length - 1) * percent;
			int num2 = Mathf.Clamp(DMath.FloorInt(num), 0, points.Length - 2);
			double num3 = num - (double)num2;
			Vector3 vector = result.position = EvaluatePosition(percent);
			result.percent = percent;
			if (num2 <= points.Length - 2)
			{
				SplinePoint splinePoint = points[num2 + 1];
				if (num2 == points.Length - 2 && closed)
				{
					splinePoint = points[0];
				}
				float num4 = (float)num3;
				if (customValueInterpolation != null && (float)customValueInterpolation.keys.Length > 0f)
				{
					num4 = customValueInterpolation.Evaluate(num4);
				}
				float num5 = (float)num3;
				if (customNormalInterpolation != null && customNormalInterpolation.keys.Length > 0)
				{
					num5 = customNormalInterpolation.Evaluate(num5);
				}
				result.size = Mathf.Lerp(points[num2].size, splinePoint.size, num4);
				result.color = Color.Lerp(points[num2].color, splinePoint.color, num4);
				result.normal = Vector3.Slerp(points[num2].normal, splinePoint.normal, num5);
			}
			else if (closed)
			{
				result.size = points[0].size;
				result.color = points[0].color;
				result.normal = points[0].normal;
			}
			else
			{
				result.size = points[num2].size;
				result.color = points[num2].color;
				result.normal = points[num2].normal;
			}
			double num6 = (1.0 - precision) / (double)points.Length;
			if (percent < 1.0 - num6)
			{
				result.direction = EvaluatePosition(percent + num6) - result.position;
			}
			else if (closed)
			{
				result.direction = EvaluatePosition(percent + num6) - result.position;
				result.direction += EvaluatePosition(percent + num6 - 1.0) - EvaluatePosition(0.0);
			}
			else
			{
				result.direction = EvaluatePosition(DMath.Clamp01(percent + num6)) - result.position;
				if (Mathf.Max(result.direction.x, result.direction.y, result.direction.z) <= 0.009f)
				{
					double percent2 = DMath.Clamp01(percent - num6);
					result.direction = result.position - EvaluatePosition(percent2);
				}
			}
			result.direction.Normalize();
		}

		public void Evaluate(ref SplineResult[] samples, double from = 0.0, double to = 1.0)
		{
			if (points.Length == 0)
			{
				samples = new SplineResult[0];
				return;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp(to, from, 1.0);
			double a = from * (double)(iterations - 1);
			double a2 = to * (double)(iterations - 1);
			int num = DMath.CeilInt(a2) - DMath.FloorInt(a) + 1;
			if (samples == null)
			{
				samples = new SplineResult[num];
			}
			if (samples.Length != num)
			{
				samples = new SplineResult[num];
			}
			double num2 = from;
			double moveStep = this.moveStep;
			int num3 = 0;
			while (true)
			{
				samples[num3] = Evaluate(num2);
				num3++;
				if (num3 >= samples.Length)
				{
					break;
				}
				num2 = DMath.Move(num2, to, moveStep);
			}
		}

		public void EvaluatePositions(ref Vector3[] positions, double from = 0.0, double to = 1.0)
		{
			if (points.Length == 0)
			{
				positions = new Vector3[0];
				return;
			}
			from = DMath.Clamp01(from);
			to = DMath.Clamp(to, from, 1.0);
			double a = from * (double)(iterations - 1);
			double a2 = to * (double)(iterations - 1);
			int num = DMath.CeilInt(a2) - DMath.FloorInt(a) + 1;
			if (positions.Length != num)
			{
				positions = new Vector3[num];
			}
			double num2 = from;
			double moveStep = this.moveStep;
			int num3 = 0;
			while (true)
			{
				positions[num3] = EvaluatePosition(num2);
				num3++;
				if (num3 >= positions.Length)
				{
					break;
				}
				num2 = DMath.Move(num2, to, moveStep);
			}
		}

		public double Travel(double start, float distance, Direction direction)
		{
			if (points.Length <= 1)
			{
				return 0.0;
			}
			if (direction == Direction.Forward && start >= 1.0)
			{
				return 1.0;
			}
			if (direction == Direction.Backward && start <= 0.0)
			{
				return 0.0;
			}
			if (distance == 0f)
			{
				return DMath.Clamp01(start);
			}
			float num = 0f;
			Vector3 point = Vector3.zero;
			EvaluatePosition(ref point, start);
			Vector3 b = point;
			double a = start;
			int num2 = iterations - 1;
			int num3 = (direction != Direction.Forward) ? DMath.FloorInt(start * (double)num2) : DMath.CeilInt(start * (double)num2);
			float num4 = 0f;
			double num5;
			while (true)
			{
				num5 = (double)num3 / (double)num2;
				point = EvaluatePosition(num5);
				num4 = Vector3.Distance(point, b);
				b = point;
				num += num4;
				if (num >= distance)
				{
					break;
				}
				a = num5;
				if (direction == Direction.Forward)
				{
					if (num3 == num2)
					{
						break;
					}
					num3++;
				}
				else
				{
					if (num3 == 0)
					{
						break;
					}
					num3--;
				}
			}
			return DMath.Lerp(a, num5, 1f - (num - distance) / num4);
		}

		private void EvaluatePosition(ref Vector3 point, double percent)
		{
			percent = DMath.Clamp01(percent);
			double num = (double)(points.Length - 1) * percent;
			int num2 = Mathf.Clamp(DMath.FloorInt(num), 0, Mathf.Max(points.Length - 2, 0));
			GetPoint(ref point, num - (double)num2, num2);
		}

		private double GetClosestPoint(int iterations, Vector3 point, double start, double end, int slices)
		{
			if (iterations <= 0)
			{
				float sqrMagnitude = (point - EvaluatePosition(start)).sqrMagnitude;
				float sqrMagnitude2 = (point - EvaluatePosition(end)).sqrMagnitude;
				if (sqrMagnitude < sqrMagnitude2)
				{
					return start;
				}
				if (sqrMagnitude2 < sqrMagnitude)
				{
					return end;
				}
				return (start + end) / 2.0;
			}
			double num = 0.0;
			float num2 = float.PositiveInfinity;
			double num3 = (end - start) / (double)slices;
			double num4 = start;
			Vector3 point2 = Vector3.zero;
			while (true)
			{
				EvaluatePosition(ref point2, num4);
				float sqrMagnitude3 = (point - point2).sqrMagnitude;
				if (sqrMagnitude3 < num2)
				{
					num2 = sqrMagnitude3;
					num = num4;
				}
				if (num4 == end)
				{
					break;
				}
				num4 = DMath.Move(num4, end, num3);
			}
			double num5 = num - num3;
			if (num5 < start)
			{
				num5 = start;
			}
			double num6 = num + num3;
			if (num6 > end)
			{
				num6 = end;
			}
			return GetClosestPoint(--iterations, point, num5, num6, slices);
		}

		public void Break()
		{
			Break(0);
		}

		public void Break(int at)
		{
			if (closed && at < points.Length)
			{
				SplinePoint[] array = new SplinePoint[at];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = points[i];
				}
				for (int j = at; j < points.Length - 1; j++)
				{
					points[j - at] = points[j];
				}
				for (int k = 0; k < array.Length; k++)
				{
					points[points.Length - at + k - 1] = array[k];
				}
				points[points.Length - 1] = points[0];
				closed = false;
			}
		}

		public void Close()
		{
			if (points.Length < 4)
			{
				UnityEngine.Debug.LogError("Points need to be at least 4 to close the spline");
			}
			else
			{
				closed = true;
			}
		}

		public void ConvertToBezier()
		{
			switch (type)
			{
			case Type.Linear:
				for (int j = 0; j < points.Length; j++)
				{
					points[j].type = SplinePoint.Type.Broken;
					points[j].SetTangentPosition(points[j].position);
					points[j].SetTangent2Position(points[j].position);
				}
				break;
			case Type.Hermite:
				for (int i = 0; i < points.Length; i++)
				{
					GetHermitePoints(i);
					points[i].type = SplinePoint.Type.SmoothMirrored;
					if (i == 0)
					{
						Vector3 a = hermitePoints[1] - hermitePoints[2];
						if (closed)
						{
							a = points[points.Length - 2].position - points[i + 1].position;
							points[i].SetTangentPosition(points[i].position + a / 6f);
						}
						else
						{
							points[i].SetTangentPosition(points[i].position + a / 3f);
						}
					}
					else if (i == points.Length - 1)
					{
						Vector3 a2 = hermitePoints[2] - hermitePoints[3];
						points[i].SetTangentPosition(points[i].position + a2 / 3f);
					}
					else
					{
						Vector3 a3 = hermitePoints[0] - hermitePoints[2];
						points[i].SetTangentPosition(points[i].position + a3 / 6f);
					}
				}
				break;
			}
			type = Type.Bezier;
		}

		private void GetPoint(ref Vector3 point, double percent, int pointIndex)
		{
			if (closed && points.Length > 3)
			{
				if (pointIndex == points.Length - 2)
				{
					points[0].SetTangentPosition(points[points.Length - 1].tangent);
					points[points.Length - 1] = points[0];
				}
			}
			else
			{
				closed = false;
			}
			switch (type)
			{
			case Type.Hermite:
				HermiteGetPoint(ref point, percent, pointIndex);
				break;
			case Type.Bezier:
				BezierGetPoint(ref point, percent, pointIndex);
				break;
			case Type.BSpline:
				BSPGetPoint(ref point, percent, pointIndex);
				break;
			case Type.Linear:
				LinearGetPoint(ref point, percent, pointIndex);
				break;
			}
		}

		private void LinearGetPoint(ref Vector3 point, double t, int i)
		{
			if (points.Length == 0)
			{
				point = Vector3.zero;
			}
			else if (i + 1 < points.Length)
			{
				t = DMath.Clamp01(t);
				i = Mathf.Clamp(i, 0, points.Length - 2);
				point = Vector3.Lerp(points[i].position, points[i + 1].position, (float)t);
			}
			else if (i < points.Length)
			{
				point = points[i].position;
			}
			else
			{
				point = Vector3.zero;
			}
		}

		private void BSPGetPoint(ref Vector3 point, double t, int i)
		{
			if (points.Length > 0)
			{
				point = points[0].position;
			}
			if (points.Length > 1)
			{
				t = DMath.Clamp01(t);
				GetHermitePoints(i);
				point.x = (float)(((-3.0 * (double)hermitePoints[0].x + 3.0 * (double)hermitePoints[2].x) / 6.0 + t * ((3.0 * (double)hermitePoints[0].x - 6.0 * (double)hermitePoints[1].x + 3.0 * (double)hermitePoints[2].x) / 6.0 + t * ((double)(0f - hermitePoints[0].x) + 3.0 * (double)hermitePoints[1].x - 3.0 * (double)hermitePoints[2].x + (double)hermitePoints[3].x) / 6.0)) * t + ((double)hermitePoints[0].x + 4.0 * (double)hermitePoints[1].x + (double)hermitePoints[2].x) / 6.0);
				point.y = (float)(((-3.0 * (double)hermitePoints[0].y + 3.0 * (double)hermitePoints[2].y) / 6.0 + t * ((3.0 * (double)hermitePoints[0].y - 6.0 * (double)hermitePoints[1].y + 3.0 * (double)hermitePoints[2].y) / 6.0 + t * ((double)(0f - hermitePoints[0].y) + 3.0 * (double)hermitePoints[1].y - 3.0 * (double)hermitePoints[2].y + (double)hermitePoints[3].y) / 6.0)) * t + ((double)hermitePoints[0].y + 4.0 * (double)hermitePoints[1].y + (double)hermitePoints[2].y) / 6.0);
				point.z = (float)(((-3.0 * (double)hermitePoints[0].z + 3.0 * (double)hermitePoints[2].z) / 6.0 + t * ((3.0 * (double)hermitePoints[0].z - 6.0 * (double)hermitePoints[1].z + 3.0 * (double)hermitePoints[2].z) / 6.0 + t * ((double)(0f - hermitePoints[0].z) + 3.0 * (double)hermitePoints[1].z - 3.0 * (double)hermitePoints[2].z + (double)hermitePoints[3].z) / 6.0)) * t + ((double)hermitePoints[0].z + 4.0 * (double)hermitePoints[1].z + (double)hermitePoints[2].z) / 6.0);
			}
		}

		private void BezierGetPoint(ref Vector3 point, double t, int i)
		{
			if (points.Length > 0)
			{
				point = points[0].position;
				if (points.Length != 1 && i < points.Length - 1)
				{
					t = DMath.Clamp01(t);
					i = Mathf.Clamp(i, 0, points.Length - 2);
					Vector3 position = points[i].position;
					Vector3 tangent = points[i].tangent2;
					Vector3 tangent2 = points[i + 1].tangent;
					Vector3 position2 = points[i + 1].position;
					float num = (float)t;
					point = Mathf.Pow(1f - num, 3f) * position + 3f * Mathf.Pow(1f - num, 2f) * num * tangent + 3f * (1f - num) * Mathf.Pow(num, 2f) * tangent2 + Mathf.Pow(num, 3f) * position2;
				}
			}
		}

		private void HermiteGetPoint(ref Vector3 point, double t, int i)
		{
			double num = t * t;
			double num2 = num * t;
			if (points.Length > 0)
			{
				point = points[0].position;
			}
			if (i < points.Length && points.Length > 1)
			{
				GetHermitePoints(i);
				point.x = (float)(0.5 * (2.0 * (double)hermitePoints[1].x + (double)(0f - hermitePoints[0].x + hermitePoints[2].x) * t + (2.0 * (double)hermitePoints[0].x - 5.0 * (double)hermitePoints[1].x + (double)(4f * hermitePoints[2].x) - (double)hermitePoints[3].x) * num + ((double)(0f - hermitePoints[0].x) + 3.0 * (double)hermitePoints[1].x - 3.0 * (double)hermitePoints[2].x + (double)hermitePoints[3].x) * num2));
				point.y = (float)(0.5 * (2.0 * (double)hermitePoints[1].y + (double)(0f - hermitePoints[0].y + hermitePoints[2].y) * t + (2.0 * (double)hermitePoints[0].y - 5.0 * (double)hermitePoints[1].y + (double)(4f * hermitePoints[2].y) - (double)hermitePoints[3].y) * num + ((double)(0f - hermitePoints[0].y) + 3.0 * (double)hermitePoints[1].y - 3.0 * (double)hermitePoints[2].y + (double)hermitePoints[3].y) * num2));
				point.z = (float)(0.5 * (2.0 * (double)hermitePoints[1].z + (double)(0f - hermitePoints[0].z + hermitePoints[2].z) * t + (2.0 * (double)hermitePoints[0].z - 5.0 * (double)hermitePoints[1].z + (double)(4f * hermitePoints[2].z) - (double)hermitePoints[3].z) * num + ((double)(0f - hermitePoints[0].z) + 3.0 * (double)hermitePoints[1].z - 3.0 * (double)hermitePoints[2].z + (double)hermitePoints[3].z) * num2));
			}
		}

		private void GetHermitePoints(int i)
		{
			if (i > 0)
			{
				hermitePoints[0] = points[i - 1].position;
			}
			else if (closed && points.Length - 2 > i)
			{
				hermitePoints[0] = points[points.Length - 2].position;
			}
			else if (i + 1 < points.Length)
			{
				hermitePoints[0] = points[i].position + (points[i].position - points[i + 1].position);
			}
			else
			{
				hermitePoints[0] = points[i].position;
			}
			hermitePoints[1] = points[i].position;
			if (i + 1 < points.Length)
			{
				hermitePoints[2] = points[i + 1].position;
			}
			else if (closed && i + 2 - points.Length != i)
			{
				hermitePoints[2] = points[i + 2 - points.Length].position;
			}
			else
			{
				hermitePoints[2] = hermitePoints[1] + (hermitePoints[1] - hermitePoints[0]);
			}
			if (i + 2 < points.Length)
			{
				hermitePoints[3] = points[i + 2].position;
			}
			else if (closed && i + 3 - points.Length != i)
			{
				hermitePoints[3] = points[i + 3 - points.Length].position;
			}
			else
			{
				hermitePoints[3] = hermitePoints[2] + (hermitePoints[2] - hermitePoints[1]);
			}
		}
	}
}
