using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Object Bender")]
	public class ObjectBender : SplineUser
	{
		public enum Axis
		{
			X,
			Y,
			Z
		}

		[Serializable]
		public class BendProperty
		{
			public bool enabled = true;

			public TS_Transform transform;

			public bool applyRotation = true;

			public bool applyScale = true;

			public bool generateLightmapUVs;

			[SerializeField]
			[HideInInspector]
			private bool _bendMesh = true;

			[SerializeField]
			[HideInInspector]
			private bool _bendSpline = true;

			[SerializeField]
			[HideInInspector]
			private bool _bendCollider = true;

			private float colliderUpdateDue;

			public float colliderUpdateRate = 0.2f;

			private bool updateCollider;

			public Vector3 originalPosition = Vector3.zero;

			public Vector3 originalScale = Vector3.one;

			public Quaternion originalRotation = Quaternion.identity;

			public Vector3 positionPercent;

			public Vector3[] vertexPercents = new Vector3[0];

			public Vector3[] normals = new Vector3[0];

			public Vector3[] colliderVertexPercents = new Vector3[0];

			public Vector3[] colliderNormals = new Vector3[0];

			[SerializeField]
			[HideInInspector]
			private Mesh originalMesh;

			[SerializeField]
			[HideInInspector]
			private Mesh originalColliderMesh;

			private Spline _originalSpline;

			[SerializeField]
			[HideInInspector]
			private Mesh destinationMesh;

			[SerializeField]
			[HideInInspector]
			private Mesh destinationColliderMesh;

			public Spline destinationSpline;

			public TS_Mesh _editMesh;

			public TS_Mesh _editColliderMesh;

			public MeshFilter filter;

			public MeshCollider collider;

			public SplineComputer splineComputer;

			public Vector3[] splinePointPercents = new Vector3[0];

			public Vector3[] primaryTangentPercents = new Vector3[0];

			public Vector3[] secondaryTangentPercents = new Vector3[0];

			[SerializeField]
			[HideInInspector]
			private bool parent;

			public bool isValid => transform != null && transform.transform != null;

			public bool bendMesh
			{
				get
				{
					return _bendMesh;
				}
				set
				{
					if (value == _bendMesh)
					{
						return;
					}
					_bendMesh = value;
					if (value)
					{
						if (filter != null && filter.sharedMesh != null)
						{
							normals = originalMesh.normals;
							for (int i = 0; i < normals.Length; i++)
							{
								normals[i] = transform.transform.TransformDirection(normals[i]);
							}
						}
					}
					else
					{
						RevertMesh();
					}
				}
			}

			public bool bendCollider
			{
				get
				{
					return _bendCollider;
				}
				set
				{
					if (value == _bendCollider)
					{
						return;
					}
					_bendCollider = value;
					if (value)
					{
						if (collider != null && collider.sharedMesh != null && collider.sharedMesh != originalMesh)
						{
							colliderNormals = originalColliderMesh.normals;
						}
					}
					else
					{
						RevertCollider();
					}
				}
			}

			public bool bendSpline
			{
				get
				{
					return _bendSpline;
				}
				set
				{
					_bendSpline = value;
					if (!value)
					{
					}
				}
			}

			public TS_Mesh editMesh
			{
				get
				{
					if (!bendMesh || originalMesh == null)
					{
						_editMesh = null;
					}
					else if (_editMesh == null && originalMesh != null)
					{
						_editMesh = new TS_Mesh(originalMesh);
					}
					return _editMesh;
				}
			}

			public TS_Mesh editColliderMesh
			{
				get
				{
					if (!bendCollider || originalColliderMesh == null)
					{
						_editColliderMesh = null;
					}
					else if (_editColliderMesh == null && originalColliderMesh != null && originalColliderMesh != originalMesh)
					{
						_editColliderMesh = new TS_Mesh(originalColliderMesh);
					}
					return _editColliderMesh;
				}
			}

			public Spline originalSpline
			{
				get
				{
					if (!bendSpline || splineComputer == null)
					{
						_originalSpline = null;
					}
					else if (_originalSpline == null && splineComputer != null)
					{
						_originalSpline = new Spline(splineComputer.type);
						_originalSpline.points = splineComputer.GetPoints();
					}
					return _originalSpline;
				}
			}

			public BendProperty(Transform t, bool isParent = false)
			{
				parent = isParent;
				transform = new TS_Transform(t);
				originalPosition = t.localPosition;
				originalScale = t.localScale;
				originalRotation = t.localRotation;
				filter = t.GetComponent<MeshFilter>();
				collider = t.GetComponent<MeshCollider>();
				if (filter != null && filter.sharedMesh != null)
				{
					originalMesh = filter.sharedMesh;
					normals = originalMesh.normals;
					for (int i = 0; i < normals.Length; i++)
					{
						normals[i] = transform.transform.TransformDirection(normals[i]).normalized;
					}
				}
				if (collider != null && collider.sharedMesh != null)
				{
					originalColliderMesh = collider.sharedMesh;
					colliderNormals = originalColliderMesh.normals;
					for (int j = 0; j < colliderNormals.Length; j++)
					{
						colliderNormals[j] = transform.transform.TransformDirection(colliderNormals[j]);
					}
				}
				if (!parent)
				{
					splineComputer = t.GetComponent<SplineComputer>();
				}
				if (splineComputer != null)
				{
					if (splineComputer.isClosed)
					{
						originalSpline.Close();
					}
					destinationSpline = new Spline(originalSpline.type);
					destinationSpline.points = new SplinePoint[originalSpline.points.Length];
					destinationSpline.points = splineComputer.GetPoints();
					if (splineComputer.isClosed)
					{
						destinationSpline.Close();
					}
				}
			}

			public void Revert()
			{
				if (isValid)
				{
					RevertTransform();
					RevertCollider();
					RevertMesh();
					if (splineComputer != null)
					{
						splineComputer.SetPoints(_originalSpline.points);
					}
				}
			}

			private void RevertMesh()
			{
				if (filter != null)
				{
					filter.sharedMesh = originalMesh;
				}
				destinationMesh = null;
			}

			private void RevertTransform()
			{
				transform.localPosition = originalPosition;
				transform.localRotation = originalRotation;
				transform.Update();
				transform.scale = originalScale;
				transform.Update();
			}

			private void RevertCollider()
			{
				if (collider != null)
				{
					collider.sharedMesh = originalColliderMesh;
				}
				destinationColliderMesh = null;
			}

			public void Apply(bool applyTransform)
			{
				if (!enabled || !isValid)
				{
					return;
				}
				if (applyTransform)
				{
					transform.Update();
				}
				if (editMesh != null && editMesh.hasUpdate)
				{
					ApplyMesh();
				}
				if (bendCollider && collider != null && !updateCollider && ((editColliderMesh == null && editMesh != null) || editColliderMesh != null))
				{
					updateCollider = true;
					if (Application.isPlaying)
					{
						colliderUpdateDue = Time.time + colliderUpdateRate;
					}
				}
				if (splineComputer != null)
				{
					ApplySpline();
				}
			}

			public void Update()
			{
				if (Time.time >= colliderUpdateDue && updateCollider)
				{
					updateCollider = false;
					ApplyCollider();
				}
			}

			private void ApplyMesh()
			{
				if (!(filter == null))
				{
					MeshUtility.InverseTransformMesh(editMesh, transform.transform);
					MeshUtility.CalculateTangents(editMesh);
					if (destinationMesh == null)
					{
						destinationMesh = new Mesh();
						destinationMesh.name = originalMesh.name;
					}
					editMesh.WriteMesh(ref destinationMesh);
					destinationMesh.RecalculateBounds();
					filter.sharedMesh = destinationMesh;
				}
			}

			private void ApplyCollider()
			{
				if (collider == null)
				{
					return;
				}
				if (originalColliderMesh == originalMesh)
				{
					collider.sharedMesh = filter.sharedMesh;
					return;
				}
				MeshUtility.InverseTransformMesh(editColliderMesh, transform.transform);
				MeshUtility.CalculateTangents(editColliderMesh);
				if (destinationColliderMesh == null)
				{
					destinationColliderMesh = new Mesh();
					destinationColliderMesh.name = originalColliderMesh.name;
				}
				editColliderMesh.WriteMesh(ref destinationColliderMesh);
				destinationColliderMesh.RecalculateBounds();
				collider.sharedMesh = destinationColliderMesh;
			}

			private void ApplySpline()
			{
				if (destinationSpline != null)
				{
					splineComputer.SetPoints(destinationSpline.points);
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		private bool _bend;

		[HideInInspector]
		public BendProperty[] bendProperties = new BendProperty[0];

		[SerializeField]
		[HideInInspector]
		private TS_Bounds bounds;

		[SerializeField]
		[HideInInspector]
		private Axis _axis = Axis.Z;

		[SerializeField]
		[HideInInspector]
		private bool _autoNormals;

		[SerializeField]
		[HideInInspector]
		private Vector3 _upVector = Vector3.up;

		private SplineResult bendResult = new SplineResult();

		public bool bend
		{
			get
			{
				return _bend;
			}
			set
			{
				if (_bend != value)
				{
					_bend = value;
					if (value)
					{
						UpdateReferences();
						Rebuild(sampleComputer: false);
					}
					else
					{
						Revert();
					}
				}
			}
		}

		public Axis axis
		{
			get
			{
				return _axis;
			}
			set
			{
				if (base.computer != null && value != _axis)
				{
					_axis = value;
					UpdateReferences();
					Rebuild(sampleComputer: false);
				}
				else
				{
					_axis = value;
				}
			}
		}

		public bool autoNormals
		{
			get
			{
				return _autoNormals;
			}
			set
			{
				if (base.computer != null && value != _autoNormals)
				{
					_autoNormals = value;
					Rebuild(sampleComputer: false);
				}
				else
				{
					_autoNormals = value;
				}
			}
		}

		public Vector3 upVector
		{
			get
			{
				return _upVector;
			}
			set
			{
				if (base.computer != null && value != _upVector)
				{
					_upVector = value;
					Rebuild(sampleComputer: false);
				}
				else
				{
					_upVector = value;
				}
			}
		}

		private void GetTransformsRecursively(Transform current, ref List<Transform> transformList)
		{
			transformList.Add(current);
			IEnumerator enumerator = current.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current2 = (Transform)enumerator.Current;
					GetTransformsRecursively(current2, ref transformList);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private void GetObjects()
		{
			List<Transform> transformList = new List<Transform>();
			GetTransformsRecursively(base.transform, ref transformList);
			BendProperty[] array = new BendProperty[transformList.Count];
			for (int i = 0; i < transformList.Count; i++)
			{
				CreateProperty(ref array[i], transformList[i]);
			}
			bendProperties = array;
		}

		public TS_Bounds GetBounds()
		{
			return new TS_Bounds(bounds.min, bounds.max, bounds.center);
		}

		private void CreateProperty(ref BendProperty property, Transform t)
		{
			property = new BendProperty(t, t == base.transform);
			for (int i = 0; i < bendProperties.Length; i++)
			{
				if (bendProperties[i].transform.transform == t)
				{
					property.applyRotation = bendProperties[i].applyRotation;
					property.applyScale = bendProperties[i].applyScale;
					property.bendMesh = bendProperties[i].bendMesh;
					property.bendCollider = bendProperties[i].bendCollider;
					property.generateLightmapUVs = bendProperties[i].generateLightmapUVs;
					property.colliderUpdateRate = bendProperties[i].colliderUpdateRate;
					break;
				}
			}
			if (t != base.transform)
			{
				property.originalPosition = base.transform.InverseTransformPoint(t.position);
				property.originalRotation = Quaternion.Inverse(base.transform.rotation) * t.rotation;
			}
		}

		private void CalculateBounds()
		{
			if (bounds == null)
			{
				bounds = new TS_Bounds(Vector3.zero, Vector3.zero);
			}
			bounds.min = (bounds.max = Vector3.zero);
			for (int i = 0; i < bendProperties.Length; i++)
			{
				CalculatePropertyBounds(ref bendProperties[i]);
			}
			for (int j = 0; j < bendProperties.Length; j++)
			{
				GetPercent(bendProperties[j]);
			}
		}

		private void CalculatePropertyBounds(ref BendProperty property)
		{
			if (property.transform.transform == base.transform)
			{
				if (0f < bounds.min.x)
				{
					bounds.min.x = 0f;
				}
				if (0f < bounds.min.y)
				{
					bounds.min.y = 0f;
				}
				if (0f < bounds.min.z)
				{
					bounds.min.z = 0f;
				}
				if (0f > bounds.max.x)
				{
					bounds.max.x = 0f;
				}
				if (0f > bounds.max.y)
				{
					bounds.max.y = 0f;
				}
				if (0f > bounds.max.z)
				{
					bounds.max.z = 0f;
				}
			}
			else
			{
				if (property.originalPosition.x < bounds.min.x)
				{
					bounds.min.x = property.originalPosition.x;
				}
				if (property.originalPosition.y < bounds.min.y)
				{
					bounds.min.y = property.originalPosition.y;
				}
				if (property.originalPosition.z < bounds.min.z)
				{
					bounds.min.z = property.originalPosition.z;
				}
				if (property.originalPosition.x > bounds.max.x)
				{
					bounds.max.x = property.originalPosition.x;
				}
				if (property.originalPosition.y > bounds.max.y)
				{
					bounds.max.y = property.originalPosition.y;
				}
				if (property.originalPosition.z > bounds.max.z)
				{
					bounds.max.z = property.originalPosition.z;
				}
			}
			if (property.editMesh != null)
			{
				for (int i = 0; i < property.editMesh.vertices.Length; i++)
				{
					Vector3 position = property.transform.TransformPoint(property.editMesh.vertices[i]);
					position = base.transform.InverseTransformPoint(position);
					if (position.x < bounds.min.x)
					{
						bounds.min.x = position.x;
					}
					if (position.y < bounds.min.y)
					{
						bounds.min.y = position.y;
					}
					if (position.z < bounds.min.z)
					{
						bounds.min.z = position.z;
					}
					if (position.x > bounds.max.x)
					{
						bounds.max.x = position.x;
					}
					if (position.y > bounds.max.y)
					{
						bounds.max.y = position.y;
					}
					if (position.z > bounds.max.z)
					{
						bounds.max.z = position.z;
					}
				}
			}
			if (property.editColliderMesh != null)
			{
				for (int j = 0; j < property.editColliderMesh.vertices.Length; j++)
				{
					Vector3 position2 = property.transform.TransformPoint(property.editColliderMesh.vertices[j]);
					position2 = base.transform.InverseTransformPoint(position2);
					if (position2.x < bounds.min.x)
					{
						bounds.min.x = position2.x;
					}
					if (position2.y < bounds.min.y)
					{
						bounds.min.y = position2.y;
					}
					if (position2.z < bounds.min.z)
					{
						bounds.min.z = position2.z;
					}
					if (position2.x > bounds.max.x)
					{
						bounds.max.x = position2.x;
					}
					if (position2.y > bounds.max.y)
					{
						bounds.max.y = position2.y;
					}
					if (position2.z > bounds.max.z)
					{
						bounds.max.z = position2.z;
					}
				}
			}
			if (property.originalSpline != null)
			{
				for (int k = 0; k < property.originalSpline.points.Length; k++)
				{
					Vector3 vector = base.transform.InverseTransformPoint(property.originalSpline.points[k].position);
					if (vector.x < bounds.min.x)
					{
						bounds.min.x = vector.x;
					}
					if (vector.y < bounds.min.y)
					{
						bounds.min.y = vector.y;
					}
					if (vector.z < bounds.min.z)
					{
						bounds.min.z = vector.z;
					}
					if (vector.x > bounds.max.x)
					{
						bounds.max.x = vector.x;
					}
					if (vector.y > bounds.max.y)
					{
						bounds.max.y = vector.y;
					}
					if (vector.z > bounds.max.z)
					{
						bounds.max.z = vector.z;
					}
				}
			}
			bounds.CreateFromMinMax(bounds.min, bounds.max);
		}

		public void GetPercent(BendProperty property)
		{
			if (property.transform.transform != base.transform)
			{
				property.positionPercent = GetPercentage(base.transform.InverseTransformPoint(property.transform.position));
			}
			else
			{
				property.positionPercent = GetPercentage(Vector3.zero);
			}
			if (property.editMesh != null)
			{
				if (property.vertexPercents.Length != property.editMesh.vertexCount)
				{
					property.vertexPercents = new Vector3[property.editMesh.vertexCount];
				}
				if (property.editColliderMesh != null && property.colliderVertexPercents.Length != property.editMesh.vertexCount)
				{
					property.colliderVertexPercents = new Vector3[property.editColliderMesh.vertexCount];
				}
				for (int i = 0; i < property.editMesh.vertexCount; i++)
				{
					Vector3 position = property.transform.TransformPoint(property.editMesh.vertices[i]);
					position = base.transform.InverseTransformPoint(position);
					property.vertexPercents[i] = GetPercentage(position);
				}
				if (property.editColliderMesh != null)
				{
					for (int j = 0; j < property.editColliderMesh.vertexCount; j++)
					{
						Vector3 position2 = property.transform.TransformPoint(property.editColliderMesh.vertices[j]);
						position2 = base.transform.InverseTransformPoint(position2);
						property.colliderVertexPercents[j] = GetPercentage(position2);
					}
				}
			}
			if (property.splineComputer != null)
			{
				SplinePoint[] points = property.splineComputer.GetPoints();
				property.splinePointPercents = new Vector3[points.Length];
				property.primaryTangentPercents = new Vector3[points.Length];
				property.secondaryTangentPercents = new Vector3[points.Length];
				for (int k = 0; k < points.Length; k++)
				{
					property.splinePointPercents[k] = GetPercentage(base.transform.InverseTransformPoint(points[k].position));
					property.primaryTangentPercents[k] = GetPercentage(base.transform.InverseTransformPoint(points[k].tangent));
					property.secondaryTangentPercents[k] = GetPercentage(base.transform.InverseTransformPoint(points[k].tangent2));
				}
			}
		}

		private void Revert()
		{
			for (int i = 0; i < bendProperties.Length; i++)
			{
				bendProperties[i].Revert();
			}
		}

		public void UpdateReferences()
		{
			if (_bend)
			{
				for (int i = 0; i < bendProperties.Length; i++)
				{
					bendProperties[i].Revert();
				}
			}
			GetObjects();
			CalculateBounds();
			if (_bend)
			{
				Bend();
				for (int j = 0; j < bendProperties.Length; j++)
				{
					bendProperties[j].Apply(j > 0 || base.transform != base.rootUser.computer.transform);
					bendProperties[j].Update();
				}
			}
		}

		private void GetBendResult(Vector3 percentage)
		{
			Evaluate(bendResult, UnclipPercent(percentage.z));
			Vector3 right = bendResult.right;
			bendResult.position += right * Mathf.Lerp(bounds.min.x, bounds.max.x, percentage.x) * bendResult.size;
			if (_autoNormals)
			{
				Vector3 normalized = Vector3.Cross(bendResult.direction, _upVector).normalized;
				bendResult.position += Vector3.Cross(normalized, bendResult.direction).normalized * Mathf.Lerp(bounds.min.y, bounds.max.y, percentage.y) * bendResult.size;
			}
			else
			{
				bendResult.position += bendResult.normal * Mathf.Lerp(bounds.min.y, bounds.max.y, percentage.y) * bendResult.size;
			}
		}

		private Vector3 GetPercentage(Vector3 point)
		{
			float x = 0f;
			float y = 0f;
			float z = 0f;
			switch (axis)
			{
			case Axis.X:
				x = Mathf.Clamp01(Mathf.InverseLerp(bounds.max.z, bounds.min.z, point.z));
				y = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.y, bounds.max.y, point.y));
				z = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.x, bounds.max.x, point.x));
				break;
			case Axis.Y:
				x = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.x, bounds.max.x, point.x));
				y = Mathf.Clamp01(Mathf.InverseLerp(bounds.max.z, bounds.min.z, point.z));
				z = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.y, bounds.max.y, point.y));
				break;
			case Axis.Z:
				x = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.x, bounds.max.x, point.x));
				y = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.y, bounds.max.y, point.y));
				z = Mathf.Clamp01(Mathf.InverseLerp(bounds.min.z, bounds.max.z, point.z));
				break;
			}
			return new Vector3(x, y, z);
		}

		protected override void Build()
		{
			base.Build();
			if (_bend)
			{
				Bend();
			}
		}

		private void Bend()
		{
			if (base.samples.Length > 1)
			{
				for (int i = 0; i < bendProperties.Length; i++)
				{
					BendObject(bendProperties[i]);
				}
			}
		}

		public void BendObject(BendProperty p)
		{
			if (!p.enabled)
			{
				return;
			}
			Quaternion rhs = Quaternion.identity;
			switch (axis)
			{
			case Axis.X:
				rhs = Quaternion.AngleAxis(-90f, Vector3.up);
				break;
			case Axis.Y:
				rhs = Quaternion.AngleAxis(90f, Vector3.right);
				break;
			}
			GetBendResult(p.positionPercent);
			p.transform.position = bendResult.position;
			if (p.applyRotation)
			{
				p.transform.rotation = bendResult.rotation * rhs * p.originalRotation;
			}
			else
			{
				p.transform.rotation = p.originalRotation;
			}
			if (p.applyScale)
			{
				p.transform.scale = p.originalScale * bendResult.size;
			}
			if (p.editMesh != null)
			{
				for (int i = 0; i < p.vertexPercents.Length; i++)
				{
					GetBendResult(p.vertexPercents[i]);
					p.editMesh.vertices[i] = bendResult.position;
					switch (axis)
					{
					case Axis.X:
						p.editMesh.normals[i] = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * rhs * Quaternion.FromToRotation(Vector3.up, bendResult.normal) * p.normals[i];
						break;
					case Axis.Y:
						p.editMesh.normals[i] = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * rhs * Quaternion.FromToRotation(Vector3.up, bendResult.normal) * p.normals[i];
						break;
					case Axis.Z:
						p.editMesh.normals[i] = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * p.normals[i];
						break;
					}
				}
				p.editMesh.hasUpdate = true;
			}
			if (p._editColliderMesh != null)
			{
				for (int j = 0; j < p.colliderVertexPercents.Length; j++)
				{
					GetBendResult(p.colliderVertexPercents[j]);
					p.editColliderMesh.vertices[j] = bendResult.position;
					switch (axis)
					{
					case Axis.X:
						p.editColliderMesh.normals[j] = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * rhs * Quaternion.FromToRotation(Vector3.up, bendResult.normal) * p.colliderNormals[j];
						break;
					case Axis.Y:
						p.editColliderMesh.normals[j] = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * rhs * Quaternion.FromToRotation(Vector3.up, bendResult.normal) * p.colliderNormals[j];
						break;
					case Axis.Z:
						p.editColliderMesh.normals[j] = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * p.colliderNormals[j];
						break;
					}
				}
				p.editColliderMesh.hasUpdate = true;
			}
			if (p.originalSpline == null)
			{
				return;
			}
			for (int k = 0; k < p.splinePointPercents.Length; k++)
			{
				SplinePoint splinePoint = p.originalSpline.points[k];
				GetBendResult(p.splinePointPercents[k]);
				splinePoint.position = bendResult.position;
				GetBendResult(p.primaryTangentPercents[k]);
				splinePoint.tangent = bendResult.position;
				GetBendResult(p.secondaryTangentPercents[k]);
				splinePoint.tangent2 = bendResult.position;
				switch (axis)
				{
				case Axis.X:
					splinePoint.normal = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * rhs * Quaternion.FromToRotation(Vector3.up, bendResult.normal) * splinePoint.normal;
					break;
				case Axis.Y:
					splinePoint.normal = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * rhs * Quaternion.FromToRotation(Vector3.up, bendResult.normal) * splinePoint.normal;
					break;
				case Axis.Z:
					splinePoint.normal = Quaternion.LookRotation(bendResult.direction, bendResult.normal) * splinePoint.normal;
					break;
				}
				p.destinationSpline.points[k] = splinePoint;
			}
		}

		protected override void PostBuild()
		{
			base.PostBuild();
			if (_bend)
			{
				for (int i = 0; i < bendProperties.Length; i++)
				{
					bendProperties[i].Apply(i > 0 || base.transform != base.rootUser.computer.transform);
					bendProperties[i].Update();
				}
			}
		}

		protected override void LateRun()
		{
			base.LateRun();
			for (int i = 0; i < bendProperties.Length; i++)
			{
				bendProperties[i].Update();
			}
		}
	}
}
