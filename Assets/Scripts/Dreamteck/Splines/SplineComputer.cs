using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	[AddComponentMenu("Dreamteck/Splines/Spline Computer")]
	public class SplineComputer : MonoBehaviour
	{
		[Serializable]
		public class NodeLink
		{
			public Node node;

			public int pointIndex;
		}

		public enum Space
		{
			World,
			Local
		}

		[Serializable]
		public class Morph
		{
			[Serializable]
			internal class SplineMorphState
			{
				public SplinePoint[] points = new SplinePoint[0];

				public float percent = 1f;

				public string name = string.Empty;
			}

			[SerializeField]
			private SplineComputer computer;

			[SerializeField]
			private SplineMorphState[] morphStates = new SplineMorphState[0];

			[SerializeField]
			internal bool initialized;

			public Morph(SplineComputer input)
			{
				computer = input;
				initialized = true;
			}

			public void SetWeight(int index, float weight)
			{
				morphStates[index].percent = Mathf.Clamp01(weight);
				Update();
			}

			public void SetWeight(string name, float weight)
			{
				int channelIndex = GetChannelIndex(name);
				morphStates[channelIndex].percent = Mathf.Clamp01(weight);
				Update();
			}

			public void CaptureSnapshot(int index)
			{
				if (morphStates.Length > 0 && computer.pointCount != morphStates[0].points.Length && index != 0)
				{
					UnityEngine.Debug.LogError("Point count must be the same as " + computer.pointCount);
					return;
				}
				computer.spline.points.CopyTo(morphStates[index].points, 0);
				Update();
			}

			public void CaptureSnapshot(string name)
			{
				int channelIndex = GetChannelIndex(name);
				CaptureSnapshot(channelIndex);
			}

			public void Clear()
			{
				morphStates = new SplineMorphState[0];
			}

			public SplinePoint[] GetSnapshot(int index)
			{
				return morphStates[index].points;
			}

			public SplinePoint[] GetSnapshot(string name)
			{
				int channelIndex = GetChannelIndex(name);
				return morphStates[channelIndex].points;
			}

			public float GetWeight(int index)
			{
				return morphStates[index].percent;
			}

			public float GetWeight(string name)
			{
				int channelIndex = GetChannelIndex(name);
				return morphStates[channelIndex].percent;
			}

			public void AddChannel(string name)
			{
				if (morphStates.Length > 0 && computer.pointCount != morphStates[0].points.Length)
				{
					UnityEngine.Debug.LogError("Point count must be the same as " + computer.pointCount);
					return;
				}
				SplineMorphState splineMorphState = new SplineMorphState();
				splineMorphState.points = computer.GetPoints(Space.Local);
				splineMorphState.name = name;
				if (morphStates.Length > 0)
				{
					splineMorphState.percent = 0f;
				}
				SplineMorphState[] array = new SplineMorphState[morphStates.Length + 1];
				morphStates.CopyTo(array, 0);
				morphStates = array;
				morphStates[morphStates.Length - 1] = splineMorphState;
			}

			public void RemoveChannel(string name)
			{
				int channelIndex = GetChannelIndex(name);
				RemoveChannel(channelIndex);
			}

			public void RemoveChannel(int index)
			{
				if (index < 0 || index >= morphStates.Length)
				{
					return;
				}
				SplineMorphState[] array = new SplineMorphState[morphStates.Length - 1];
				for (int i = 0; i < morphStates.Length; i++)
				{
					if (i != index)
					{
						if (i < index)
						{
							array[i] = morphStates[i];
						}
						else if (i >= index)
						{
							array[i - 1] = morphStates[i];
						}
					}
				}
				morphStates = array;
			}

			private void Update()
			{
				if (morphStates.Length == 0)
				{
					return;
				}
				for (int i = 0; i < computer.pointCount; i++)
				{
					Vector3 vector = morphStates[0].points[i].position;
					Vector3 vector2 = morphStates[0].points[i].tangent;
					Vector3 vector3 = morphStates[0].points[i].tangent2;
					Vector3 vector4 = morphStates[0].points[i].normal;
					Color color = morphStates[0].points[i].color;
					float num = morphStates[0].points[i].size;
					for (int j = 1; j < morphStates.Length; j++)
					{
						vector += (morphStates[j].points[i].position - morphStates[0].points[i].position) * morphStates[j].percent;
						vector2 += (morphStates[j].points[i].tangent - morphStates[0].points[i].tangent) * morphStates[j].percent;
						vector3 += (morphStates[j].points[i].tangent2 - morphStates[0].points[i].tangent2) * morphStates[j].percent;
						vector4 += (morphStates[j].points[i].normal - morphStates[0].points[i].normal) * morphStates[j].percent;
						color += (morphStates[j].points[i].color - morphStates[0].points[i].color) * morphStates[j].percent;
						num += (morphStates[j].points[i].size - morphStates[0].points[i].size) * morphStates[j].percent;
					}
					SplinePoint point = computer.GetPoint(i, Space.Local);
					point.type = SplinePoint.Type.Broken;
					point.position = vector;
					point.tangent = vector2;
					point.tangent2 = vector3;
					point.normal = vector4;
					point.color = color;
					point.size = num;
					computer.SetPoint(i, point, Space.Local);
				}
			}

			private int GetChannelIndex(string name)
			{
				for (int i = 0; i < morphStates.Length; i++)
				{
					if (morphStates[i].name == name)
					{
						return i;
					}
				}
				return 0;
			}

			public int GetChannelCount()
			{
				if (morphStates == null)
				{
					return 0;
				}
				return morphStates.Length;
			}

			public string[] GetChannelNames()
			{
				if (morphStates == null)
				{
					return new string[0];
				}
				string[] array = new string[morphStates.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = morphStates[i].name;
				}
				return array;
			}
		}

		[HideInInspector]
		[SerializeField]
		private Spline spline = new Spline(Spline.Type.Hermite);

		[HideInInspector]
		[SerializeField]
		private Morph _morph;

		[HideInInspector]
		[SerializeField]
		private Space _space = Space.Local;

		[HideInInspector]
		[SerializeField]
		private SplineUser[] subscribers = new SplineUser[0];

		[HideInInspector]
		[SerializeField]
		private NodeLink[] _nodeLinks = new NodeLink[0];

		private bool rebuildPending;

		private TS_Transform tsTransform;

		private bool updateRebuild;

		private bool lateUpdateRebuild;

		private SplineUser.UpdateMethod method;

		public Space space
		{
			get
			{
				return _space;
			}
			set
			{
				if (value != _space)
				{
					Rebuild();
				}
				_space = value;
			}
		}

		public Spline.Type type
		{
			get
			{
				return spline.type;
			}
			set
			{
				if (value != spline.type)
				{
					spline.type = value;
					Rebuild();
				}
				else
				{
					spline.type = value;
				}
			}
		}

		public double precision
		{
			get
			{
				return spline.precision;
			}
			set
			{
				if (value != spline.precision)
				{
					if (value >= 1.0)
					{
						value = 0.99999;
					}
					spline.precision = value;
					Rebuild();
				}
				else
				{
					spline.precision = value;
				}
			}
		}

		public AnimationCurve customValueInterpolation
		{
			get
			{
				return spline.customValueInterpolation;
			}
			set
			{
				spline.customValueInterpolation = value;
				Rebuild();
			}
		}

		public AnimationCurve customNormalInterpolation
		{
			get
			{
				return spline.customNormalInterpolation;
			}
			set
			{
				spline.customNormalInterpolation = value;
				Rebuild();
			}
		}

		public int iterations => spline.iterations;

		public double moveStep => spline.moveStep;

		public bool isClosed => spline.isClosed;

		public int pointCount => spline.points.Length;

		public Morph morph
		{
			get
			{
				if (_morph == null)
				{
					_morph = new Morph(this);
				}
				else if (!_morph.initialized)
				{
					_morph = new Morph(this);
				}
				return _morph;
			}
		}

		public NodeLink[] nodeLinks => _nodeLinks;

		public bool hasMorph
		{
			get
			{
				if (_morph == null)
				{
					return false;
				}
				return _morph.GetChannelCount() > 0;
			}
		}

		public Vector3 position
		{
			get
			{
				if (tsTransform == null)
				{
					return base.transform.position;
				}
				return tsTransform.position;
			}
		}

		public Quaternion rotation
		{
			get
			{
				if (tsTransform == null)
				{
					return base.transform.rotation;
				}
				return tsTransform.rotation;
			}
		}

		public Vector3 scale
		{
			get
			{
				if (tsTransform == null)
				{
					return base.transform.localScale;
				}
				return tsTransform.scale;
			}
		}

		public int subscriberCount => subscribers.Length;

		public event EmptySplineHandler onRebuild;

		private void Awake()
		{
			tsTransform = new TS_Transform(base.transform);
		}

		private void LateUpdate()
		{
			method = SplineUser.UpdateMethod.LateUpdate;
			Run();
			if (lateUpdateRebuild)
			{
				RebuildOnUpdate();
			}
			lateUpdateRebuild = false;
		}

		private void Update()
		{
			method = SplineUser.UpdateMethod.Update;
			Run();
			if (updateRebuild)
			{
				RebuildOnUpdate();
			}
			updateRebuild = false;
		}

		private void Run()
		{
			if (tsTransform.HasChange())
			{
				updateRebuild = true;
				lateUpdateRebuild = true;
				if (_nodeLinks.Length > 0)
				{
					UpdateConnectedNodes();
				}
				ResampleTransform();
			}
		}

		private void OnEnable()
		{
			if (rebuildPending)
			{
				rebuildPending = false;
				Rebuild();
			}
		}

		public void ResampleTransform()
		{
			tsTransform.Update();
		}

		public void Subscribe(SplineUser input)
		{
			int num = -1;
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (subscribers[i] == input)
				{
					return;
				}
				if (subscribers[i] == null && num < 0)
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				subscribers[num] = input;
				return;
			}
			SplineUser[] array = new SplineUser[subscribers.Length + 1];
			subscribers.CopyTo(array, 0);
			array[subscribers.Length] = input;
			subscribers = array;
		}

		public void Unsubscribe(SplineUser input)
		{
			int num = 0;
			while (true)
			{
				if (num < subscribers.Length)
				{
					if (subscribers[num] == input)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			SplineUser[] array = new SplineUser[subscribers.Length - 1];
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (i < num)
				{
					array[i] = subscribers[i];
				}
				else if (i > num)
				{
					array[i - 1] = subscribers[i];
				}
			}
			subscribers = array;
		}

		public bool IsSubscribed(SplineUser user)
		{
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (subscribers[i] == user)
				{
					return true;
				}
			}
			return false;
		}

		public SplineUser[] GetSubscribers()
		{
			return subscribers;
		}

		public void AddNodeLink(Node node, int pointIndex)
		{
			if (node == null || pointIndex < 0 || pointIndex >= spline.points.Length)
			{
				return;
			}
			for (int i = 0; i < nodeLinks.Length; i++)
			{
				if (nodeLinks[i].node == node && nodeLinks[i].pointIndex == pointIndex)
				{
					UnityEngine.Debug.LogError("Junction already exists, cannot add junction " + node.name + " at point " + pointIndex);
					return;
				}
			}
			if (!node.HasConnection(this, pointIndex))
			{
				UnityEngine.Debug.LogError("Junction " + node.name + " does not have a connection for point " + pointIndex + " Call AddConnection on the junction.");
				return;
			}
			NodeLink nodeLink = new NodeLink();
			nodeLink.node = node;
			nodeLink.pointIndex = pointIndex;
			NodeLink[] array = new NodeLink[_nodeLinks.Length + 1];
			_nodeLinks.CopyTo(array, 0);
			array[_nodeLinks.Length] = nodeLink;
			_nodeLinks = array;
		}

		public void RemoveNodeLink(int pointIndex)
		{
			int num = -1;
			for (int i = 0; i < _nodeLinks.Length; i++)
			{
				if (_nodeLinks[i].pointIndex == pointIndex)
				{
					if (_nodeLinks[i].node != null && _nodeLinks[i].node.HasConnection(this, pointIndex))
					{
						_nodeLinks[i].node.RemoveConnection(this, pointIndex);
						return;
					}
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				RemoveNodeLinkAt(num);
			}
		}

		public SplinePoint[] GetPoints(Space getSpace = Space.World)
		{
			SplinePoint[] array = new SplinePoint[spline.points.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = spline.points[i];
				if (_space == Space.Local && getSpace == Space.World)
				{
					array[i].position = TransformPoint(array[i].position);
					array[i].tangent = TransformPoint(array[i].tangent);
					array[i].tangent2 = TransformPoint(array[i].tangent2);
					array[i].normal = TransformDirection(array[i].normal);
				}
			}
			return array;
		}

		public SplinePoint GetPoint(int index, Space getSpace = Space.World)
		{
			if (index < 0 || index >= spline.points.Length)
			{
				return default(SplinePoint);
			}
			if (_space == Space.Local && getSpace == Space.World)
			{
				SplinePoint result = spline.points[index];
				result.position = TransformPoint(result.position);
				result.tangent = TransformPoint(result.tangent);
				result.tangent2 = TransformPoint(result.tangent2);
				result.normal = TransformDirection(result.normal);
				return result;
			}
			return spline.points[index];
		}

		public Vector3 GetPointPosition(int index, Space getSpace = Space.World)
		{
			if (_space == Space.Local && getSpace == Space.World)
			{
				return TransformPoint(spline.points[index].position);
			}
			return spline.points[index].position;
		}

		public Vector3 GetPointNormal(int index, Space getSpace = Space.World)
		{
			if (_space == Space.Local && getSpace == Space.World)
			{
				return TransformDirection(spline.points[index].normal).normalized;
			}
			return spline.points[index].normal;
		}

		public Vector3 GetPointTangent(int index, Space getSpace = Space.World)
		{
			if (_space == Space.Local && getSpace == Space.World)
			{
				return TransformPoint(spline.points[index].tangent);
			}
			return spline.points[index].tangent;
		}

		public Vector3 GetPointTangent2(int index, Space getSpace = Space.World)
		{
			if (_space == Space.Local && getSpace == Space.World)
			{
				return TransformPoint(spline.points[index].tangent2);
			}
			return spline.points[index].tangent2;
		}

		public float GetPointSize(int index, Space getSpace = Space.World)
		{
			return spline.points[index].size;
		}

		public Color GetPointColor(int index, Space getSpace = Space.World)
		{
			return spline.points[index].color;
		}

		public void SetPoints(SplinePoint[] points, Space setSpace = Space.World)
		{
			bool flag = false;
			if (points.Length != spline.points.Length)
			{
				flag = true;
				if (points.Length < 4)
				{
					Break();
				}
				spline.points = new SplinePoint[points.Length];
			}
			for (int i = 0; i < points.Length; i++)
			{
				SplinePoint splinePoint = points[i];
				if (_space == Space.Local && setSpace == Space.World)
				{
					splinePoint.position = InverseTransformPoint(points[i].position);
					splinePoint.tangent = InverseTransformPoint(points[i].tangent);
					splinePoint.tangent2 = InverseTransformPoint(points[i].tangent2);
					splinePoint.normal = InverseTransformDirection(points[i].normal);
				}
				if (!flag)
				{
					if (splinePoint.position != spline.points[i].position)
					{
						flag = true;
					}
					else if (splinePoint.tangent != spline.points[i].tangent)
					{
						flag = true;
					}
					else if (splinePoint.tangent2 != spline.points[i].tangent2)
					{
						flag = true;
					}
					else if (splinePoint.size != spline.points[i].size)
					{
						flag = true;
					}
					else if (splinePoint.type != spline.points[i].type)
					{
						flag = true;
					}
					else if (splinePoint.color != spline.points[i].color)
					{
						flag = true;
					}
					else if (splinePoint.normal != spline.points[i].normal)
					{
						flag = true;
					}
				}
				spline.points[i] = splinePoint;
			}
			if (flag)
			{
				Rebuild();
				UpdateConnectedNodes(points);
			}
		}

		public void SetPointPosition(int index, Vector3 pos, Space setSpace = Space.World)
		{
			if (index >= 0)
			{
				if (index >= spline.points.Length)
				{
					AppendPoints(index + 1 - spline.points.Length);
				}
				Vector3 vector = pos;
				if (_space == Space.Local && setSpace == Space.World)
				{
					vector = InverseTransformPoint(pos);
				}
				if (vector != spline.points[index].position)
				{
					spline.points[index].position = vector;
					Rebuild();
					SetNodeForPoint(index, GetPoint(index));
				}
			}
		}

		public void SetPointTangents(int index, Vector3 tan1, Vector3 tan2, Space setSpace = Space.World)
		{
			if (index >= 0)
			{
				if (index >= spline.points.Length)
				{
					AppendPoints(index + 1 - spline.points.Length);
				}
				Vector3 vector = tan1;
				Vector3 vector2 = tan2;
				if (_space == Space.Local && setSpace == Space.World)
				{
					vector = InverseTransformPoint(tan1);
					vector2 = InverseTransformPoint(tan2);
				}
				bool flag = false;
				if (vector2 != spline.points[index].tangent2)
				{
					flag = true;
					spline.points[index].SetTangent2Position(vector2);
				}
				if (vector != spline.points[index].tangent)
				{
					flag = true;
					spline.points[index].SetTangentPosition(vector);
				}
				if (flag)
				{
					Rebuild();
					SetNodeForPoint(index, GetPoint(index));
				}
			}
		}

		public void SetPointNormal(int index, Vector3 nrm, Space setSpace = Space.World)
		{
			if (index >= 0)
			{
				if (index >= spline.points.Length)
				{
					AppendPoints(index + 1 - spline.points.Length);
				}
				Vector3 vector = nrm;
				if (_space == Space.Local && setSpace == Space.World)
				{
					vector = InverseTransformDirection(nrm);
				}
				if (vector != spline.points[index].normal)
				{
					spline.points[index].normal = vector;
					Rebuild();
					SetNodeForPoint(index, GetPoint(index));
				}
			}
		}

		public void SetPointSize(int index, float size)
		{
			if (index >= 0)
			{
				if (index >= spline.points.Length)
				{
					AppendPoints(index + 1 - spline.points.Length);
				}
				if (size != spline.points[index].size)
				{
					spline.points[index].size = size;
					Rebuild();
					SetNodeForPoint(index, GetPoint(index));
				}
			}
		}

		public void SetPointColor(int index, Color color)
		{
			if (index >= 0)
			{
				if (index >= spline.points.Length)
				{
					AppendPoints(index + 1 - spline.points.Length);
				}
				if (color != spline.points[index].color)
				{
					spline.points[index].color = color;
					Rebuild();
					SetNodeForPoint(index, GetPoint(index));
				}
			}
		}

		public void SetPoint(int index, SplinePoint point, Space setSpace = Space.World)
		{
			if (index >= 0)
			{
				if (index >= spline.points.Length)
				{
					AppendPoints(index + 1 - spline.points.Length);
				}
				bool flag = false;
				SplinePoint splinePoint = point;
				if (_space == Space.Local && setSpace == Space.World)
				{
					splinePoint.position = InverseTransformPoint(point.position);
					splinePoint.tangent = InverseTransformPoint(point.tangent);
					splinePoint.tangent2 = InverseTransformPoint(point.tangent2);
					splinePoint.normal = InverseTransformDirection(point.normal);
				}
				if (splinePoint.position != spline.points[index].position)
				{
					flag = true;
				}
				else if (splinePoint.tangent != spline.points[index].tangent)
				{
					flag = true;
				}
				else if (splinePoint.tangent2 != spline.points[index].tangent2)
				{
					flag = true;
				}
				else if (splinePoint.size != spline.points[index].size)
				{
					flag = true;
				}
				else if (splinePoint.type != spline.points[index].type)
				{
					flag = true;
				}
				else if (splinePoint.color != spline.points[index].color)
				{
					flag = true;
				}
				else if (splinePoint.normal != spline.points[index].normal)
				{
					flag = true;
				}
				spline.points[index] = splinePoint;
				if (flag)
				{
					Rebuild();
					SetNodeForPoint(index, point);
				}
			}
		}

		private void AppendPoints(int count)
		{
			SplinePoint[] array = new SplinePoint[spline.points.Length + count];
			spline.points.CopyTo(array, 0);
			spline.points = array;
			Rebuild();
		}

		public Vector3 EvaluatePosition(double percent)
		{
			Vector3 vector = spline.EvaluatePosition(percent);
			if (_space == Space.Local)
			{
				vector = TransformPoint(vector);
			}
			return vector;
		}

		public SplineResult Evaluate(double percent)
		{
			SplineResult result = new SplineResult();
			Evaluate(result, percent);
			return result;
		}

		public void Evaluate(SplineResult result, double percent)
		{
			spline.Evaluate(result, percent);
			if (_space == Space.Local)
			{
				TransformResult(result);
			}
		}

		public void Evaluate(ref SplineResult[] samples, double from = 0.0, double to = 1.0)
		{
			spline.Evaluate(ref samples, from, to);
			if (_space == Space.Local)
			{
				for (int i = 0; i < samples.Length; i++)
				{
					TransformResult(samples[i]);
				}
			}
		}

		public void EvaluatePositions(ref Vector3[] positions, double from = 0.0, double to = 1.0)
		{
			spline.EvaluatePositions(ref positions, from, to);
			if (_space == Space.Local)
			{
				for (int i = 0; i < positions.Length; i++)
				{
					positions[i] = TransformPoint(positions[i]);
				}
			}
		}

		public double Travel(double start, float distance, Spline.Direction direction)
		{
			if (pointCount <= 1)
			{
				return 0.0;
			}
			if (direction == Spline.Direction.Forward && start >= 1.0)
			{
				return 1.0;
			}
			if (direction == Spline.Direction.Backward && start <= 0.0)
			{
				return 0.0;
			}
			if (distance == 0f)
			{
				return DMath.Clamp01(start);
			}
			float num = 0f;
			Vector3 b = EvaluatePosition(start);
			double a = start;
			int num2 = iterations - 1;
			int num3 = (direction != Spline.Direction.Forward) ? DMath.FloorInt(start * (double)num2) : DMath.CeilInt(start * (double)num2);
			float num4 = 0f;
			Vector3 zero = Vector3.zero;
			double num5;
			while (true)
			{
				num5 = (double)num3 / (double)num2;
				zero = EvaluatePosition(num5);
				num4 = Vector3.Distance(zero, b);
				b = zero;
				num += num4;
				if (num >= distance)
				{
					break;
				}
				a = num5;
				if (direction == Spline.Direction.Forward)
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

		private void TransformResult(SplineResult result)
		{
			result.position = TransformPoint(result.position);
			result.direction = TransformDirection(result.direction);
			result.normal = TransformDirection(result.normal);
		}

		public void Rebuild()
		{
			updateRebuild = true;
			lateUpdateRebuild = true;
		}

		public void RebuildImmediate()
		{
			if (Application.isPlaying)
			{
				ResampleTransform();
			}
			for (int num = subscribers.Length - 1; num >= 0; num--)
			{
				if (subscribers[num] != null)
				{
					if (subscribers[num].computer != this)
					{
						RemoveSubscriber(num);
					}
					else
					{
						subscribers[num].RebuildImmediate(sampleComputer: true);
					}
				}
				else
				{
					RemoveSubscriber(num);
				}
			}
			if (this.onRebuild != null)
			{
				this.onRebuild();
			}
		}

		private void RemoveSubscriber(int index)
		{
			SplineUser[] array = new SplineUser[subscribers.Length - 1];
			for (int i = 0; i < subscribers.Length; i++)
			{
				if (i != index)
				{
					if (i < index)
					{
						array[i] = subscribers[i];
					}
					else
					{
						array[i - 1] = subscribers[i];
					}
				}
			}
			subscribers = array;
		}

		private void RebuildOnUpdate()
		{
			for (int num = subscribers.Length - 1; num >= 0; num--)
			{
				if (subscribers[num] != null)
				{
					if (subscribers[num].computer != this)
					{
						RemoveSubscriber(num);
					}
					else
					{
						subscribers[num].Rebuild(sampleComputer: true);
					}
				}
				else
				{
					RemoveSubscriber(num);
				}
			}
			if (this.onRebuild != null)
			{
				this.onRebuild();
			}
		}

		public void RebuildConnectedUsers()
		{
		}

		private void RebuildUser(int index)
		{
			if (index < subscribers.Length)
			{
				if (subscribers[index] == null)
				{
					RemoveSubscriber(index);
				}
				else if (method == subscribers[index].updateMethod)
				{
					subscribers[index].Rebuild(sampleComputer: true);
				}
				else if (method == SplineUser.UpdateMethod.Update && subscribers[index].updateMethod == SplineUser.UpdateMethod.FixedUpdate)
				{
					subscribers[index].Rebuild(sampleComputer: true);
				}
			}
		}

		public double Project(Vector3 point, int subdivide = 3, double from = 0.0, double to = 1.0)
		{
			if (_space == Space.Local)
			{
				point = InverseTransformPoint(point);
			}
			return spline.Project(point, subdivide, from, to);
		}

		public void Break()
		{
			Break(0);
		}

		public void Break(int at)
		{
			if (spline.isClosed)
			{
				spline.Break(at);
				Rebuild();
			}
		}

		public void Close()
		{
			if (!spline.isClosed)
			{
				spline.Close();
				Rebuild();
			}
		}

		public void ConvertToBezier()
		{
			spline.ConvertToBezier();
		}

		public float CalculateLength(double from = 0.0, double to = 1.0, double resolution = 1.0)
		{
			if (pointCount <= 1)
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

		public bool Raycast(out RaycastHit hit, out double hitPercent, LayerMask layerMask, double resolution = 1.0, double from = 0.0, double to = 1.0, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal)
		{
			resolution = DMath.Clamp01(resolution);
			from = DMath.Clamp01(from);
			to = DMath.Clamp01(to);
			double num = from;
			Vector3 vector = EvaluatePosition(num);
			hitPercent = 0.0;
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

		public int[] GetAvailableNodeLinksAtPosition(double percent, Spline.Direction direction)
		{
			List<int> list = new List<int>();
			double num = (double)(pointCount - 1) * percent;
			for (int i = 0; i < _nodeLinks.Length; i++)
			{
				if (direction == Spline.Direction.Forward)
				{
					if ((double)_nodeLinks[i].pointIndex >= num)
					{
						list.Add(i);
					}
				}
				else if ((double)_nodeLinks[i].pointIndex <= num)
				{
					list.Add(i);
				}
			}
			return list.ToArray();
		}

		public void GetConnectedComputers(List<SplineComputer> computers, List<int> connectionIndices, List<int> connectedIndices, double percent, Spline.Direction direction, bool includeEqual)
		{
			if (computers == null)
			{
				computers = new List<SplineComputer>();
			}
			if (connectionIndices == null)
			{
				connectionIndices = new List<int>();
			}
			if (connectedIndices == null)
			{
				connectionIndices = new List<int>();
			}
			computers.Clear();
			connectionIndices.Clear();
			connectedIndices.Clear();
			int num = Mathf.FloorToInt((float)(pointCount - 1) * (float)percent);
			for (int i = 0; i < _nodeLinks.Length; i++)
			{
				bool flag = false;
				if (!(includeEqual ? ((direction != Spline.Direction.Forward) ? (_nodeLinks[i].pointIndex <= num) : (_nodeLinks[i].pointIndex >= num)) : ((direction != Spline.Direction.Forward) ? (_nodeLinks[i].pointIndex < num) : (_nodeLinks[i].pointIndex > num))))
				{
					continue;
				}
				Node.Connection[] connections = _nodeLinks[i].node.GetConnections();
				for (int j = 0; j < connections.Length; j++)
				{
					if (connections[j].computer != this)
					{
						computers.Add(connections[j].computer);
						connectionIndices.Add(_nodeLinks[i].pointIndex);
						connectedIndices.Add(connections[j].pointIndex);
					}
				}
			}
		}

		public void SetMorphState(int index)
		{
			if (!hasMorph)
			{
				return;
			}
			for (int i = 0; i < _morph.GetChannelCount(); i++)
			{
				if (i != index)
				{
					_morph.SetWeight(i, 0f);
				}
				else
				{
					_morph.SetWeight(i, 1f);
				}
			}
		}

		public void SetMorphState(string morphName)
		{
			if (!hasMorph)
			{
				return;
			}
			string[] channelNames = _morph.GetChannelNames();
			for (int i = 0; i < channelNames.Length; i++)
			{
				if (channelNames[i] == morphName)
				{
					SetMorphState(i);
					return;
				}
			}
			UnityEngine.Debug.LogError("Morph state " + morphName + " not found");
		}

		public void SetMorphState(int index, float percent)
		{
			if (!hasMorph)
			{
				return;
			}
			percent = Mathf.Clamp01(percent);
			float max = 1f - percent;
			for (int i = 0; i < _morph.GetChannelCount(); i++)
			{
				if (i != index)
				{
					float weight = _morph.GetWeight(i);
					weight = Mathf.Clamp(weight, 0f, max);
					_morph.SetWeight(i, weight);
				}
				else
				{
					_morph.SetWeight(i, percent);
				}
			}
		}

		public void SetMorphState(string morphName, float percent)
		{
			if (!hasMorph)
			{
				return;
			}
			string[] channelNames = _morph.GetChannelNames();
			for (int i = 0; i < channelNames.Length; i++)
			{
				if (channelNames[i] == morphName)
				{
					SetMorphState(i, percent);
					return;
				}
			}
			UnityEngine.Debug.LogError("Morph state " + morphName + " not found");
		}

		public void SetMorphState(float percent)
		{
			if (!hasMorph)
			{
				return;
			}
			int channelCount = _morph.GetChannelCount();
			float num = percent * (float)(channelCount - 1);
			for (int i = 0; i < channelCount; i++)
			{
				float num2 = Mathf.Abs((float)i - num);
				if (num2 > 1f)
				{
					_morph.SetWeight(i, 0f);
				}
				else if (num <= (float)i)
				{
					_morph.SetWeight(i, 1f - ((float)i - num));
				}
				else
				{
					_morph.SetWeight(i, 1f - (num - (float)i));
				}
			}
		}

		public List<SplineComputer> GetConnectedComputers()
		{
			List<SplineComputer> computers = new List<SplineComputer>();
			computers.Add(this);
			if (nodeLinks.Length == 0)
			{
				return computers;
			}
			GetConnectedComputers(ref computers);
			return computers;
		}

		private void GetConnectedComputers(ref List<SplineComputer> computers)
		{
			SplineComputer splineComputer = computers[computers.Count - 1];
			if (splineComputer == null)
			{
				return;
			}
			for (int i = 0; i < splineComputer._nodeLinks.Length; i++)
			{
				if (splineComputer._nodeLinks[i].node == null)
				{
					continue;
				}
				Node.Connection[] connections = splineComputer._nodeLinks[i].node.GetConnections();
				for (int j = 0; j < connections.Length; j++)
				{
					bool flag = false;
					if (connections[j].computer == this)
					{
						continue;
					}
					for (int k = 0; k < computers.Count; k++)
					{
						if (computers[k] == connections[j].computer)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						computers.Add(connections[j].computer);
						GetConnectedComputers(ref computers);
					}
				}
			}
		}

		private void RemoveNodeLinkAt(int index)
		{
			for (int i = 0; i < subscribers.Length; i++)
			{
				for (int j = 0; j < subscribers[i].address.depth; j++)
				{
					if (subscribers[i].address.elements[j].computer == this && (subscribers[i].address.elements[j].startPoint == nodeLinks[index].pointIndex || subscribers[i].address.elements[j].endPoint == nodeLinks[index].pointIndex))
					{
						subscribers[i].ExitAddress(subscribers[i].address.depth - j);
						break;
					}
				}
			}
			NodeLink[] array = new NodeLink[_nodeLinks.Length - 1];
			for (int k = 0; k < _nodeLinks.Length; k++)
			{
				if (k != index)
				{
					if (k < index)
					{
						array[k] = _nodeLinks[k];
					}
					else
					{
						array[k - 1] = _nodeLinks[k];
					}
				}
			}
			_nodeLinks = array;
		}

		private void SetNodeForPoint(int index, SplinePoint worldPoint)
		{
			int num = 0;
			while (true)
			{
				if (num < _nodeLinks.Length)
				{
					if (_nodeLinks[num].pointIndex == index)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_nodeLinks[num].node.UpdatePoint(this, _nodeLinks[num].pointIndex, worldPoint);
		}

		private void UpdateConnectedNodes(SplinePoint[] worldPoints)
		{
			for (int i = 0; i < _nodeLinks.Length; i++)
			{
				if (_nodeLinks[i].node == null)
				{
					RemoveNodeLinkAt(i);
					i--;
					Rebuild();
					continue;
				}
				bool flag = false;
				Node.Connection[] connections = _nodeLinks[i].node.GetConnections();
				foreach (Node.Connection connection in connections)
				{
					if (connection.computer == this)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					RemoveNodeLinkAt(i);
					i--;
					Rebuild();
				}
				else
				{
					_nodeLinks[i].node.UpdatePoint(this, _nodeLinks[i].pointIndex, worldPoints[_nodeLinks[i].pointIndex]);
					_nodeLinks[i].node.UpdateConnectedComputers(this);
				}
			}
		}

		private void UpdateConnectedNodes()
		{
			for (int i = 0; i < _nodeLinks.Length; i++)
			{
				if (_nodeLinks[i].node == null)
				{
					RemoveNodeLinkAt(i);
					Rebuild();
					i--;
				}
				else
				{
					_nodeLinks[i].node.UpdatePoint(this, _nodeLinks[i].pointIndex, GetPoint(_nodeLinks[i].pointIndex));
					_nodeLinks[i].node.UpdateConnectedComputers(this);
				}
			}
		}

		private Vector3 TransformPoint(Vector3 point)
		{
			if (tsTransform != null && tsTransform.transform != null)
			{
				return tsTransform.TransformPoint(point);
			}
			return base.transform.TransformPoint(point);
		}

		private Vector3 InverseTransformPoint(Vector3 point)
		{
			if (tsTransform != null && tsTransform.transform != null)
			{
				return tsTransform.InverseTransformPoint(point);
			}
			return base.transform.InverseTransformPoint(point);
		}

		private Vector3 TransformDirection(Vector3 direction)
		{
			if (tsTransform != null && tsTransform.transform != null)
			{
				return tsTransform.TransformDirection(direction);
			}
			return base.transform.TransformDirection(direction);
		}

		private Vector3 InverseTransformDirection(Vector3 direction)
		{
			if (tsTransform != null && tsTransform.transform != null)
			{
				return tsTransform.InverseTransformDirection(direction);
			}
			return base.transform.InverseTransformDirection(direction);
		}
	}
}
