using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Extrude Mesh")]
	public class ExtrudeMesh : MeshGenerator
	{
		public enum Axis
		{
			X,
			Y,
			Z
		}

		public enum Iteration
		{
			Ordered,
			Random
		}

		public enum MirrorMethod
		{
			None,
			X,
			Y,
			Z
		}

		public enum TileUVs
		{
			None,
			U,
			V,
			UniformU,
			UniformV
		}

		[Serializable]
		internal class ExtrudableMesh
		{
			[Serializable]
			public class VertexGroup
			{
				public float value;

				public int[] ids;

				public VertexGroup(float val, int[] vertIds)
				{
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

			public Vector3[] vertices = new Vector3[0];

			public Vector3[] normals = new Vector3[0];

			public Vector4[] tangents = new Vector4[0];

			public Color[] colors = new Color[0];

			public Vector2[] uv = new Vector2[0];

			public List<Submesh> subMeshes = new List<Submesh>();

			public TS_Bounds bounds = new TS_Bounds(Vector3.zero, Vector3.zero);

			public List<VertexGroup> vertexGroups = new List<VertexGroup>();

			[SerializeField]
			private MirrorMethod _mirror;

			[SerializeField]
			private Axis _axis = Axis.Z;

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
						Mirror(_mirror);
						_mirror = value;
						Mirror(_mirror);
					}
				}
			}

			public ExtrudableMesh()
			{
				vertices = new Vector3[0];
				normals = new Vector3[0];
				tangents = new Vector4[0];
				colors = new Color[0];
				uv = new Vector2[0];
				subMeshes = new List<Submesh>();
				bounds = new TS_Bounds(Vector3.zero, Vector3.zero);
				vertexGroups = new List<VertexGroup>();
			}

			public ExtrudableMesh(Mesh inputMesh, Axis axis)
			{
				Update(inputMesh, axis);
			}

			public void Update(Mesh inputMesh, Axis axis)
			{
				vertices = inputMesh.vertices;
				normals = inputMesh.normals;
				tangents = inputMesh.tangents;
				colors = inputMesh.colors;
				if (colors.Length != vertices.Length)
				{
					colors = new Color[vertices.Length];
					for (int i = 0; i < colors.Length; i++)
					{
						colors[i] = Color.white;
					}
				}
				uv = inputMesh.uv;
				bounds = new TS_Bounds(inputMesh.bounds);
				subMeshes.Clear();
				for (int j = 0; j < inputMesh.subMeshCount; j++)
				{
					subMeshes.Add(new Submesh(inputMesh.GetTriangles(j)));
				}
				_axis = axis;
				Mirror(_mirror);
				GroupVertices(axis);
			}

			private void Mirror(MirrorMethod method)
			{
				switch (method)
				{
				case MirrorMethod.None:
					return;
				case MirrorMethod.X:
					for (int m = 0; m < vertices.Length; m++)
					{
						float num5 = Mathf.InverseLerp(bounds.min.x, bounds.max.x, vertices[m].x);
						vertices[m].x = Mathf.Lerp(bounds.min.x, bounds.max.x, 1f - num5);
						normals[m].x = 0f - normals[m].x;
					}
					if (_axis == Axis.X)
					{
						for (int n = 0; n < vertexGroups.Count; n++)
						{
							float num6 = Mathf.InverseLerp(bounds.min.x, bounds.max.x, vertexGroups[n].value);
							vertexGroups[n].value = Mathf.Lerp(bounds.min.x, bounds.max.x, 1f - num6);
						}
					}
					break;
				case MirrorMethod.Y:
					for (int k = 0; k < vertices.Length; k++)
					{
						float num3 = Mathf.InverseLerp(bounds.min.y, bounds.max.y, vertices[k].y);
						vertices[k].y = Mathf.Lerp(bounds.min.y, bounds.max.y, 1f - num3);
						normals[k].y = 0f - normals[k].y;
					}
					if (_axis == Axis.Y)
					{
						for (int l = 0; l < vertexGroups.Count; l++)
						{
							float num4 = Mathf.InverseLerp(bounds.min.y, bounds.max.y, vertexGroups[l].value);
							vertexGroups[l].value = Mathf.Lerp(bounds.min.y, bounds.max.y, 1f - num4);
						}
					}
					break;
				case MirrorMethod.Z:
					for (int i = 0; i < vertices.Length; i++)
					{
						float num = Mathf.InverseLerp(bounds.min.z, bounds.max.z, vertices[i].z);
						vertices[i].z = Mathf.Lerp(bounds.min.z, bounds.max.z, 1f - num);
						normals[i].z = 0f - normals[i].z;
					}
					if (_axis == Axis.Z)
					{
						for (int j = 0; j < vertexGroups.Count; j++)
						{
							float num2 = Mathf.InverseLerp(bounds.min.z, bounds.max.z, vertexGroups[j].value);
							vertexGroups[j].value = Mathf.Lerp(bounds.min.z, bounds.max.z, 1f - num2);
						}
					}
					break;
				}
				for (int num7 = 0; num7 < subMeshes.Count; num7++)
				{
					for (int num8 = 0; num8 < subMeshes[num7].triangles.Length; num8 += 3)
					{
						int num9 = subMeshes[num7].triangles[num8];
						subMeshes[num7].triangles[num8] = subMeshes[num7].triangles[num8 + 2];
						subMeshes[num7].triangles[num8 + 2] = num9;
					}
				}
				CalculateTangents();
			}

			private void GroupVertices(Axis axis)
			{
				vertexGroups = new List<VertexGroup>();
				int num = (int)axis;
				if (num > 2)
				{
					num -= 2;
				}
				for (int i = 0; i < vertices.Length; i++)
				{
					float num2 = 0f;
					switch (num)
					{
					case 0:
						num2 = vertices[i].x;
						break;
					case 1:
						num2 = vertices[i].y;
						break;
					case 2:
						num2 = vertices[i].z;
						break;
					}
					int num3 = FindInsertIndex(vertices[i], num2);
					if (num3 >= vertexGroups.Count)
					{
						vertexGroups.Add(new VertexGroup(num2, new int[1]
						{
							i
						}));
					}
					else if (Mathf.Approximately(vertexGroups[num3].value, num2))
					{
						vertexGroups[num3].AddId(i);
					}
					else if (vertexGroups[num3].value < num2)
					{
						vertexGroups.Insert(num3, new VertexGroup(num2, new int[1]
						{
							i
						}));
					}
					else if (num3 < vertexGroups.Count - 1)
					{
						vertexGroups.Insert(num3 + 1, new VertexGroup(num2, new int[1]
						{
							i
						}));
					}
					else
					{
						vertexGroups.Add(new VertexGroup(num2, new int[1]
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

		[SerializeField]
		[HideInInspector]
		private Mesh _startMesh;

		[SerializeField]
		[HideInInspector]
		private Mesh _endMesh;

		[SerializeField]
		[HideInInspector]
		private bool _dontStretchCaps;

		[SerializeField]
		[HideInInspector]
		private TileUVs _tileUVs;

		[SerializeField]
		[HideInInspector]
		private Mesh[] _middleMeshes = new Mesh[0];

		[SerializeField]
		[HideInInspector]
		private List<ExtrudableMesh> extrudableMeshes = new List<ExtrudableMesh>();

		[SerializeField]
		[HideInInspector]
		private Axis _axis = Axis.Z;

		[SerializeField]
		[HideInInspector]
		private Iteration _iteration;

		[SerializeField]
		[HideInInspector]
		private int _randomSeed;

		[SerializeField]
		[HideInInspector]
		private int _repeat = 1;

		[SerializeField]
		[HideInInspector]
		private double _spacing;

		[SerializeField]
		[HideInInspector]
		private Vector2 _scale = Vector2.one;

		private SplineResult lastResult = new SplineResult();

		private bool useLastResult;

		private List<TS_Mesh> combineMeshes = new List<TS_Mesh>();

		private System.Random random;

		private int iterations;

		private bool _hasAnyMesh;

		private bool _hasStartMesh;

		private bool _hasEndMesh;

		private Matrix4x4 vertexMatrix = default(Matrix4x4);

		private Matrix4x4 normalMatrix = default(Matrix4x4);

		public Axis axis
		{
			get
			{
				return _axis;
			}
			set
			{
				if (value != _axis)
				{
					_axis = value;
					UpdateExtrudableMeshes();
					Rebuild(sampleComputer: false);
				}
			}
		}

		public Iteration iteration
		{
			get
			{
				return _iteration;
			}
			set
			{
				if (value != _iteration)
				{
					_iteration = value;
					UpdateExtrudableMeshes();
					Rebuild(sampleComputer: false);
				}
			}
		}

		public int randomSeed
		{
			get
			{
				return _randomSeed;
			}
			set
			{
				if (value != _randomSeed)
				{
					_randomSeed = value;
					if (_iteration == Iteration.Random)
					{
						UpdateExtrudableMeshes();
						Rebuild(sampleComputer: false);
					}
				}
			}
		}

		public int repeat
		{
			get
			{
				return _repeat;
			}
			set
			{
				if (value != _repeat)
				{
					_repeat = value;
					if (_repeat < 1)
					{
						_repeat = 1;
					}
					UpdateEndExtrudeMesh();
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool dontStretchCaps
		{
			get
			{
				return _dontStretchCaps;
			}
			set
			{
				if (value != _dontStretchCaps)
				{
					_dontStretchCaps = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public TileUVs tileUVs
		{
			get
			{
				return _tileUVs;
			}
			set
			{
				if (value != _tileUVs)
				{
					_tileUVs = value;
					Rebuild(sampleComputer: false);
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
					if ((_spacing == 0.0 && value > 0.0) || (value == 0.0 && _spacing > 0.0))
					{
						UpdateExtrudableMeshes();
					}
					_spacing = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public Vector2 scale
		{
			get
			{
				return _scale;
			}
			set
			{
				if (value != _scale)
				{
					_scale = value;
					Rebuild(sampleComputer: false);
				}
			}
		}

		public bool hasAnyMesh => _hasAnyMesh;

		protected override void Awake()
		{
			base.Awake();
			CheckMeshes();
			mesh.name = "Stretch Mesh";
		}

		public Mesh GetStartMesh()
		{
			return _startMesh;
		}

		public Mesh GetEndMesh()
		{
			return _endMesh;
		}

		public MirrorMethod GetStartMeshMirror()
		{
			if (extrudableMeshes.Count == 0)
			{
				return MirrorMethod.None;
			}
			if (extrudableMeshes[0] == null)
			{
				return MirrorMethod.None;
			}
			return extrudableMeshes[0].mirror;
		}

		public MirrorMethod GetEndMeshMirror()
		{
			if (extrudableMeshes.Count < 2)
			{
				return MirrorMethod.None;
			}
			if (extrudableMeshes[1] == null)
			{
				return MirrorMethod.None;
			}
			return extrudableMeshes[1].mirror;
		}

		public void SetStartMeshMirror(MirrorMethod mirror)
		{
			if (extrudableMeshes.Count != 0 && extrudableMeshes[0] != null)
			{
				extrudableMeshes[0].mirror = mirror;
				CheckMeshes();
				Rebuild(sampleComputer: false);
			}
		}

		public void SetEndMeshMirror(MirrorMethod mirror)
		{
			if (extrudableMeshes.Count >= 2 && extrudableMeshes[1] != null)
			{
				extrudableMeshes[1].mirror = mirror;
				CheckMeshes();
				Rebuild(sampleComputer: false);
			}
		}

		public void SetMeshMirror(int index, MirrorMethod mirror)
		{
			if (extrudableMeshes.Count >= 2 + index && extrudableMeshes[2 + index] != null)
			{
				extrudableMeshes[2 + index].mirror = mirror;
				CheckMeshes();
				Rebuild(sampleComputer: false);
			}
		}

		public void SetStartMesh(Mesh inputMesh, MirrorMethod mirror = MirrorMethod.None)
		{
			_startMesh = inputMesh;
			if (extrudableMeshes.Count == 0)
			{
				extrudableMeshes.Add(null);
			}
			if (_startMesh == null)
			{
				extrudableMeshes[0] = null;
				_hasStartMesh = false;
				Rebuild(sampleComputer: false);
			}
			else
			{
				extrudableMeshes[0] = new ExtrudableMesh(_startMesh, _axis);
				extrudableMeshes[0].mirror = mirror;
				CheckMeshes();
				Rebuild(sampleComputer: false);
			}
		}

		public void SetEndMesh(Mesh inputMesh, MirrorMethod mirror = MirrorMethod.None)
		{
			_endMesh = inputMesh;
			if (extrudableMeshes.Count < 2)
			{
				extrudableMeshes.AddRange(new ExtrudableMesh[2 - extrudableMeshes.Count]);
			}
			if (_endMesh == null)
			{
				extrudableMeshes[1] = null;
				_hasEndMesh = false;
				Rebuild(sampleComputer: false);
			}
			else
			{
				extrudableMeshes[1] = new ExtrudableMesh(_endMesh, _axis);
				extrudableMeshes[1].mirror = mirror;
				CheckMeshes();
				Rebuild(sampleComputer: false);
			}
		}

		public Mesh GetMesh(int index)
		{
			return _middleMeshes[index];
		}

		public MirrorMethod GetMeshMirror(int index)
		{
			if (extrudableMeshes[index + 2] == null)
			{
				return MirrorMethod.None;
			}
			return extrudableMeshes[index + 2].mirror;
		}

		public void SetMesh(int index, Mesh inputMesh, MirrorMethod mirror = MirrorMethod.None)
		{
			if (inputMesh == null)
			{
				RemoveMesh(index);
				return;
			}
			_middleMeshes[index] = inputMesh;
			UpdateExtrudableMeshes();
			extrudableMeshes[2 + index].mirror = mirror;
			CheckMeshes();
			Rebuild(sampleComputer: false);
		}

		public void RemoveMesh(int index)
		{
			Mesh[] array = new Mesh[_middleMeshes.Length - 1];
			for (int i = 0; i < _middleMeshes.Length; i++)
			{
				if (i < index)
				{
					array[i] = _middleMeshes[i];
				}
				else if (i > index)
				{
					array[i - 1] = _middleMeshes[i];
				}
			}
			_middleMeshes = array;
			extrudableMeshes.RemoveAt(index + 2);
			UpdateExtrudableMeshes();
			CheckMeshes();
			Rebuild(sampleComputer: false);
		}

		public void AddMesh(Mesh inputMesh)
		{
			if (!(inputMesh == null))
			{
				Mesh[] array = new Mesh[_middleMeshes.Length + 1];
				_middleMeshes.CopyTo(array, 0);
				array[array.Length - 1] = inputMesh;
				_middleMeshes = array;
				UpdateExtrudableMeshes();
				CheckMeshes();
				Rebuild(sampleComputer: false);
			}
		}

		private void CheckMeshes()
		{
			_hasAnyMesh = false;
			_hasStartMesh = false;
			_hasEndMesh = false;
			if (_startMesh != null)
			{
				_hasAnyMesh = true;
				_hasStartMesh = true;
			}
			for (int i = 0; i < _middleMeshes.Length; i++)
			{
				if (_middleMeshes[i] != null)
				{
					_hasAnyMesh = true;
					break;
				}
			}
			if (_endMesh != null)
			{
				_hasAnyMesh = true;
				_hasEndMesh = true;
			}
		}

		public int GetMeshCount()
		{
			return _middleMeshes.Length;
		}

		protected override void BuildMesh()
		{
			if (base.clippedSamples.Length == 0)
			{
				return;
			}
			base.BuildMesh();
			if (!_hasAnyMesh && !multithreaded)
			{
				CheckMeshes();
				UpdateExtrudableMeshes();
				if (!_hasAnyMesh)
				{
					return;
				}
			}
			Generate();
		}

		private void Generate()
		{
			random = new System.Random(_randomSeed);
			useLastResult = false;
			iterations = 0;
			if (_hasStartMesh)
			{
				iterations++;
			}
			if (_hasEndMesh)
			{
				iterations++;
			}
			iterations += (extrudableMeshes.Count - 2) * _repeat;
			double num = 1.0 / (double)iterations;
			double num2 = num * _spacing * 0.5;
			if (combineMeshes.Count < iterations)
			{
				combineMeshes.AddRange(new TS_Mesh[iterations - combineMeshes.Count]);
			}
			else if (combineMeshes.Count > iterations)
			{
				combineMeshes.RemoveRange(combineMeshes.Count - 1 - (combineMeshes.Count - iterations), combineMeshes.Count - iterations);
			}
			for (int i = 0; i < iterations; i++)
			{
				double from = (double)i * num + num2;
				double to = (double)i * num + num - num2;
				if (combineMeshes[i] == null)
				{
					combineMeshes[i] = new TS_Mesh();
				}
				Stretch(extrudableMeshes[GetMeshIndex(i)], combineMeshes[i], from, to);
				if (_spacing == 0.0)
				{
					useLastResult = true;
				}
			}
			if (_dontStretchCaps)
			{
				if (_hasStartMesh)
				{
					TS_Mesh tS_Mesh = new TS_Mesh();
					TRS(extrudableMeshes[0], tS_Mesh, 0.0);
					combineMeshes.Add(tS_Mesh);
				}
				if (_hasEndMesh)
				{
					TS_Mesh tS_Mesh2 = new TS_Mesh();
					TRS(extrudableMeshes[1], tS_Mesh2, 1.0);
					combineMeshes.Add(tS_Mesh2);
				}
			}
			if (tsMesh == null)
			{
				tsMesh = new TS_Mesh();
			}
			tsMesh.Combine(combineMeshes, overwrite: true);
		}

		private int GetMeshIndex(int repeatIndex)
		{
			if (repeatIndex == 0 && _hasStartMesh && !_dontStretchCaps)
			{
				return 0;
			}
			if (repeatIndex == iterations - 1 && _hasEndMesh && !_dontStretchCaps)
			{
				return 1;
			}
			if (_middleMeshes.Length == 0)
			{
				if (_hasStartMesh && !_dontStretchCaps)
				{
					return 0;
				}
				if (_hasEndMesh && !_dontStretchCaps)
				{
					return 1;
				}
			}
			if (_middleMeshes.Length == 1)
			{
				return 2;
			}
			if (_iteration == Iteration.Random)
			{
				return 2 + random.Next(_middleMeshes.Length);
			}
			return 2 + (repeatIndex - ((!_hasStartMesh) ? 1 : 0)) % _middleMeshes.Length;
		}

		private void TRS(ExtrudableMesh source, TS_Mesh target, double percent)
		{
			CreateTSFromExtrudableMesh(source, ref target);
			SplineResult splineResult = Evaluate(percent);
			Quaternion rhs = Quaternion.identity;
			switch (axis)
			{
			case Axis.X:
				rhs = Quaternion.LookRotation(Vector3.right);
				break;
			case Axis.Y:
				rhs = Quaternion.LookRotation(Vector3.up, Vector3.back);
				break;
			}
			ref Matrix4x4 reference = ref vertexMatrix;
			Vector3 position = splineResult.position;
			Vector3 right = splineResult.right;
			Vector3 offset = base.offset;
			Vector3 a = position + right * offset.x;
			Vector3 normal = splineResult.normal;
			Vector3 offset2 = base.offset;
			Vector3 a2 = a + normal * offset2.y;
			Vector3 direction = splineResult.direction;
			Vector3 offset3 = base.offset;
			reference.SetTRS(a2 + direction * offset3.z, splineResult.rotation * Quaternion.AngleAxis(base.rotation, Vector3.forward) * rhs, new Vector3(_scale.x, _scale.y, 1f) * splineResult.size);
			normalMatrix = vertexMatrix.inverse.transpose;
			for (int i = 0; i < target.vertexCount; i++)
			{
				target.vertices[i] = vertexMatrix.MultiplyPoint3x4(source.vertices[i]);
				target.normals[i] = normalMatrix.MultiplyVector(source.normals[i]);
			}
		}

		private void CreateTSFromExtrudableMesh(ExtrudableMesh source, ref TS_Mesh target)
		{
			if (target.vertices.Length != source.vertices.Length)
			{
				target.vertices = new Vector3[source.vertices.Length];
			}
			if (target.normals.Length != source.normals.Length)
			{
				target.normals = new Vector3[source.normals.Length];
			}
			if (target.tangents.Length != source.tangents.Length)
			{
				target.tangents = new Vector4[source.tangents.Length];
			}
			if (target.colors.Length != source.colors.Length)
			{
				target.colors = new Color[source.colors.Length];
			}
			if (target.uv.Length != source.uv.Length)
			{
				target.uv = new Vector2[source.uv.Length];
			}
			source.uv.CopyTo(target.uv, 0);
			if (target.uv.Length != target.vertices.Length)
			{
				Vector2[] array = new Vector2[target.vertices.Length];
				for (int i = 0; i < target.vertices.Length; i++)
				{
					if (i < target.uv.Length)
					{
						array[i] = target.uv[i];
					}
					else
					{
						array[i] = Vector2.zero;
					}
				}
				target.uv = array;
			}
			source.colors.CopyTo(target.colors, 0);
			target.subMeshes.Clear();
			for (int j = 0; j < source.subMeshes.Count; j++)
			{
				target.subMeshes.Add(source.subMeshes[j].triangles);
			}
		}

		private void Stretch(ExtrudableMesh source, TS_Mesh target, double from, double to)
		{
			CreateTSFromExtrudableMesh(source, ref target);
			SplineResult splineResult = new SplineResult();
			Vector2 vector = Vector2.zero;
			Vector3 vector2 = Vector3.zero;
			Matrix4x4 matrix4x = default(Matrix4x4);
			Quaternion rotation = Quaternion.identity;
			switch (axis)
			{
			case Axis.X:
				rotation = Quaternion.LookRotation(Vector3.left);
				break;
			case Axis.Y:
				rotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
				break;
			}
			for (int i = 0; i < source.vertexGroups.Count; i++)
			{
				double t = 0.0;
				switch (axis)
				{
				case Axis.X:
					t = DMath.Clamp01(Mathf.InverseLerp(source.bounds.min.x, source.bounds.max.x, source.vertexGroups[i].value));
					break;
				case Axis.Y:
					t = DMath.Clamp01(Mathf.InverseLerp(source.bounds.min.y, source.bounds.max.y, source.vertexGroups[i].value));
					break;
				case Axis.Z:
					t = DMath.Clamp01(Mathf.InverseLerp(source.bounds.min.z, source.bounds.max.z, source.vertexGroups[i].value));
					break;
				}
				if (useLastResult && i == source.vertexGroups.Count)
				{
					splineResult = lastResult;
				}
				else
				{
					Evaluate(splineResult, UnclipPercent(DMath.Lerp(from, to, t)));
				}
				Vector3 position = splineResult.position;
				Vector3 right = splineResult.right;
				Vector3 offset = base.offset;
				Vector3 a = position + right * offset.x;
				Vector3 normal = splineResult.normal;
				Vector3 offset2 = base.offset;
				Vector3 a2 = a + normal * offset2.y;
				Vector3 direction = splineResult.direction;
				Vector3 offset3 = base.offset;
				matrix4x.SetTRS(a2 + direction * offset3.z, splineResult.rotation * Quaternion.AngleAxis(base.rotation, Vector3.forward), new Vector3(_scale.x, _scale.y, 1f) * splineResult.size);
				if (i == 0)
				{
					lastResult.CopyFrom(splineResult);
				}
				for (int j = 0; j < source.vertexGroups[i].ids.Length; j++)
				{
					int num = source.vertexGroups[i].ids[j];
					vector2 = rotation * source.vertices[num];
					vector2.z = 0f;
					target.vertices[num] = matrix4x.MultiplyPoint3x4(vector2);
					vector2 = rotation * source.normals[num];
					target.normals[num] = matrix4x.MultiplyVector(vector2);
					target.colors[num] = target.colors[num] * splineResult.color;
					vector = target.uv[num];
					switch (_tileUVs)
					{
					case TileUVs.U:
						vector.x = (float)splineResult.percent;
						break;
					case TileUVs.V:
						vector.y = (float)splineResult.percent;
						break;
					case TileUVs.UniformU:
						vector.x = CalculateLength(0.0, splineResult.percent);
						break;
					case TileUVs.UniformV:
						vector.y = CalculateLength(0.0, splineResult.percent);
						break;
					}
					ref Vector2 reference = ref target.uv[num];
					float x = vector.x;
					Vector2 uvScale = base.uvScale;
					float x2 = x * uvScale.x;
					float y = vector.y;
					Vector2 uvScale2 = base.uvScale;
					reference = new Vector2(x2, y * uvScale2.y);
					target.uv[num] += base.uvOffset;
				}
			}
		}

		private void UpdateExtrudableMeshes()
		{
			iterations = 0;
			if (_startMesh != null)
			{
				iterations++;
			}
			if (_endMesh != null)
			{
				iterations++;
			}
			iterations += (extrudableMeshes.Count - 2) * _repeat;
			int num = 2 + _middleMeshes.Length;
			if (extrudableMeshes.Count < num)
			{
				extrudableMeshes.AddRange(new ExtrudableMesh[num - extrudableMeshes.Count]);
			}
			for (int i = 0; i < _middleMeshes.Length; i++)
			{
				if (_middleMeshes[i] == null)
				{
					RemoveMesh(i);
					i--;
					continue;
				}
				MirrorMethod mirror = MirrorMethod.None;
				if (extrudableMeshes[i + 2] != null)
				{
					mirror = extrudableMeshes[i + 2].mirror;
					extrudableMeshes[i + 2].Update(_middleMeshes[i], _axis);
				}
				else
				{
					extrudableMeshes[i + 2] = new ExtrudableMesh(_middleMeshes[i], _axis);
				}
				extrudableMeshes[i + 2].mirror = mirror;
			}
			UpdateStartExtrudeMesh();
			UpdateEndExtrudeMesh();
		}

		private void UpdateStartExtrudeMesh()
		{
			MirrorMethod mirror = MirrorMethod.None;
			if (extrudableMeshes[0] != null)
			{
				mirror = extrudableMeshes[0].mirror;
			}
			if (_startMesh != null)
			{
				extrudableMeshes[0] = new ExtrudableMesh(_startMesh, _axis);
			}
			else if (_middleMeshes.Length > 0)
			{
				if (_iteration == Iteration.Ordered)
				{
					extrudableMeshes[0] = new ExtrudableMesh(_middleMeshes[0], _axis);
				}
				else
				{
					random = new System.Random(_randomSeed);
					extrudableMeshes[0] = new ExtrudableMesh(_middleMeshes[random.Next(_middleMeshes.Length - 1)], _axis);
				}
			}
			if (extrudableMeshes[0] != null)
			{
				extrudableMeshes[0].mirror = mirror;
			}
		}

		private void UpdateEndExtrudeMesh()
		{
			MirrorMethod mirrorMethod = MirrorMethod.None;
			mirrorMethod = MirrorMethod.None;
			if (extrudableMeshes[1] != null)
			{
				mirrorMethod = extrudableMeshes[1].mirror;
			}
			if (_endMesh != null)
			{
				extrudableMeshes[1] = new ExtrudableMesh(_endMesh, _axis);
			}
			else if (_middleMeshes.Length > 0)
			{
				if (_iteration == Iteration.Ordered)
				{
					extrudableMeshes[1] = new ExtrudableMesh(_middleMeshes[(!(_startMesh != null)) ? ((iterations - 1) % _middleMeshes.Length) : ((iterations - 2) % _middleMeshes.Length)], _axis);
				}
				else
				{
					random = new System.Random(_randomSeed);
					for (int i = 0; i < iterations - 1; i++)
					{
						random.Next(_middleMeshes.Length - 1);
					}
					extrudableMeshes[1] = new ExtrudableMesh(_middleMeshes[random.Next(_middleMeshes.Length - 1)], _axis);
				}
			}
			if (extrudableMeshes[1] != null)
			{
				extrudableMeshes[1].mirror = mirrorMethod;
			}
		}
	}
}
