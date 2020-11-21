using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Spline Mesh")]
	public class SplineMesh : MeshGenerator
	{
		[Serializable]
		public class Channel
		{
			public enum Type
			{
				Extrude,
				Place
			}

			public enum UVOverride
			{
				None,
				ClampU,
				ClampV,
				UniformU,
				UniformV
			}

			[Serializable]
			public class MeshDefinition
			{
				public enum MirrorMethod
				{
					None,
					X,
					Y,
					Z
				}

				[Serializable]
				public class Submesh
				{
					public int[] triangles = new int[0];

					public Submesh()
					{
					}

					public Submesh(int[] input)
					{
						triangles = new int[input.Length];
						input.CopyTo(triangles, 0);
					}
				}

				[Serializable]
				public class VertexGroup
				{
					public float value;

					public double percent;

					public int[] ids;

					public VertexGroup(float val, double perc, int[] vertIds)
					{
						percent = perc;
						value = val;
						ids = vertIds;
					}

					public void AddId(int id)
					{
						int[] array = new int[ids.Length + 1];
						ids.CopyTo(array, 0);
						array[array.Length - 1] = id;
						ids = array;
					}
				}

				[SerializeField]
				[HideInInspector]
				internal Vector3[] vertices = new Vector3[0];

				[SerializeField]
				[HideInInspector]
				internal Vector3[] normals = new Vector3[0];

				[SerializeField]
				[HideInInspector]
				internal Vector4[] tangents = new Vector4[0];

				[SerializeField]
				[HideInInspector]
				internal Color[] colors = new Color[0];

				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv = new Vector2[0];

				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv2 = new Vector2[0];

				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv3 = new Vector2[0];

				[SerializeField]
				[HideInInspector]
				internal Vector2[] uv4 = new Vector2[0];

				[SerializeField]
				[HideInInspector]
				internal int[] triangles = new int[0];

				[SerializeField]
				[HideInInspector]
				internal List<Submesh> subMeshes = new List<Submesh>();

				[SerializeField]
				[HideInInspector]
				internal TS_Bounds bounds = new TS_Bounds(Vector3.zero, Vector3.zero);

				[SerializeField]
				[HideInInspector]
				internal List<VertexGroup> vertexGroups = new List<VertexGroup>();

				[SerializeField]
				[HideInInspector]
				private Mesh _mesh;

				[SerializeField]
				[HideInInspector]
				private Vector3 _rotation = Vector3.zero;

				[SerializeField]
				[HideInInspector]
				private Vector2 _offset = Vector2.zero;

				[SerializeField]
				[HideInInspector]
				private Vector3 _scale = Vector3.one;

				[SerializeField]
				[HideInInspector]
				private Vector2 _uvScale = Vector2.one;

				[SerializeField]
				[HideInInspector]
				private Vector2 _uvOffset = Vector2.zero;

				[SerializeField]
				[HideInInspector]
				private float _uvRotation;

				[SerializeField]
				[HideInInspector]
				private MirrorMethod _mirror;

				[SerializeField]
				[HideInInspector]
				private bool _flipFaces;

				[SerializeField]
				[HideInInspector]
				private bool _doubleSided;

				public Mesh mesh
				{
					get
					{
						return _mesh;
					}
					set
					{
						if (_mesh != value)
						{
							_mesh = value;
							Refresh();
						}
					}
				}

				public Vector3 rotation
				{
					get
					{
						return _rotation;
					}
					set
					{
						if (rotation != value)
						{
							_rotation = value;
							Refresh();
						}
					}
				}

				public Vector2 offset
				{
					get
					{
						return _offset;
					}
					set
					{
						if (_offset != value)
						{
							_offset = value;
							Refresh();
						}
					}
				}

				public Vector3 scale
				{
					get
					{
						return _scale;
					}
					set
					{
						if (_scale != value)
						{
							_scale = value;
							Refresh();
						}
					}
				}

				public Vector2 uvScale
				{
					get
					{
						return _uvScale;
					}
					set
					{
						if (_uvScale != value)
						{
							_uvScale = value;
							Refresh();
						}
					}
				}

				public Vector2 uvOffset
				{
					get
					{
						return _uvOffset;
					}
					set
					{
						if (_uvOffset != value)
						{
							_uvOffset = value;
							Refresh();
						}
					}
				}

				public float uvRotation
				{
					get
					{
						return _uvRotation;
					}
					set
					{
						if (_uvRotation != value)
						{
							_uvRotation = value;
							Refresh();
						}
					}
				}

				public MirrorMethod mirror
				{
					get
					{
						return _mirror;
					}
					set
					{
						if (_mirror != value)
						{
							_mirror = value;
							Refresh();
						}
					}
				}

				public bool flipFaces
				{
					get
					{
						return _flipFaces;
					}
					set
					{
						if (_flipFaces != value)
						{
							_flipFaces = value;
							Refresh();
						}
					}
				}

				public bool doubleSided
				{
					get
					{
						return _doubleSided;
					}
					set
					{
						if (_doubleSided != value)
						{
							_doubleSided = value;
							Refresh();
						}
					}
				}

				public MeshDefinition(Mesh input)
				{
					_mesh = input;
					Refresh();
				}

				internal MeshDefinition Copy()
				{
					MeshDefinition meshDefinition = new MeshDefinition(_mesh);
					meshDefinition.vertices = new Vector3[vertices.Length];
					meshDefinition.normals = new Vector3[normals.Length];
					meshDefinition.colors = new Color[colors.Length];
					meshDefinition.tangents = new Vector4[tangents.Length];
					meshDefinition.uv = new Vector2[uv.Length];
					meshDefinition.uv2 = new Vector2[uv2.Length];
					meshDefinition.uv3 = new Vector2[uv3.Length];
					meshDefinition.uv4 = new Vector2[uv4.Length];
					meshDefinition.triangles = new int[triangles.Length];
					vertices.CopyTo(meshDefinition.vertices, 0);
					normals.CopyTo(meshDefinition.normals, 0);
					colors.CopyTo(meshDefinition.colors, 0);
					tangents.CopyTo(meshDefinition.tangents, 0);
					uv.CopyTo(meshDefinition.uv, 0);
					uv2.CopyTo(meshDefinition.uv2, 0);
					uv3.CopyTo(meshDefinition.uv3, 0);
					uv4.CopyTo(meshDefinition.uv4, 0);
					triangles.CopyTo(meshDefinition.triangles, 0);
					meshDefinition.bounds = new TS_Bounds(bounds.min, bounds.max);
					meshDefinition.subMeshes = new List<Submesh>();
					for (int i = 0; i < subMeshes.Count; i++)
					{
						meshDefinition.subMeshes.Add(new Submesh(new int[subMeshes[i].triangles.Length]));
						subMeshes[i].triangles.CopyTo(meshDefinition.subMeshes[meshDefinition.subMeshes.Count - 1].triangles, 0);
					}
					meshDefinition._mirror = _mirror;
					meshDefinition._offset = _offset;
					meshDefinition._rotation = _rotation;
					meshDefinition._scale = _scale;
					meshDefinition._uvOffset = _uvOffset;
					meshDefinition._uvScale = _uvScale;
					meshDefinition._uvRotation = _uvRotation;
					meshDefinition._flipFaces = _flipFaces;
					meshDefinition._doubleSided = _doubleSided;
					return meshDefinition;
				}

				public void Refresh()
				{
					if (_mesh == null)
					{
						vertices = new Vector3[0];
						normals = new Vector3[0];
						colors = new Color[0];
						uv = new Vector2[0];
						uv2 = new Vector2[0];
						uv3 = new Vector2[0];
						uv4 = new Vector2[0];
						tangents = new Vector4[0];
						triangles = new int[0];
						subMeshes = new List<Submesh>();
						vertexGroups = new List<VertexGroup>();
						return;
					}
					if (vertices.Length != _mesh.vertexCount)
					{
						vertices = new Vector3[_mesh.vertexCount];
					}
					if (normals.Length != _mesh.normals.Length)
					{
						normals = new Vector3[_mesh.normals.Length];
					}
					if (colors.Length != _mesh.colors.Length)
					{
						colors = new Color[_mesh.colors.Length];
					}
					if (uv.Length != _mesh.uv.Length)
					{
						uv = new Vector2[_mesh.uv.Length];
					}
					if (uv2.Length != _mesh.uv2.Length)
					{
						uv2 = new Vector2[_mesh.uv2.Length];
					}
					if (uv3.Length != _mesh.uv3.Length)
					{
						uv3 = new Vector2[_mesh.uv3.Length];
					}
					if (uv4.Length != _mesh.uv4.Length)
					{
						uv4 = new Vector2[_mesh.uv4.Length];
					}
					if (tangents.Length != _mesh.tangents.Length)
					{
						tangents = new Vector4[_mesh.tangents.Length];
					}
					if (triangles.Length != _mesh.triangles.Length)
					{
						triangles = new int[_mesh.triangles.Length];
					}
					vertices = _mesh.vertices;
					normals = _mesh.normals;
					colors = _mesh.colors;
					uv = _mesh.uv;
					uv2 = _mesh.uv2;
					uv3 = _mesh.uv3;
					uv2 = _mesh.uv4;
					tangents = _mesh.tangents;
					triangles = _mesh.triangles;
					colors = _mesh.colors;
					while (subMeshes.Count > _mesh.subMeshCount)
					{
						subMeshes.RemoveAt(0);
					}
					while (subMeshes.Count < _mesh.subMeshCount)
					{
						subMeshes.Add(new Submesh(new int[0]));
					}
					for (int i = 0; i < subMeshes.Count; i++)
					{
						subMeshes[i].triangles = _mesh.GetTriangles(i);
					}
					if (colors.Length != vertices.Length)
					{
						colors = new Color[vertices.Length];
						for (int j = 0; j < colors.Length; j++)
						{
							colors[j] = Color.white;
						}
					}
					Mirror();
					if (_doubleSided)
					{
						DoubleSided();
					}
					else if (_flipFaces)
					{
						FlipFaces();
					}
					TransformVertices();
					CalculateBounds();
					GroupVertices();
				}

				private void FlipFaces()
				{
					TS_Mesh tS_Mesh = new TS_Mesh();
					tS_Mesh.normals = normals;
					tS_Mesh.tangents = tangents;
					tS_Mesh.triangles = triangles;
					for (int i = 0; i < subMeshes.Count; i++)
					{
						tS_Mesh.subMeshes.Add(subMeshes[i].triangles);
					}
					MeshUtility.FlipFaces(tS_Mesh);
					tS_Mesh = null;
				}

				private void DoubleSided()
				{
					TS_Mesh tS_Mesh = new TS_Mesh();
					tS_Mesh.vertices = vertices;
					tS_Mesh.normals = normals;
					tS_Mesh.tangents = tangents;
					tS_Mesh.colors = colors;
					tS_Mesh.uv = uv;
					tS_Mesh.uv2 = uv2;
					tS_Mesh.uv3 = uv3;
					tS_Mesh.uv4 = uv4;
					tS_Mesh.triangles = triangles;
					for (int i = 0; i < subMeshes.Count; i++)
					{
						tS_Mesh.subMeshes.Add(subMeshes[i].triangles);
					}
					MeshUtility.MakeDoublesided(tS_Mesh);
					vertices = tS_Mesh.vertices;
					normals = tS_Mesh.normals;
					tangents = tS_Mesh.tangents;
					colors = tS_Mesh.colors;
					uv = tS_Mesh.uv;
					uv2 = tS_Mesh.uv2;
					uv3 = tS_Mesh.uv3;
					uv4 = tS_Mesh.uv4;
					triangles = tS_Mesh.triangles;
					for (int j = 0; j < subMeshes.Count; j++)
					{
						subMeshes[j].triangles = tS_Mesh.subMeshes[j];
					}
				}

				public void Write(TS_Mesh target, int forceMaterialId = -1)
				{
					if (target.vertices.Length != vertices.Length)
					{
						target.vertices = new Vector3[vertices.Length];
					}
					if (target.normals.Length != normals.Length)
					{
						target.normals = new Vector3[normals.Length];
					}
					if (target.colors.Length != colors.Length)
					{
						target.colors = new Color[colors.Length];
					}
					if (target.uv.Length != uv.Length)
					{
						target.uv = new Vector2[uv.Length];
					}
					if (target.uv2.Length != uv2.Length)
					{
						target.uv2 = new Vector2[uv2.Length];
					}
					if (target.uv3.Length != uv3.Length)
					{
						target.uv3 = new Vector2[uv3.Length];
					}
					if (target.uv4.Length != uv4.Length)
					{
						target.uv4 = new Vector2[uv4.Length];
					}
					if (target.tangents.Length != tangents.Length)
					{
						target.tangents = new Vector4[tangents.Length];
					}
					if (target.triangles.Length != triangles.Length)
					{
						target.triangles = new int[triangles.Length];
					}
					vertices.CopyTo(target.vertices, 0);
					normals.CopyTo(target.normals, 0);
					colors.CopyTo(target.colors, 0);
					uv.CopyTo(target.uv, 0);
					uv2.CopyTo(target.uv2, 0);
					uv3.CopyTo(target.uv3, 0);
					uv4.CopyTo(target.uv4, 0);
					tangents.CopyTo(target.tangents, 0);
					triangles.CopyTo(target.triangles, 0);
					if (target.subMeshes == null)
					{
						target.subMeshes = new List<int[]>();
					}
					if (forceMaterialId >= 0)
					{
						while (target.subMeshes.Count > forceMaterialId + 1)
						{
							target.subMeshes.RemoveAt(0);
						}
						while (target.subMeshes.Count < forceMaterialId + 1)
						{
							target.subMeshes.Add(new int[0]);
						}
						for (int i = 0; i < target.subMeshes.Count; i++)
						{
							if (i != forceMaterialId)
							{
								if (target.subMeshes[i].Length > 0)
								{
									target.subMeshes[i] = new int[0];
								}
								continue;
							}
							if (target.subMeshes[i].Length != triangles.Length)
							{
								target.subMeshes[i] = new int[triangles.Length];
							}
							triangles.CopyTo(target.subMeshes[i], 0);
						}
						return;
					}
					while (target.subMeshes.Count > subMeshes.Count)
					{
						target.subMeshes.RemoveAt(0);
					}
					while (target.subMeshes.Count < subMeshes.Count)
					{
						target.subMeshes.Add(new int[0]);
					}
					for (int j = 0; j < subMeshes.Count; j++)
					{
						if (subMeshes[j].triangles.Length != target.subMeshes[j].Length)
						{
							target.subMeshes[j] = new int[subMeshes[j].triangles.Length];
						}
						subMeshes[j].triangles.CopyTo(target.subMeshes[j], 0);
					}
				}

				private void CalculateBounds()
				{
					Vector3 zero = Vector3.zero;
					Vector3 zero2 = Vector3.zero;
					for (int i = 0; i < vertices.Length; i++)
					{
						if (vertices[i].x < zero.x)
						{
							zero.x = vertices[i].x;
						}
						else if (vertices[i].x > zero2.x)
						{
							zero2.x = vertices[i].x;
						}
						if (vertices[i].y < zero.y)
						{
							zero.y = vertices[i].y;
						}
						else if (vertices[i].y > zero2.y)
						{
							zero2.y = vertices[i].y;
						}
						if (vertices[i].z < zero.z)
						{
							zero.z = vertices[i].z;
						}
						else if (vertices[i].z > zero2.z)
						{
							zero2.z = vertices[i].z;
						}
					}
					bounds.CreateFromMinMax(zero, zero2);
				}

				private void Mirror()
				{
					if (_mirror == MirrorMethod.None)
					{
						return;
					}
					switch (_mirror)
					{
					case MirrorMethod.X:
						for (int j = 0; j < vertices.Length; j++)
						{
							vertices[j].x *= -1f;
							normals[j].x = 0f - normals[j].x;
						}
						break;
					case MirrorMethod.Y:
						for (int k = 0; k < vertices.Length; k++)
						{
							vertices[k].y *= -1f;
							normals[k].y = 0f - normals[k].y;
						}
						break;
					case MirrorMethod.Z:
						for (int i = 0; i < vertices.Length; i++)
						{
							vertices[i].z *= -1f;
							normals[i].z = 0f - normals[i].z;
						}
						break;
					}
					for (int l = 0; l < triangles.Length; l += 3)
					{
						int num = triangles[l];
						triangles[l] = triangles[l + 2];
						triangles[l + 2] = num;
					}
					for (int m = 0; m < subMeshes.Count; m++)
					{
						for (int n = 0; n < subMeshes[m].triangles.Length; n += 3)
						{
							int num2 = subMeshes[m].triangles[n];
							subMeshes[m].triangles[n] = subMeshes[m].triangles[n + 2];
							subMeshes[m].triangles[n + 2] = num2;
						}
					}
					CalculateTangents();
				}

				private void TransformVertices()
				{
					Matrix4x4 matrix4x = default(Matrix4x4);
					matrix4x.SetTRS(_offset, Quaternion.Euler(_rotation), _scale);
					Matrix4x4 transpose = matrix4x.inverse.transpose;
					for (int i = 0; i < vertices.Length; i++)
					{
						vertices[i] = matrix4x.MultiplyPoint3x4(vertices[i]);
						normals[i] = transpose.MultiplyVector(normals[i]).normalized;
					}
					for (int j = 0; j < tangents.Length; j++)
					{
						tangents[j] = transpose.MultiplyVector(tangents[j]);
					}
					for (int k = 0; k < uv.Length; k++)
					{
						uv[k].x *= _uvScale.x;
						uv[k].y *= _uvScale.y;
						uv[k] += _uvOffset;
						uv[k] = Quaternion.AngleAxis(uvRotation, Vector3.forward) * uv[k];
					}
				}

				private void GroupVertices()
				{
					vertexGroups = new List<VertexGroup>();
					for (int i = 0; i < vertices.Length; i++)
					{
						float z = vertices[i].z;
						double perc = DMath.Clamp01(DMath.InverseLerp(bounds.min.z, bounds.max.z, z));
						int num = FindInsertIndex(vertices[i], z);
						if (num >= vertexGroups.Count)
						{
							vertexGroups.Add(new VertexGroup(z, perc, new int[1]
							{
								i
							}));
						}
						else if (Mathf.Approximately(vertexGroups[num].value, z))
						{
							vertexGroups[num].AddId(i);
						}
						else if (vertexGroups[num].value < z)
						{
							vertexGroups.Insert(num, new VertexGroup(z, perc, new int[1]
							{
								i
							}));
						}
						else if (num < vertexGroups.Count - 1)
						{
							vertexGroups.Insert(num + 1, new VertexGroup(z, perc, new int[1]
							{
								i
							}));
						}
						else
						{
							vertexGroups.Add(new VertexGroup(z, perc, new int[1]
							{
								i
							}));
						}
					}
				}

				private int FindInsertIndex(Vector3 pos, float value)
				{
					int num = 0;
					int num2 = vertexGroups.Count - 1;
					while (num <= num2)
					{
						int num3 = num + (num2 - num) / 2;
						if (vertexGroups[num3].value == value)
						{
							return num3;
						}
						if (vertexGroups[num3].value < value)
						{
							num2 = num3 - 1;
						}
						else
						{
							num = num3 + 1;
						}
					}
					return num;
				}

				private void CalculateTangents()
				{
					if (vertices.Length == 0)
					{
						tangents = new Vector4[0];
						return;
					}
					tangents = new Vector4[vertices.Length];
					Vector3[] array = new Vector3[vertices.Length];
					Vector3[] array2 = new Vector3[vertices.Length];
					for (int i = 0; i < subMeshes.Count; i++)
					{
						for (int j = 0; j < subMeshes[i].triangles.Length; j += 3)
						{
							int num = subMeshes[i].triangles[j];
							int num2 = subMeshes[i].triangles[j + 1];
							int num3 = subMeshes[i].triangles[j + 2];
							float num4 = vertices[num2].x - vertices[num].x;
							float num5 = vertices[num3].x - vertices[num].x;
							float num6 = vertices[num2].y - vertices[num].y;
							float num7 = vertices[num3].y - vertices[num].y;
							float num8 = vertices[num2].z - vertices[num].z;
							float num9 = vertices[num3].z - vertices[num].z;
							float num10 = uv[num2].x - uv[num].x;
							float num11 = uv[num3].x - uv[num].x;
							float num12 = uv[num2].y - uv[num].y;
							float num13 = uv[num3].y - uv[num].y;
							float num14 = num10 * num13 - num11 * num12;
							float num15 = (num14 != 0f) ? (1f / num14) : 0f;
							Vector3 vector = new Vector3((num13 * num4 - num12 * num5) * num15, (num13 * num6 - num12 * num7) * num15, (num13 * num8 - num12 * num9) * num15);
							Vector3 vector2 = new Vector3((num10 * num5 - num11 * num4) * num15, (num10 * num7 - num11 * num6) * num15, (num10 * num9 - num11 * num8) * num15);
							array[num] += vector;
							array[num2] += vector;
							array[num3] += vector;
							array2[num] += vector2;
							array2[num2] += vector2;
							array2[num3] += vector2;
						}
					}
					for (int k = 0; k < vertices.Length; k++)
					{
						Vector3 normal = normals[k];
						Vector3 tangent = array[k];
						Vector3.OrthoNormalize(ref normal, ref tangent);
						tangents[k].x = tangent.x;
						tangents[k].y = tangent.y;
						tangents[k].z = tangent.z;
						tangents[k].w = ((!(Vector3.Dot(Vector3.Cross(normal, tangent), array2[k]) < 0f)) ? 1f : (-1f));
					}
				}
			}

			public string name = "Channel";

			private System.Random iterationRandom;

			[SerializeField]
			[HideInInspector]
			private int _iterationSeed;

			[SerializeField]
			[HideInInspector]
			private int _offsetSeed;

			private System.Random offsetRandom;

			[SerializeField]
			[HideInInspector]
			private int _rotationSeed;

			private System.Random rotationRandom;

			[SerializeField]
			[HideInInspector]
			private int _scaleSeed;

			private System.Random scaleRandom;

			[SerializeField]
			internal SplineMesh owner;

			[SerializeField]
			[HideInInspector]
			private List<MeshDefinition> meshes = new List<MeshDefinition>();

			[SerializeField]
			[HideInInspector]
			private double _clipFrom;

			[SerializeField]
			[HideInInspector]
			private double _clipTo = 1.0;

			[SerializeField]
			[HideInInspector]
			private bool _randomOrder;

			[SerializeField]
			[HideInInspector]
			private UVOverride _overrideUVs;

			[SerializeField]
			[HideInInspector]
			private Vector2 _uvScale = Vector2.one;

			[SerializeField]
			[HideInInspector]
			private Vector2 _uvOffset = Vector2.zero;

			[SerializeField]
			[HideInInspector]
			private bool _overrideNormal;

			[SerializeField]
			[HideInInspector]
			private Vector3 _customNormal = Vector3.up;

			[SerializeField]
			[HideInInspector]
			private Type _type;

			[SerializeField]
			[HideInInspector]
			private int _count = 1;

			[SerializeField]
			[HideInInspector]
			private double _spacing;

			[SerializeField]
			[HideInInspector]
			private bool _randomRotation;

			[SerializeField]
			[HideInInspector]
			private Vector3 _minRotation = Vector3.zero;

			[SerializeField]
			[HideInInspector]
			private Vector3 _maxRotation = Vector3.zero;

			[SerializeField]
			[HideInInspector]
			private bool _randomOffset;

			[SerializeField]
			[HideInInspector]
			private Vector2 _minOffset = Vector2.one;

			[SerializeField]
			[HideInInspector]
			private Vector2 _maxOffset = Vector2.one;

			[SerializeField]
			[HideInInspector]
			private bool _randomScale;

			[SerializeField]
			[HideInInspector]
			private bool _uniformRandomScale;

			[SerializeField]
			[HideInInspector]
			private Vector3 _minScale = Vector3.one;

			[SerializeField]
			[HideInInspector]
			private Vector3 _maxScale = Vector3.one;

			private int iterator;

			[SerializeField]
			[HideInInspector]
			private bool _overrideMaterialID;

			[SerializeField]
			[HideInInspector]
			private int _targetMaterialID;

			public double clipFrom
			{
				get
				{
					return _clipFrom;
				}
				set
				{
					if (value != _clipFrom)
					{
						_clipFrom = value;
						Rebuild();
					}
				}
			}

			public double clipTo
			{
				get
				{
					return _clipTo;
				}
				set
				{
					if (value != _clipTo)
					{
						_clipTo = value;
						Rebuild();
					}
				}
			}

			public bool randomOffset
			{
				get
				{
					return _randomOffset;
				}
				set
				{
					if (value != _randomOffset)
					{
						_randomOffset = value;
						Rebuild();
					}
				}
			}

			public bool overrideMaterialID
			{
				get
				{
					return _overrideMaterialID;
				}
				set
				{
					if (value != _overrideMaterialID)
					{
						_overrideMaterialID = value;
						Rebuild();
					}
				}
			}

			public int targetMaterialID
			{
				get
				{
					return _targetMaterialID;
				}
				set
				{
					if (value != _targetMaterialID)
					{
						_targetMaterialID = value;
						Rebuild();
					}
				}
			}

			public bool randomRotation
			{
				get
				{
					return _randomRotation;
				}
				set
				{
					if (value != _randomRotation)
					{
						_randomRotation = value;
						Rebuild();
					}
				}
			}

			public bool randomScale
			{
				get
				{
					return _randomScale;
				}
				set
				{
					if (value != _randomScale)
					{
						_randomScale = value;
						Rebuild();
					}
				}
			}

			public bool uniformRandomScale
			{
				get
				{
					return _uniformRandomScale;
				}
				set
				{
					if (value != _uniformRandomScale)
					{
						_uniformRandomScale = value;
						Rebuild();
					}
				}
			}

			public int offsetSeed
			{
				get
				{
					return _offsetSeed;
				}
				set
				{
					if (value != _offsetSeed)
					{
						_offsetSeed = value;
						Rebuild();
					}
				}
			}

			public int rotationSeed
			{
				get
				{
					return _rotationSeed;
				}
				set
				{
					if (value != _rotationSeed)
					{
						_rotationSeed = value;
						Rebuild();
					}
				}
			}

			public int scaleSeed
			{
				get
				{
					return _scaleSeed;
				}
				set
				{
					if (value != _scaleSeed)
					{
						_scaleSeed = value;
						Rebuild();
					}
				}
			}

			public double spacing
			{
				get
				{
					return _spacing;
				}
				set
				{
					if (value != _spacing)
					{
						_spacing = value;
						Rebuild();
					}
				}
			}

			public Vector2 minOffset
			{
				get
				{
					return _minOffset;
				}
				set
				{
					if (value != _minOffset)
					{
						_minOffset = value;
						Rebuild();
					}
				}
			}

			public Vector2 maxOffset
			{
				get
				{
					return _maxOffset;
				}
				set
				{
					if (value != _maxOffset)
					{
						_maxOffset = value;
						Rebuild();
					}
				}
			}

			public Vector3 minRotation
			{
				get
				{
					return _minRotation;
				}
				set
				{
					if (value != _minRotation)
					{
						_minRotation = value;
						Rebuild();
					}
				}
			}

			public Vector3 maxRotation
			{
				get
				{
					return _maxRotation;
				}
				set
				{
					if (value != _maxRotation)
					{
						_maxRotation = value;
						Rebuild();
					}
				}
			}

			public Vector3 minScale
			{
				get
				{
					return _minScale;
				}
				set
				{
					if (value != _minScale)
					{
						_minScale = value;
						Rebuild();
					}
				}
			}

			public Vector3 maxScale
			{
				get
				{
					return _maxScale;
				}
				set
				{
					if (value != _maxScale)
					{
						_maxScale = value;
						Rebuild();
					}
				}
			}

			public Type type
			{
				get
				{
					return _type;
				}
				set
				{
					if (value != _type)
					{
						_type = value;
						Rebuild();
					}
				}
			}

			public bool randomOrder
			{
				get
				{
					return _randomOrder;
				}
				set
				{
					if (value != _randomOrder)
					{
						_randomOrder = value;
						Rebuild();
					}
				}
			}

			public int randomSeed
			{
				get
				{
					return _iterationSeed;
				}
				set
				{
					if (value != _iterationSeed)
					{
						_iterationSeed = value;
						if (_randomOrder)
						{
							Rebuild();
						}
					}
				}
			}

			public int count
			{
				get
				{
					return _count;
				}
				set
				{
					if (value != _count)
					{
						_count = value;
						if (_count < 1)
						{
							_count = 1;
						}
						Rebuild();
					}
				}
			}

			public UVOverride overrideUVs
			{
				get
				{
					return _overrideUVs;
				}
				set
				{
					if (value != _overrideUVs)
					{
						_overrideUVs = value;
						Rebuild();
					}
				}
			}

			public Vector2 uvOffset
			{
				get
				{
					return _uvOffset;
				}
				set
				{
					if (value != _uvOffset)
					{
						_uvOffset = value;
						Rebuild();
					}
				}
			}

			public Vector2 uvScale
			{
				get
				{
					return _uvScale;
				}
				set
				{
					if (value != _uvScale)
					{
						_uvScale = value;
						Rebuild();
					}
				}
			}

			public bool overrideNormal
			{
				get
				{
					return _overrideNormal;
				}
				set
				{
					if (value != _overrideNormal)
					{
						_overrideNormal = value;
						Rebuild();
					}
				}
			}

			public Vector3 customNormal
			{
				get
				{
					return _customNormal;
				}
				set
				{
					if (value != _customNormal)
					{
						_customNormal = value;
						Rebuild();
					}
				}
			}

			public Channel(string n, SplineMesh parent)
			{
				name = n;
				owner = parent;
				Init();
			}

			public Channel(string n, Mesh inputMesh, SplineMesh parent)
			{
				name = n;
				owner = parent;
				meshes.Add(new MeshDefinition(inputMesh));
				Init();
				Rebuild();
			}

			private void Init()
			{
				_minScale = (_maxScale = Vector3.one);
				_minOffset = (_maxOffset = Vector3.zero);
				_minRotation = (_maxRotation = Vector3.zero);
			}

			public void CopyTo(Channel target)
			{
				target.meshes.Clear();
				for (int i = 0; i < meshes.Count; i++)
				{
					target.meshes.Add(meshes[i].Copy());
				}
				target._clipFrom = _clipFrom;
				target._clipTo = _clipTo;
				target._customNormal = _customNormal;
				target._iterationSeed = _iterationSeed;
				target._minOffset = _minOffset;
				target._minRotation = _minRotation;
				target._minScale = _minScale;
				target._maxOffset = _maxOffset;
				target._maxRotation = _maxRotation;
				target._maxScale = _maxScale;
				target._randomOffset = _randomOffset;
				target._randomRotation = _randomRotation;
				target._randomScale = _randomScale;
				target._offsetSeed = _offsetSeed;
				target._rotationSeed = _rotationSeed;
				target._scaleSeed = _scaleSeed;
				target._iterationSeed = _iterationSeed;
				target._count = _count;
				target._spacing = _spacing;
				target._overrideUVs = _overrideUVs;
				target._type = _type;
				target._overrideMaterialID = _overrideMaterialID;
				target._targetMaterialID = _targetMaterialID;
				target._overrideNormal = _overrideNormal;
			}

			public int GetMeshCount()
			{
				return meshes.Count;
			}

			public void SwapMeshes(int a, int b)
			{
				if (a >= 0 && a < meshes.Count && b >= 0 && b < meshes.Count)
				{
					MeshDefinition value = meshes[b];
					meshes[b] = meshes[a];
					meshes[a] = value;
					Rebuild();
				}
			}

			public void DuplicateMesh(int index)
			{
				if (index >= 0 && index < meshes.Count)
				{
					meshes.Add(meshes[index].Copy());
					Rebuild();
				}
			}

			public MeshDefinition GetMesh(int index)
			{
				return meshes[index];
			}

			public void AddMesh(Mesh input)
			{
				meshes.Add(new MeshDefinition(input));
				Rebuild();
			}

			public void RemoveMesh(int index)
			{
				meshes.RemoveAt(index);
				Rebuild();
			}

			public void ResetIteration()
			{
				if (_randomOrder)
				{
					iterationRandom = new System.Random(_iterationSeed);
				}
				if (_randomOffset)
				{
					offsetRandom = new System.Random(_offsetSeed);
				}
				if (_randomRotation)
				{
					rotationRandom = new System.Random(_rotationSeed);
				}
				if (_randomScale)
				{
					scaleRandom = new System.Random(_scaleSeed);
				}
				iterator = 0;
			}

			public Vector2 NextOffset()
			{
				if (_randomOffset)
				{
					return new Vector2(Mathf.Lerp(_minOffset.x, _maxOffset.x, (float)offsetRandom.NextDouble()), Mathf.Lerp(_minOffset.y, _maxOffset.y, (float)offsetRandom.NextDouble()));
				}
				return _minOffset;
			}

			public Quaternion NextPlaceRotation()
			{
				if (_randomRotation)
				{
					return Quaternion.Euler(new Vector3(Mathf.Lerp(_minRotation.x, _maxRotation.x, (float)rotationRandom.NextDouble()), Mathf.Lerp(_minRotation.y, _maxRotation.y, (float)rotationRandom.NextDouble()), Mathf.Lerp(_minRotation.z, _maxRotation.z, (float)rotationRandom.NextDouble())));
				}
				return Quaternion.Euler(_minRotation);
			}

			public float NextExtrudeRotation()
			{
				if (_randomRotation)
				{
					return Mathf.Lerp(_minRotation.z, _maxRotation.z, (float)rotationRandom.NextDouble());
				}
				return _minRotation.z;
			}

			public Vector3 NextExtrudeScale()
			{
				if (_randomScale)
				{
					if (_uniformRandomScale)
					{
						return Vector3.Lerp(new Vector3(_minScale.x, _minScale.y, 1f), new Vector3(_maxScale.x, _maxScale.y, 1f), (float)scaleRandom.NextDouble());
					}
					return new Vector3(Mathf.Lerp(_minScale.x, _maxScale.x, (float)scaleRandom.NextDouble()), Mathf.Lerp(_minScale.y, _maxScale.y, (float)scaleRandom.NextDouble()), 1f);
				}
				return new Vector3(_minScale.x, _minScale.y, 1f);
			}

			public Vector3 NextPlaceScale()
			{
				if (_randomScale)
				{
					if (_uniformRandomScale)
					{
						return Vector3.Lerp(_minScale, _maxScale, (float)scaleRandom.NextDouble());
					}
					return new Vector3(Mathf.Lerp(_minScale.x, _maxScale.x, (float)scaleRandom.NextDouble()), Mathf.Lerp(_minScale.y, _maxScale.y, (float)scaleRandom.NextDouble()), Mathf.Lerp(_minScale.z, _maxScale.z, (float)scaleRandom.NextDouble()));
				}
				return _minScale;
			}

			public MeshDefinition NextMesh()
			{
				if (_randomOrder)
				{
					return meshes[iterationRandom.Next(meshes.Count)];
				}
				if (iterator >= meshes.Count)
				{
					iterator = 0;
				}
				return meshes[iterator++];
			}

			internal void Rebuild()
			{
				if (owner != null)
				{
					owner.Rebuild(sampleComputer: false);
				}
			}

			private void Refresh()
			{
				for (int i = 0; i < meshes.Count; i++)
				{
					meshes[i].Refresh();
				}
				Rebuild();
			}
		}

		[SerializeField]
		[HideInInspector]
		private List<Channel> channels = new List<Channel>();

		private SplineResult lastResult = new SplineResult();

		private bool useLastResult;

		private List<TS_Mesh> combineMeshes = new List<TS_Mesh>();

		private int meshCount;

		private Matrix4x4 vertexMatrix = default(Matrix4x4);

		private Matrix4x4 normalMatrix = default(Matrix4x4);

		protected override void Awake()
		{
			base.Awake();
			mesh.name = "Extruded Mesh";
		}

		protected override void Reset()
		{
			base.Reset();
			AddChannel("Channel 1");
		}

		public void RemoveChannel(int index)
		{
			channels.RemoveAt(index);
			Rebuild(sampleComputer: false);
		}

		public void SwapChannels(int a, int b)
		{
			if (a >= 0 && a < channels.Count && b >= 0 && b < channels.Count)
			{
				Channel value = channels[b];
				channels[b] = channels[a];
				channels[a] = value;
				Rebuild(sampleComputer: false);
			}
		}

		public Channel AddChannel(Mesh inputMesh, string name)
		{
			Channel channel = new Channel(name, inputMesh, this);
			channels.Add(channel);
			return channel;
		}

		public Channel AddChannel(string name)
		{
			Channel channel = new Channel(name, this);
			channels.Add(channel);
			return channel;
		}

		public int GetChannelCount()
		{
			return channels.Count;
		}

		public Channel GetChannel(int index)
		{
			return channels[index];
		}

		protected override void BuildMesh()
		{
			if (base.clippedSamples.Length != 0)
			{
				base.BuildMesh();
				Generate();
			}
		}

		private void Generate()
		{
			meshCount = 0;
			for (int i = 0; i < channels.Count; i++)
			{
				if (channels[i].GetMeshCount() != 0)
				{
					meshCount += channels[i].count;
				}
			}
			if (meshCount == 0)
			{
				tsMesh.Clear();
				return;
			}
			if (combineMeshes.Count < meshCount)
			{
				combineMeshes.AddRange(new TS_Mesh[meshCount - combineMeshes.Count]);
			}
			else if (combineMeshes.Count > meshCount)
			{
				combineMeshes.RemoveRange(combineMeshes.Count - 1 - (combineMeshes.Count - meshCount), combineMeshes.Count - meshCount);
			}
			int num = 0;
			for (int j = 0; j < channels.Count; j++)
			{
				if (channels[j].GetMeshCount() == 0)
				{
					continue;
				}
				channels[j].ResetIteration();
				useLastResult = false;
				double num2 = 1.0 / (double)channels[j].count;
				double num3 = num2 * channels[j].spacing * 0.5;
				switch (channels[j].type)
				{
				case Channel.Type.Extrude:
					for (int l = 0; l < channels[j].count; l++)
					{
						double from = DMath.Lerp(channels[j].clipFrom, channels[j].clipTo, (double)l * num2 + num3);
						double to = DMath.Lerp(channels[j].clipFrom, channels[j].clipTo, (double)l * num2 + num2 - num3);
						if (combineMeshes[num] == null)
						{
							combineMeshes[num] = new TS_Mesh();
						}
						Stretch(channels[j], combineMeshes[num], from, to);
						num++;
					}
					if (num3 == 0.0)
					{
						useLastResult = true;
					}
					break;
				case Channel.Type.Place:
					for (int k = 0; k < channels[j].count; k++)
					{
						if (combineMeshes[num] == null)
						{
							combineMeshes[num] = new TS_Mesh();
						}
						Place(channels[j], combineMeshes[num], DMath.Lerp(channels[j].clipFrom, channels[j].clipTo, (double)k / (double)Mathf.Max(channels[j].count - 1, 1)));
						num++;
					}
					break;
				}
			}
			if (tsMesh == null)
			{
				tsMesh = new TS_Mesh();
			}
			else
			{
				tsMesh.Clear();
			}
			tsMesh.Combine(combineMeshes);
		}

		private void Place(Channel channel, TS_Mesh target, double percent)
		{
			Channel.MeshDefinition meshDefinition = channel.NextMesh();
			if (target == null)
			{
				target = new TS_Mesh();
			}
			meshDefinition.Write(target, (!channel.overrideMaterialID) ? (-1) : channel.targetMaterialID);
			Vector2 vector = channel.NextOffset();
			SplineResult splineResult = Evaluate(UnclipPercent(percent));
			Vector3 normal = splineResult.normal;
			Vector3 right = splineResult.right;
			Vector3 direction = splineResult.direction;
			if (channel.overrideNormal)
			{
				splineResult.direction = Vector3.Cross(splineResult.right, channel.customNormal);
				splineResult.normal = channel.customNormal;
			}
			Quaternion identity = Quaternion.identity;
			ref Matrix4x4 reference = ref vertexMatrix;
			Vector3 position = splineResult.position;
			Vector3 a = right;
			Vector3 offset = base.offset;
			Vector3 a2 = position + a * (offset.x + vector.x) * splineResult.size;
			Vector3 a3 = normal;
			Vector3 offset2 = base.offset;
			Vector3 a4 = a2 + a3 * (offset2.y + vector.y) * splineResult.size;
			Vector3 a5 = direction;
			Vector3 offset3 = base.offset;
			reference.SetTRS(a4 + a5 * offset3.z, splineResult.rotation * channel.NextPlaceRotation() * Quaternion.AngleAxis(base.rotation, Vector3.forward), channel.NextPlaceScale() * splineResult.size);
			normalMatrix = vertexMatrix.inverse.transpose;
			for (int i = 0; i < target.vertexCount; i++)
			{
				target.vertices[i] = vertexMatrix.MultiplyPoint3x4(meshDefinition.vertices[i]);
				target.normals[i] = normalMatrix.MultiplyVector(meshDefinition.normals[i]);
			}
		}

		private void Stretch(Channel channel, TS_Mesh target, double from, double to)
		{
			Channel.MeshDefinition meshDefinition = channel.NextMesh();
			if (target == null)
			{
				target = new TS_Mesh();
			}
			meshDefinition.Write(target, (!channel.overrideMaterialID) ? (-1) : channel.targetMaterialID);
			Vector2 vector = channel.NextOffset();
			Vector3 a = channel.NextExtrudeScale();
			float num = channel.NextExtrudeRotation();
			SplineResult splineResult = new SplineResult();
			Vector2 vector2 = Vector2.zero;
			Vector3 vector3 = Vector3.zero;
			for (int i = 0; i < meshDefinition.vertexGroups.Count; i++)
			{
				if (useLastResult && i == meshDefinition.vertexGroups.Count)
				{
					splineResult = lastResult;
				}
				else
				{
					Evaluate(splineResult, UnclipPercent(DMath.Lerp(from, to, meshDefinition.vertexGroups[i].percent)));
				}
				Vector3 normal = splineResult.normal;
				Vector3 right = splineResult.right;
				Vector3 direction = splineResult.direction;
				if (channel.overrideNormal)
				{
					splineResult.direction = Vector3.Cross(splineResult.right, channel.customNormal);
					splineResult.normal = channel.customNormal;
				}
				ref Matrix4x4 reference = ref vertexMatrix;
				Vector3 position = splineResult.position;
				Vector3 a2 = right;
				Vector3 offset = base.offset;
				Vector3 a3 = position + a2 * (offset.x + vector.x) * splineResult.size;
				Vector3 a4 = normal;
				Vector3 offset2 = base.offset;
				Vector3 a5 = a3 + a4 * (offset2.y + vector.y) * splineResult.size;
				Vector3 a6 = direction;
				Vector3 offset3 = base.offset;
				reference.SetTRS(a5 + a6 * offset3.z, splineResult.rotation * Quaternion.AngleAxis(base.rotation + num, Vector3.forward), a * splineResult.size);
				normalMatrix = vertexMatrix.inverse.transpose;
				if (i == 0)
				{
					lastResult.CopyFrom(splineResult);
				}
				for (int j = 0; j < meshDefinition.vertexGroups[i].ids.Length; j++)
				{
					int num2 = meshDefinition.vertexGroups[i].ids[j];
					vector3 = meshDefinition.vertices[num2];
					vector3.z = 0f;
					target.vertices[num2] = vertexMatrix.MultiplyPoint3x4(vector3);
					vector3 = meshDefinition.normals[num2];
					target.normals[num2] = normalMatrix.MultiplyVector(vector3);
					target.colors[num2] = target.colors[num2] * splineResult.color;
					vector2 = target.uv[num2];
					switch (channel.overrideUVs)
					{
					case Channel.UVOverride.ClampU:
						vector2.x = (float)splineResult.percent;
						break;
					case Channel.UVOverride.ClampV:
						vector2.y = (float)splineResult.percent;
						break;
					case Channel.UVOverride.UniformU:
						vector2.x = CalculateLength(0.0, splineResult.percent);
						break;
					case Channel.UVOverride.UniformV:
						vector2.y = CalculateLength(0.0, splineResult.percent);
						break;
					}
					ref Vector2 reference2 = ref target.uv[num2];
					float x = vector2.x;
					Vector2 uvScale = base.uvScale;
					float num3 = x * uvScale.x;
					Vector2 uvScale2 = channel.uvScale;
					float x2 = num3 * uvScale2.x;
					float y = vector2.y;
					Vector2 uvScale3 = base.uvScale;
					float num4 = y * uvScale3.y;
					Vector2 uvScale4 = channel.uvScale;
					reference2 = new Vector2(x2, num4 * uvScale4.y);
					target.uv[num2] += base.uvOffset + channel.uvOffset;
				}
			}
		}
	}
}
