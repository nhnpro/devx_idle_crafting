using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	public class Node : MonoBehaviour
	{
		[Serializable]
		public class Connection
		{
			public bool invertTangents;

			[SerializeField]
			private int _pointIndex;

			[SerializeField]
			private SplineComputer _computer;

			[SerializeField]
			[HideInInspector]
			internal SplinePoint point;

			public SplineComputer computer => _computer;

			public int pointIndex => _pointIndex;

			internal bool isValid
			{
				get
				{
					if (_computer == null)
					{
						return false;
					}
					if (_pointIndex >= _computer.pointCount)
					{
						return false;
					}
					return true;
				}
			}

			internal Connection(SplineComputer comp, int index, SplinePoint inputPoint)
			{
				_pointIndex = index;
				_computer = comp;
				point = inputPoint;
			}
		}

		public enum Type
		{
			Smooth,
			Free
		}

		public Type type;

		[SerializeField]
		protected Connection[] connections = new Connection[0];

		[SerializeField]
		private bool _transformSize = true;

		[SerializeField]
		private bool _transformNormals = true;

		[SerializeField]
		private bool _transformTangents = true;

		private TS_Transform tsTransform;

		public bool transformNormals
		{
			get
			{
				return _transformNormals;
			}
			set
			{
				if (value != _transformNormals)
				{
					_transformNormals = value;
					UpdatePoints();
				}
			}
		}

		public bool transformSize
		{
			get
			{
				return _transformSize;
			}
			set
			{
				if (value != _transformSize)
				{
					_transformSize = value;
					UpdatePoints();
				}
			}
		}

		public bool transformTangents
		{
			get
			{
				return _transformTangents;
			}
			set
			{
				if (value != _transformTangents)
				{
					_transformTangents = value;
					UpdatePoints();
				}
			}
		}

		private void Awake()
		{
			tsTransform = new TS_Transform(base.transform);
		}

		private void LateUpdate()
		{
			Run();
		}

		private void Update()
		{
			Run();
		}

		private void Run()
		{
			if (tsTransform.HasChange())
			{
				UpdateConnectedComputers();
				tsTransform.Update();
			}
		}

		public SplinePoint GetPoint(int connectionIndex, bool swapTangents)
		{
			SplinePoint result = PointToWorld(connections[connectionIndex].point);
			if (connections[connectionIndex].invertTangents && swapTangents)
			{
				Vector3 tangent = result.tangent;
				result.tangent = result.tangent2;
				result.tangent2 = tangent;
			}
			return result;
		}

		public void SetPoint(int connectionIndex, SplinePoint worldPoint, bool swappedTangents)
		{
			Connection connection = connections[connectionIndex];
			connection.point = PointToLocal(worldPoint);
			if (connection.invertTangents && swappedTangents)
			{
				Vector3 tangent = connection.point.tangent;
				connection.point.tangent = connection.point.tangent2;
				connection.point.tangent2 = tangent;
			}
			if (type != 0)
			{
				return;
			}
			if (connection.point.type == SplinePoint.Type.SmoothFree)
			{
				for (int i = 0; i < connections.Length; i++)
				{
					if (i != connectionIndex)
					{
						Vector3 vector = (connection.point.tangent - connection.point.position).normalized;
						if (vector == Vector3.zero)
						{
							vector = -(connection.point.tangent2 - connection.point.position).normalized;
						}
						float magnitude = (connections[i].point.tangent - connections[i].point.position).magnitude;
						float magnitude2 = (connections[i].point.tangent2 - connections[i].point.position).magnitude;
						connections[i].point = connection.point;
						connections[i].point.tangent = connections[i].point.position + vector * magnitude;
						connections[i].point.tangent2 = connections[i].point.position - vector * magnitude2;
					}
				}
				return;
			}
			for (int j = 0; j < connections.Length; j++)
			{
				if (j != connectionIndex)
				{
					connections[j].point = connection.point;
				}
			}
		}

		private void OnDestroy()
		{
			ClearConnections();
		}

		public void ClearConnections()
		{
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].computer != null)
				{
					connections[i].computer.RemoveNodeLink(connections[i].pointIndex);
				}
			}
			connections = new Connection[0];
		}

		public void UpdateConnectedComputers(SplineComputer excludeComputer = null)
		{
			for (int num = connections.Length - 1; num >= 0; num--)
			{
				if (!connections[num].isValid)
				{
					RemoveConnection(num);
				}
				else if (!(connections[num].computer == excludeComputer))
				{
					if (type == Type.Smooth && num != 0)
					{
						SetPoint(num, GetPoint(0, swapTangents: false), swappedTangents: false);
					}
					SplinePoint point = GetPoint(num, swapTangents: true);
					if (!transformNormals)
					{
						point.normal = connections[num].computer.GetPointNormal(connections[num].pointIndex);
					}
					if (!transformTangents)
					{
						point.tangent = connections[num].computer.GetPointTangent(connections[num].pointIndex);
						point.tangent2 = connections[num].computer.GetPointTangent2(connections[num].pointIndex);
					}
					if (!transformSize)
					{
						point.size = connections[num].computer.GetPointSize(connections[num].pointIndex);
					}
					connections[num].computer.SetPoint(connections[num].pointIndex, point);
				}
			}
		}

		public void UpdatePoint(SplineComputer computer, int pointIndex, SplinePoint point, bool updatePosition = true)
		{
			tsTransform.position = point.position;
			tsTransform.Update();
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].computer == computer && connections[i].pointIndex == pointIndex)
				{
					SetPoint(i, point, swappedTangents: true);
				}
			}
		}

		private void UpdatePoints()
		{
			for (int num = connections.Length - 1; num >= 0; num--)
			{
				if (!connections[num].isValid)
				{
					RemoveConnection(num);
				}
				else
				{
					SplinePoint point = connections[num].computer.GetPoint(connections[num].pointIndex);
					point.SetPosition(base.transform.position);
					SetPoint(num, point, swappedTangents: true);
				}
			}
		}

		protected void RemoveInvalidConnections()
		{
			for (int num = connections.Length - 1; num >= 0; num--)
			{
				if (connections[num] == null || !connections[num].isValid)
				{
					RemoveConnection(num);
				}
			}
		}

		public virtual void AddConnection(SplineComputer computer, int pointIndex)
		{
			RemoveInvalidConnections();
			for (int i = 0; i < computer.nodeLinks.Length; i++)
			{
				if (computer.nodeLinks[i].pointIndex == pointIndex)
				{
					UnityEngine.Debug.LogError("Connection has been already added in " + computer.nodeLinks[i].node);
					return;
				}
			}
			SplinePoint point = computer.GetPoint(pointIndex);
			point.SetPosition(base.transform.position);
			Connection connection = new Connection(computer, pointIndex, PointToLocal(point));
			Connection[] array = new Connection[connections.Length + 1];
			connections.CopyTo(array, 0);
			array[connections.Length] = connection;
			connections = array;
			SetPoint(connections.Length - 1, point, swappedTangents: true);
			computer.AddNodeLink(this, pointIndex);
			UpdateConnectedComputers();
		}

		protected SplinePoint PointToLocal(SplinePoint worldPoint)
		{
			worldPoint.position = Vector3.zero;
			worldPoint.tangent = base.transform.InverseTransformPoint(worldPoint.tangent);
			worldPoint.tangent2 = base.transform.InverseTransformPoint(worldPoint.tangent2);
			worldPoint.normal = base.transform.InverseTransformDirection(worldPoint.normal);
			float size = worldPoint.size;
			Vector3 localScale = base.transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = base.transform.localScale;
			float num = x + localScale2.y;
			Vector3 localScale3 = base.transform.localScale;
			worldPoint.size = size / ((num + localScale3.z) / 3f);
			return worldPoint;
		}

		protected SplinePoint PointToWorld(SplinePoint localPoint)
		{
			localPoint.position = base.transform.position;
			localPoint.tangent = base.transform.TransformPoint(localPoint.tangent);
			localPoint.tangent2 = base.transform.TransformPoint(localPoint.tangent2);
			localPoint.normal = base.transform.TransformDirection(localPoint.normal);
			float size = localPoint.size;
			Vector3 localScale = base.transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = base.transform.localScale;
			float num = x + localScale2.y;
			Vector3 localScale3 = base.transform.localScale;
			localPoint.size = size * ((num + localScale3.z) / 3f);
			return localPoint;
		}

		public virtual void RemoveConnection(SplineComputer computer, int pointIndex)
		{
			int num = -1;
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].computer == computer && connections[i].pointIndex == pointIndex)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				UnityEngine.Debug.LogError("Connection not found in " + base.name);
			}
			else
			{
				RemoveConnection(num);
			}
		}

		private void RemoveConnection(int index)
		{
			Connection[] array = new Connection[connections.Length - 1];
			SplineComputer computer = connections[index].computer;
			int pointIndex = connections[index].pointIndex;
			for (int i = 0; i < connections.Length; i++)
			{
				if (i < index)
				{
					array[i] = connections[i];
				}
				else if (i != index)
				{
					array[i - 1] = connections[i];
				}
			}
			connections = array;
			if (computer != null)
			{
				computer.RemoveNodeLink(pointIndex);
			}
		}

		public virtual bool HasConnection(SplineComputer computer, int pointIndex)
		{
			for (int num = connections.Length - 1; num >= 0; num--)
			{
				if (!connections[num].isValid)
				{
					RemoveConnection(num);
				}
				else if (connections[num].computer == computer && connections[num].pointIndex == pointIndex)
				{
					return true;
				}
			}
			return false;
		}

		public Connection[] GetConnections()
		{
			return connections;
		}
	}
}
