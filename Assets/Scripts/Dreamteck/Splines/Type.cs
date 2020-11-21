using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	[Serializable]
	public struct SplinePoint
	{
		public enum Type
		{
			SmoothMirrored,
			Broken,
			SmoothFree
		}

		[FormerlySerializedAs("type")]
		public Type _type;

		public Vector3 position;

		public Color color;

		public Vector3 normal;

		public float size;

		public Vector3 tangent;

		public Vector3 tangent2;

		public Type type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				if (value == Type.SmoothMirrored)
				{
					SmoothMirrorTangent2();
				}
			}
		}

		public SplinePoint(Vector3 p)
		{
			position = p;
			tangent = p;
			tangent2 = p;
			color = Color.white;
			normal = Vector3.up;
			size = 1f;
			_type = Type.SmoothMirrored;
			SmoothMirrorTangent2();
		}

		public SplinePoint(Vector3 p, Vector3 t)
		{
			position = p;
			tangent = t;
			tangent2 = p + (p - t);
			color = Color.white;
			normal = Vector3.up;
			size = 1f;
			_type = Type.SmoothMirrored;
			SmoothMirrorTangent2();
		}

		public SplinePoint(Vector3 pos, Vector3 tan, Vector3 nor, float s, Color col)
		{
			position = pos;
			tangent = tan;
			tangent2 = pos + (pos - tan);
			normal = nor;
			size = s;
			color = col;
			_type = Type.SmoothMirrored;
			SmoothMirrorTangent2();
		}

		public SplinePoint(Vector3 pos, Vector3 tan, Vector3 tan2, Vector3 nor, float s, Color col)
		{
			position = pos;
			tangent = tan;
			tangent2 = tan2;
			normal = nor;
			size = s;
			color = col;
			_type = Type.Broken;
			switch (_type)
			{
			case Type.SmoothMirrored:
				SmoothMirrorTangent2();
				break;
			case Type.SmoothFree:
				SmoothFreeTangent2();
				break;
			}
		}

		public SplinePoint(SplinePoint source)
		{
			position = source.position;
			tangent = source.tangent;
			tangent2 = source.tangent2;
			color = source.color;
			normal = source.normal;
			size = source.size;
			_type = source.type;
			switch (_type)
			{
			case Type.SmoothMirrored:
				SmoothMirrorTangent2();
				break;
			case Type.SmoothFree:
				SmoothFreeTangent2();
				break;
			}
		}

		public static SplinePoint Lerp(SplinePoint a, SplinePoint b, float t)
		{
			SplinePoint result = a;
			if (a.type == Type.Broken || b.type == Type.Broken)
			{
				result.type = Type.Broken;
			}
			else if (a.type == Type.SmoothFree || b.type == Type.SmoothFree)
			{
				result.type = Type.SmoothFree;
			}
			else
			{
				result.type = Type.SmoothMirrored;
			}
			result.position = Vector3.Lerp(a.position, b.position, t);
			GetInterpolatedTangents(a, b, t, out result.tangent, out result.tangent2);
			result.color = Color.Lerp(a.color, b.color, t);
			result.size = Mathf.Lerp(a.size, b.size, t);
			result.normal = Vector3.Slerp(a.normal, b.normal, t);
			return result;
		}

		private static void GetInterpolatedTangents(SplinePoint a, SplinePoint b, float t, out Vector3 t1, out Vector3 t2)
		{
			Vector3 a2 = (1f - t) * a.position + t * a.tangent2;
			Vector3 a3 = (1f - t) * a.tangent2 + t * b.tangent;
			Vector3 a4 = (1f - t) * b.tangent + t * b.position;
			Vector3 vector = (1f - t) * a2 + t * a3;
			Vector3 vector2 = (1f - t) * a3 + t * a4;
			t1 = vector;
			t2 = vector2;
		}

		public void SetPosition(Vector3 pos)
		{
			tangent -= position - pos;
			tangent2 -= position - pos;
			position = pos;
		}

		public void SetTangentPosition(Vector3 pos)
		{
			tangent = pos;
			switch (_type)
			{
			case Type.SmoothMirrored:
				SmoothMirrorTangent2();
				break;
			case Type.SmoothFree:
				SmoothFreeTangent2();
				break;
			}
		}

		public void SetTangent2Position(Vector3 pos)
		{
			tangent2 = pos;
			switch (_type)
			{
			case Type.SmoothMirrored:
				SmoothMirrorTangent();
				break;
			case Type.SmoothFree:
				SmoothFreeTangent();
				break;
			}
		}

		private void SmoothMirrorTangent2()
		{
			tangent2 = position + (position - tangent);
		}

		private void SmoothMirrorTangent()
		{
			tangent = position + (position - tangent2);
		}

		private void SmoothFreeTangent2()
		{
			tangent2 = position + (position - tangent).normalized * (tangent2 - position).magnitude;
		}

		private void SmoothFreeTangent()
		{
			tangent = position + (position - tangent2).normalized * (tangent - position).magnitude;
		}
	}
}
