using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dreamteck
{
	public class MeshUtility
	{
		private static Vector3[] tan1 = new Vector3[0];

		private static Vector3[] tan2 = new Vector3[0];

		private static Vector4[] meshTangents = new Vector4[0];

		public static int[] GeneratePlaneTriangles(int x, int z, bool flip, int startTriangleIndex = 0, int startVertex = 0)
		{
			int num = x * (z - 1);
			int[] triangles = new int[num * 6];
			GeneratePlaneTriangles(ref triangles, x, z, flip);
			return triangles;
		}

		public static int[] GeneratePlaneTriangles(ref int[] triangles, int x, int z, bool flip, int startTriangleIndex = 0, int startVertex = 0, bool reallocateArray = false)
		{
			int num = x * (z - 1);
			if (reallocateArray && triangles.Length != num * 6)
			{
				if (startTriangleIndex > 0)
				{
					int[] array = new int[startTriangleIndex + num * 6];
					for (int i = 0; i < startTriangleIndex; i++)
					{
						array[i] = triangles[i];
					}
					triangles = array;
				}
				else
				{
					triangles = new int[num * 6];
				}
			}
			int num2 = x + 1;
			int num3 = startTriangleIndex;
			for (int j = 0; j < num + z - 2; j++)
			{
				if ((float)(j + 1) % (float)num2 == 0f && j != 0)
				{
					j++;
				}
				if (flip)
				{
					triangles[num3++] = j + x + 1 + startVertex;
					triangles[num3++] = j + 1 + startVertex;
					triangles[num3++] = j + startVertex;
					triangles[num3++] = j + x + 1 + startVertex;
					triangles[num3++] = j + x + 2 + startVertex;
					triangles[num3++] = j + 1 + startVertex;
				}
				else
				{
					triangles[num3++] = j + startVertex;
					triangles[num3++] = j + 1 + startVertex;
					triangles[num3++] = j + x + 1 + startVertex;
					triangles[num3++] = j + 1 + startVertex;
					triangles[num3++] = j + x + 2 + startVertex;
					triangles[num3++] = j + x + 1 + startVertex;
				}
			}
			return triangles;
		}

		public static void CalculateTangents(TS_Mesh mesh)
		{
			int num = mesh.triangles.Length / 3;
			if (meshTangents.Length != mesh.vertexCount)
			{
				meshTangents = new Vector4[mesh.vertexCount];
				tan1 = new Vector3[mesh.vertexCount];
				tan2 = new Vector3[mesh.vertexCount];
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				int num3 = mesh.triangles[num2];
				int num4 = mesh.triangles[num2 + 1];
				int num5 = mesh.triangles[num2 + 2];
				float num6 = mesh.vertices[num4].x - mesh.vertices[num3].x;
				float num7 = mesh.vertices[num5].x - mesh.vertices[num3].x;
				float num8 = mesh.vertices[num4].y - mesh.vertices[num3].y;
				float num9 = mesh.vertices[num5].y - mesh.vertices[num3].y;
				float num10 = mesh.vertices[num4].z - mesh.vertices[num3].z;
				float num11 = mesh.vertices[num5].z - mesh.vertices[num3].z;
				float num12 = mesh.uv[num4].x - mesh.uv[num3].x;
				float num13 = mesh.uv[num5].x - mesh.uv[num3].x;
				float num14 = mesh.uv[num4].y - mesh.uv[num3].y;
				float num15 = mesh.uv[num5].y - mesh.uv[num3].y;
				float num16 = num12 * num15 - num13 * num14;
				float num17 = (num16 != 0f) ? (1f / num16) : 0f;
				Vector3 vector = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
				Vector3 vector2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
				tan1[num3] += vector;
				tan1[num4] += vector;
				tan1[num5] += vector;
				tan2[num3] += vector2;
				tan2[num4] += vector2;
				tan2[num5] += vector2;
				num2 += 3;
			}
			for (int j = 0; j < mesh.vertexCount; j++)
			{
				Vector3 normal = mesh.normals[j];
				Vector3 tangent = tan1[j];
				Vector3.OrthoNormalize(ref normal, ref tangent);
				meshTangents[j].x = tangent.x;
				meshTangents[j].y = tangent.y;
				meshTangents[j].z = tangent.z;
				meshTangents[j].w = ((!(Vector3.Dot(Vector3.Cross(normal, tangent), tan2[j]) < 0f)) ? 1f : (-1f));
			}
			mesh.tangents = meshTangents;
		}

		public static void MakeDoublesided(TS_Mesh input)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] normals = input.normals;
			Vector2[] uv = input.uv;
			Color[] colors = input.colors;
			int[] triangles = input.triangles;
			List<int[]> subMeshes = input.subMeshes;
			Vector3[] array = new Vector3[vertices.Length * 2];
			Vector3[] array2 = new Vector3[normals.Length * 2];
			Vector2[] array3 = new Vector2[uv.Length * 2];
			Color[] array4 = new Color[colors.Length * 2];
			int[] array5 = new int[triangles.Length * 2];
			List<int[]> list = new List<int[]>();
			for (int i = 0; i < subMeshes.Count; i++)
			{
				list.Add(new int[subMeshes[i].Length * 2]);
				subMeshes[i].CopyTo(list[i], 0);
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				array[j] = vertices[j];
				array2[j] = normals[j];
				array3[j] = uv[j];
				array4[j] = colors[j];
				array[j + vertices.Length] = vertices[j];
				array2[j + vertices.Length] = -normals[j];
				array3[j + vertices.Length] = uv[j];
				array4[j + vertices.Length] = colors[j];
			}
			for (int k = 0; k < triangles.Length; k += 3)
			{
				int num = triangles[k];
				int num2 = triangles[k + 1];
				int num3 = triangles[k + 2];
				array5[k] = num;
				array5[k + 1] = num2;
				array5[k + 2] = num3;
				array5[k + triangles.Length] = num3 + vertices.Length;
				array5[k + triangles.Length + 1] = num2 + vertices.Length;
				array5[k + triangles.Length + 2] = num + vertices.Length;
			}
			for (int l = 0; l < subMeshes.Count; l++)
			{
				for (int m = 0; m < subMeshes[l].Length; m += 3)
				{
					int num4 = subMeshes[l][m];
					int num5 = subMeshes[l][m + 1];
					int num6 = subMeshes[l][m + 2];
					list[l][m] = num4;
					list[l][m + 1] = num5;
					list[l][m + 2] = num6;
					list[l][m + subMeshes[l].Length] = num6 + vertices.Length;
					list[l][m + subMeshes[l].Length + 1] = num5 + vertices.Length;
					list[l][m + subMeshes[l].Length + 2] = num4 + vertices.Length;
				}
			}
			input.vertices = array;
			input.normals = array2;
			input.uv = array3;
			input.colors = array4;
			input.triangles = array5;
			input.subMeshes = list;
		}

		public static void MakeDoublesidedHalf(TS_Mesh input)
		{
			int num = input.vertices.Length / 2;
			int num2 = input.triangles.Length / 2;
			for (int i = 0; i < num; i++)
			{
				input.vertices[i + num] = input.vertices[i];
				input.normals[i + num] = -input.normals[i];
				input.uv[i + num] = input.uv[i];
				input.colors[i + num] = input.colors[i];
			}
			for (int j = 0; j < num2; j += 3)
			{
				input.triangles[j + num2 + 2] = input.triangles[j] + num;
				input.triangles[j + num2 + 1] = input.triangles[j + 1] + num;
				input.triangles[j + num2] = input.triangles[j + 2] + num;
			}
			for (int k = 0; k < input.subMeshes.Count; k++)
			{
				num2 = input.subMeshes[k].Length / 2;
				for (int l = 0; l < input.subMeshes[k].Length; l += 3)
				{
					input.subMeshes[k][l + num2 + 2] = input.subMeshes[k][l] + num;
					input.subMeshes[k][l + num2 + 1] = input.subMeshes[k][l + 1] + num;
					input.subMeshes[k][l + num2] = input.subMeshes[k][l + 2] + num;
				}
			}
		}

		public static void InverseTransformMesh(TS_Mesh input, TS_Transform transform)
		{
			if (input.vertices != null && input.normals != null)
			{
				for (int i = 0; i < input.vertices.Length; i++)
				{
					input.vertices[i] = transform.InverseTransformPoint(input.vertices[i]);
					input.normals[i] = transform.InverseTransformDirection(input.normals[i]);
				}
			}
		}

		public static void TransformMesh(TS_Mesh input, TS_Transform transform)
		{
			if (input.vertices != null && input.normals != null)
			{
				for (int i = 0; i < input.vertices.Length; i++)
				{
					input.vertices[i] = transform.TransformPoint(input.vertices[i]);
					input.normals[i] = transform.TransformDirection(input.normals[i]);
				}
			}
		}

		public static void InverseTransformMesh(TS_Mesh input, Transform transform)
		{
			if (input.vertices != null && input.normals != null)
			{
				for (int i = 0; i < input.vertices.Length; i++)
				{
					input.vertices[i] = transform.InverseTransformPoint(input.vertices[i]);
					input.normals[i] = transform.InverseTransformDirection(input.normals[i]);
				}
			}
		}

		public static void TransformMesh(TS_Mesh input, Transform transform)
		{
			if (input.vertices != null && input.normals != null)
			{
				for (int i = 0; i < input.vertices.Length; i++)
				{
					input.vertices[i] = transform.TransformPoint(input.vertices[i]);
					input.normals[i] = transform.TransformDirection(input.normals[i]);
				}
			}
		}

		public static void InverseTransformMesh(Mesh input, Transform transform)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] vertices2 = input.vertices;
			Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = worldToLocalMatrix.MultiplyPoint3x4(vertices[i]);
				vertices2[i] = worldToLocalMatrix.MultiplyVector(vertices2[i]);
			}
			input.vertices = vertices;
			input.normals = vertices2;
		}

		public static void TransformMesh(Mesh input, Transform transform)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] vertices2 = input.vertices;
			Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
			if (input.vertices != null && input.normals != null)
			{
				for (int i = 0; i < input.vertices.Length; i++)
				{
					vertices[i] = localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
					vertices2[i] = localToWorldMatrix.MultiplyVector(vertices2[i]);
				}
				input.vertices = vertices;
				input.normals = vertices2;
			}
		}

		public static void TransformVertices(Vector3[] vertices, Transform transform)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = transform.TransformPoint(vertices[i]);
			}
		}

		public static void InverseTransformVertices(Vector3[] vertices, Transform transform)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = transform.InverseTransformPoint(vertices[i]);
			}
		}

		public static void TransformNormals(Vector3[] normals, Transform transform)
		{
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = transform.TransformDirection(normals[i]);
			}
		}

		public static void InverseTransformNormals(Vector3[] normals, Transform transform)
		{
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = transform.InverseTransformDirection(normals[i]);
			}
		}

		public static string ToOBJString(Mesh mesh, Material[] materials)
		{
			int num = 0;
			if (mesh == null)
			{
				return "####Error####";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("g " + mesh.name + "\n");
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = vertices[i];
				num++;
				stringBuilder.Append($"v {0f - vector.x} {vector.y} {vector.z}\n");
			}
			stringBuilder.Append("\n");
			Vector3[] normals = mesh.normals;
			for (int j = 0; j < normals.Length; j++)
			{
				Vector3 vector2 = normals[j];
				stringBuilder.Append($"vn {0f - vector2.x} {vector2.y} {vector2.z}\n");
			}
			stringBuilder.Append("\n");
			Vector2[] uv = mesh.uv;
			for (int k = 0; k < uv.Length; k++)
			{
				Vector3 vector3 = uv[k];
				stringBuilder.Append($"vt {vector3.x} {vector3.y}\n");
			}
			stringBuilder.Append("\n");
			Vector2[] uv2 = mesh.uv2;
			for (int l = 0; l < uv2.Length; l++)
			{
				Vector2 vector4 = uv2[l];
				stringBuilder.Append($"vt2 {vector4.x} {vector4.y}\n");
			}
			stringBuilder.Append("\n");
			Vector2[] uv3 = mesh.uv3;
			for (int m = 0; m < uv3.Length; m++)
			{
				Vector2 vector5 = uv3[m];
				stringBuilder.Append($"vt2 {vector5.x} {vector5.y}\n");
			}
			stringBuilder.Append("\n");
			Color[] colors = mesh.colors;
			for (int n = 0; n < colors.Length; n++)
			{
				Color color = colors[n];
				stringBuilder.Append($"vc {color.r} {color.g} {color.b} {color.a}\n");
			}
			for (int num2 = 0; num2 < mesh.subMeshCount; num2++)
			{
				stringBuilder.Append("\n");
				stringBuilder.Append("usemtl ").Append(materials[num2].name).Append("\n");
				stringBuilder.Append("usemap ").Append(materials[num2].name).Append("\n");
				int[] triangles = mesh.GetTriangles(num2);
				for (int num3 = 0; num3 < triangles.Length; num3 += 3)
				{
					stringBuilder.Append(string.Format("f {2}/{2}/{2} {1}/{1}/{1} {0}/{0}/{0}\n", triangles[num3] + 1, triangles[num3 + 1] + 1, triangles[num3 + 2] + 1));
				}
			}
			return stringBuilder.ToString();
		}

		public static Mesh Copy(Mesh input)
		{
			Mesh mesh = new Mesh();
			mesh.name = input.name;
			mesh.vertices = input.vertices;
			mesh.normals = input.normals;
			mesh.colors = input.colors;
			mesh.uv = input.uv;
			mesh.uv2 = input.uv2;
			mesh.uv3 = input.uv3;
			mesh.uv4 = input.uv4;
			mesh.tangents = input.tangents;
			mesh.triangles = input.triangles;
			mesh.subMeshCount = input.subMeshCount;
			for (int i = 0; i < input.subMeshCount; i++)
			{
				mesh.SetTriangles(input.GetTriangles(i), i);
			}
			return mesh;
		}

		public static void Triangulate(Vector2[] points, ref int[] output)
		{
			List<int> list = new List<int>();
			int num = points.Length;
			if (num < 3)
			{
				output = new int[0];
				return;
			}
			int[] array = new int[num];
			if (Area(points, num) > 0f)
			{
				for (int i = 0; i < num; i++)
				{
					array[i] = i;
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					array[j] = num - 1 - j;
				}
			}
			int num2 = num;
			int num3 = 2 * num2;
			int num4 = 0;
			int num5 = num2 - 1;
			while (num2 > 2)
			{
				if (num3-- <= 0)
				{
					if (output.Length != list.Count)
					{
						output = new int[list.Count];
					}
					list.CopyTo(output, 0);
					return;
				}
				int num7 = num5;
				if (num2 <= num7)
				{
					num7 = 0;
				}
				num5 = num7 + 1;
				if (num2 <= num5)
				{
					num5 = 0;
				}
				int num8 = num5 + 1;
				if (num2 <= num8)
				{
					num8 = 0;
				}
				if (Snip(points, num7, num5, num8, num2, array))
				{
					int item = array[num7];
					int item2 = array[num5];
					int item3 = array[num8];
					list.Add(item3);
					list.Add(item2);
					list.Add(item);
					num4++;
					int num9 = num5;
					for (int k = num5 + 1; k < num2; k++)
					{
						array[num9] = array[k];
						num9++;
					}
					num2--;
					num3 = 2 * num2;
				}
			}
			list.Reverse();
			if (output.Length != list.Count)
			{
				output = new int[list.Count];
			}
			list.CopyTo(output, 0);
		}

		public static void FlipTriangles(ref int[] triangles)
		{
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num = triangles[i];
				triangles[i] = triangles[i + 2];
				triangles[i + 2] = num;
			}
		}

		public static void FlipFaces(TS_Mesh input)
		{
			for (int i = 0; i < input.subMeshes.Count; i++)
			{
				int[] triangles = input.subMeshes[i];
				FlipTriangles(ref triangles);
			}
			FlipTriangles(ref input.triangles);
			for (int j = 0; j < input.normals.Length; j++)
			{
				input.normals[j] *= -1f;
			}
		}

		public static void BreakMesh(Mesh input, bool keepNormals = true)
		{
			Vector3[] array = new Vector3[input.triangles.Length];
			Vector3[] array2 = new Vector3[array.Length];
			Vector2[] array3 = new Vector2[array.Length];
			Vector4[] array4 = new Vector4[array.Length];
			Color[] array5 = new Color[array.Length];
			Vector3[] vertices = input.vertices;
			Vector2[] uv = input.uv;
			Vector3[] normals = input.normals;
			Vector4[] tangents = input.tangents;
			Color[] array6 = input.colors;
			if (array6.Length != vertices.Length)
			{
				array6 = new Color[vertices.Length];
				for (int i = 0; i < array6.Length; i++)
				{
					array6[i] = Color.white;
				}
			}
			List<int[]> list = new List<int[]>();
			int subMeshCount = input.subMeshCount;
			int num = 0;
			for (int j = 0; j < subMeshCount; j++)
			{
				int[] triangles = input.GetTriangles(j);
				for (int k = 0; k < triangles.Length; k += 3)
				{
					array[num] = vertices[triangles[k]];
					array[num + 1] = vertices[triangles[k + 1]];
					array[num + 2] = vertices[triangles[k + 2]];
					if (normals.Length > triangles[k + 2])
					{
						if (!keepNormals)
						{
							array2[num] = (array2[num + 1] = (array2[num + 2] = (normals[triangles[k]] + normals[triangles[k + 1]] + normals[triangles[k + 2]]).normalized));
						}
						else
						{
							array2[num] = normals[triangles[k]];
							array2[num + 1] = normals[triangles[k + 1]];
							array2[num + 2] = normals[triangles[k + 2]];
						}
					}
					if (array6.Length > triangles[k + 2])
					{
						array5[num] = (array5[num + 1] = (array5[num + 2] = (array6[triangles[k]] + array6[triangles[k + 1]] + array6[triangles[k + 2]]) / 3f));
					}
					if (uv.Length > triangles[k + 2])
					{
						array3[num] = uv[triangles[k]];
						array3[num + 1] = uv[triangles[k + 1]];
						array3[num + 2] = uv[triangles[k + 2]];
					}
					if (tangents.Length > triangles[k + 2])
					{
						array4[num] = tangents[triangles[k]];
						array4[num + 1] = tangents[triangles[k + 1]];
						array4[num + 2] = tangents[triangles[k + 2]];
					}
					triangles[k] = num;
					triangles[k + 1] = num + 1;
					triangles[k + 2] = num + 2;
					num += 3;
				}
				list.Add(triangles);
			}
			input.vertices = array;
			input.normals = array2;
			input.colors = array5;
			input.uv = array3;
			input.tangents = array4;
			input.subMeshCount = list.Count;
			for (int l = 0; l < list.Count; l++)
			{
				input.SetTriangles(list[l], l);
			}
		}

		private static float Area(Vector2[] points, int maxCount)
		{
			float num = 0f;
			int num2 = maxCount - 1;
			int num3 = 0;
			while (num3 < maxCount)
			{
				Vector2 vector = points[num2];
				Vector2 vector2 = points[num3];
				num += vector.x * vector2.y - vector2.x * vector.y;
				num2 = num3++;
			}
			return num * 0.5f;
		}

		private static bool Snip(Vector2[] points, int u, int v, int w, int n, int[] V)
		{
			Vector2 a = points[V[u]];
			Vector2 b = points[V[v]];
			Vector2 c = points[V[w]];
			if (Mathf.Epsilon > (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x))
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (i != u && i != v && i != w)
				{
					Vector2 p = points[V[i]];
					if (InsideTriangle(a, b, c, p))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
		{
			float num = C.x - B.x;
			float num2 = C.y - B.y;
			float num3 = A.x - C.x;
			float num4 = A.y - C.y;
			float num5 = B.x - A.x;
			float num6 = B.y - A.y;
			float num7 = P.x - A.x;
			float num8 = P.y - A.y;
			float num9 = P.x - B.x;
			float num10 = P.y - B.y;
			float num11 = P.x - C.x;
			float num12 = P.y - C.y;
			float num13 = num * num10 - num2 * num9;
			float num14 = num5 * num8 - num6 * num7;
			float num15 = num3 * num12 - num4 * num11;
			return num13 >= 0f && num15 >= 0f && num14 >= 0f;
		}
	}
}
