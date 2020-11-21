using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	[Serializable]
	public class SplineAddress
	{
		[Serializable]
		public class Element
		{
			public SplineComputer computer;

			[SerializeField]
			private int _startPoint;

			[SerializeField]
			private int _endPoint = -1;

			public int startPoint
			{
				get
				{
					return _startPoint;
				}
				set
				{
					if (value < 0)
					{
						value = 0;
					}
					if (value >= computer.pointCount)
					{
						UnityEngine.Debug.LogError("Out of bounds point index setting. Tried setting index to " + value + " when computer only has " + computer.pointCount);
					}
					else
					{
						_startPoint = value;
					}
				}
			}

			public int endPoint
			{
				get
				{
					if (_endPoint < 0)
					{
						return computer.pointCount - 1;
					}
					return _endPoint;
				}
				set
				{
					if (computer == null)
					{
						_endPoint = -1;
					}
					else if (value >= computer.pointCount)
					{
						UnityEngine.Debug.LogError("Out of bounds point index setting. Tried setting index to " + value + " when computer only has " + computer.pointCount);
					}
					else
					{
						_endPoint = value;
					}
				}
			}

			public double startPercent => (double)startPoint / (double)(computer.pointCount - 1);

			public double endPercent => (double)endPoint / (double)(computer.pointCount - 1);

			public int span
			{
				get
				{
					if (endPoint < 0)
					{
						return computer.pointCount - 1 - startPoint;
					}
					return Mathf.Abs(endPoint - startPoint);
				}
			}
		}

		public Element[] _elements = new Element[0];

		public int depth
		{
			get
			{
				if (_elements == null)
				{
					return 0;
				}
				return _elements.Length;
			}
		}

		public SplineComputer root
		{
			get
			{
				if (_elements == null)
				{
					return null;
				}
				if (_elements.Length == 0)
				{
					return null;
				}
				if (_elements[0] == null)
				{
					return null;
				}
				return _elements[0].computer;
			}
			set
			{
				_elements = new Element[1];
				_elements[0] = new Element();
				_elements[0].computer = value;
				_elements[0].endPoint = -1;
			}
		}

		public Element[] elements => _elements;

		public double moveStep
		{
			get
			{
				if (root == null)
				{
					return 1.0;
				}
				if (root.pointCount < 2)
				{
					return 1.0;
				}
				double num = (double)(root.pointCount - 1) / (double)(GetTotalPointCount() - 1);
				return root.moveStep * num;
			}
		}

		public SplineAddress(SplineComputer rootComp)
		{
			_elements = new Element[1];
			_elements[0] = new Element();
			_elements[0].computer = root;
		}

		public SplineAddress(SplineAddress copy)
		{
			if (copy != null && copy.depth != 0)
			{
				for (int i = 0; i < copy.elements.Length; i++)
				{
					AddElement(copy.elements[i]);
				}
			}
		}

		public SplineResult Evaluate(double percent)
		{
			if (root == null)
			{
				return null;
			}
			double percent2 = 0.0;
			GetEvaluationValues(percent, out SplineComputer computer, out percent2, out Spline.Direction _);
			if (computer == null)
			{
				return null;
			}
			SplineResult splineResult = computer.Evaluate(percent2);
			splineResult.percent = percent;
			return splineResult;
		}

		public Vector3 EvaluatePosition(double percent)
		{
			if (root == null)
			{
				return Vector3.zero;
			}
			if (_elements.Length == 1)
			{
				return _elements[0].computer.EvaluatePosition(percent);
			}
			double num = 0.0;
			for (int i = 0; i < _elements.Length; i++)
			{
				num += (double)_elements[i].span;
			}
			double num2 = 0.0;
			for (int j = 0; j < _elements.Length; j++)
			{
				double num3 = (double)_elements[j].span / num;
				num2 += num3;
				if (num2 >= percent || Mathf.Approximately((float)num2, (float)percent))
				{
					double percent2 = DMath.Lerp(_elements[j].startPercent, _elements[j].endPercent, DMath.InverseLerp(num2 - num3, num2, percent));
					return _elements[j].computer.EvaluatePosition(percent2);
				}
			}
			return Vector3.zero;
		}

		public double Project(Vector3 point, int subdivide = 4, double from = 0.0, double to = 1.0)
		{
			if (root == null)
			{
				return 0.0;
			}
			if (to > 1.0)
			{
				to = 1.0;
			}
			if (from > to)
			{
				from = to;
			}
			if (from < 0.0)
			{
				from = 0.0;
			}
			float num = float.PositiveInfinity;
			int elementIndex = 0;
			double localPercent = 0.0;
			for (int i = 0; i < _elements.Length; i++)
			{
				double from2 = PathToLocalPercent(from, i);
				double to2 = PathToLocalPercent(to, i);
				double num2 = _elements[i].computer.Project(point, subdivide, from2, to2);
				Vector3 a = _elements[i].computer.EvaluatePosition(num2);
				float sqrMagnitude = (a - point).sqrMagnitude;
				if (i == 0 || sqrMagnitude < num)
				{
					num = sqrMagnitude;
					elementIndex = i;
					localPercent = num2;
				}
			}
			return LocalToPathPercent(localPercent, elementIndex);
		}

		public float CalculateLength(double from = 0.0, double to = 1.0)
		{
			if (root == null)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < _elements.Length; i++)
			{
				double from2 = PathToLocalPercent(from, i);
				double to2 = PathToLocalPercent(to, i);
				num += _elements[i].computer.CalculateLength(from2, to2);
			}
			return num;
		}

		public double Travel(double start, float distance, Spline.Direction direction, int iterations)
		{
			if (GetTotalPointCount() <= 1)
			{
				return 0.0;
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

		public int GetElementIndex(double percent)
		{
			if (root == null)
			{
				return 0;
			}
			int num = 0;
			Element[] elements = _elements;
			foreach (Element element in elements)
			{
				num += element.span;
			}
			double num2 = 0.0;
			for (int j = 0; j < _elements.Length; j++)
			{
				double num3 = (double)_elements[j].span / (double)num;
				num2 += num3;
				if (num2 >= percent || Mathf.Approximately((float)num2, (float)percent))
				{
					return j;
				}
			}
			return 0;
		}

		public double PathToLocalPercent(double pathPercent, int elementIndex)
		{
			if (root == null)
			{
				return 0.0;
			}
			int num = 0;
			Element[] elements = _elements;
			foreach (Element element in elements)
			{
				num += element.span;
			}
			double num2 = 0.0;
			for (int j = 0; j < _elements.Length; j++)
			{
				double num3 = (double)_elements[j].span / (double)num;
				num2 += num3;
				if (j == elementIndex)
				{
					num2 -= num3;
					return DMath.Lerp(_elements[j].startPercent, _elements[j].endPercent, DMath.InverseLerp(num2, num2 + num3, pathPercent));
				}
			}
			return 0.0;
		}

		public double LocalToPathPercent(double localPercent, int elementIndex)
		{
			if (root == null)
			{
				return 0.0;
			}
			int num = 0;
			Element[] elements = _elements;
			foreach (Element element in elements)
			{
				num += element.span;
			}
			double num2 = 0.0;
			for (int j = 0; j < _elements.Length; j++)
			{
				double num3 = (double)_elements[j].span / (double)num;
				num2 += num3;
				if (j == elementIndex)
				{
					num2 -= num3;
					double num4 = DMath.InverseLerp(_elements[j].startPercent, _elements[j].endPercent, localPercent);
					return num2 + num3 * num4;
				}
			}
			return 0.0;
		}

		public int GetTotalPointCount()
		{
			if (root == null)
			{
				return 0;
			}
			if (_elements.Length == 1)
			{
				return root.pointCount;
			}
			int num = 0;
			for (int i = 0; i < _elements.Length; i++)
			{
				num += _elements[i].span + 1;
				if (i > 0)
				{
					num--;
				}
			}
			return num;
		}

		public void GetEvaluationValues(double inputPercent, out SplineComputer computer, out double percent, out Spline.Direction direction)
		{
			computer = null;
			percent = 0.0;
			direction = Spline.Direction.Forward;
			if (root == null)
			{
				return;
			}
			int num = 0;
			Element[] elements = _elements;
			foreach (Element element in elements)
			{
				num += element.span;
			}
			double num2 = 0.0;
			for (int j = 0; j < _elements.Length; j++)
			{
				if (num2 > inputPercent)
				{
					break;
				}
				double num3 = (double)_elements[j].span / (double)num;
				percent = DMath.Lerp(_elements[j].startPercent, _elements[j].endPercent, DMath.InverseLerp(num2, num2 + num3, inputPercent));
				computer = _elements[j].computer;
				num2 += num3;
			}
		}

		private int LocalToPathPoint(int point, int elementIndex)
		{
			if (root == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < elementIndex; i++)
			{
				num += _elements[i].span;
			}
			num -= elementIndex;
			return num + point;
		}

		private void PathToLocalPoint(int point, out int computerIndex, out int localPoint)
		{
			int num = 0;
			localPoint = 0;
			computerIndex = 0;
			if (root == null)
			{
				return;
			}
			for (int i = 0; i < _elements.Length; i++)
			{
				num += _elements[i].span;
				if (num == point)
				{
					computerIndex = i;
					localPoint = point - num;
				}
			}
		}

		[Obsolete("Enter is obsolete, use AddSpline instead")]
		public void Enter(Node node, int connectionIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			AddSpline(node, connectionIndex, direction);
		}

		public void AddSpline(Node node, int connectionIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			if (root == null)
			{
				return;
			}
			Node.Connection[] connections = node.GetConnections();
			Element element = new Element();
			Node.Connection[] array = connections;
			foreach (Node.Connection connection in array)
			{
				if (connection.computer == _elements[_elements.Length - 1].computer && ((connection.pointIndex >= _elements[_elements.Length - 1].startPoint && connection.pointIndex <= _elements[_elements.Length - 1].endPoint) || (connection.pointIndex >= _elements[_elements.Length - 1].endPoint && connection.pointIndex <= _elements[_elements.Length - 1].startPoint)))
				{
					if (_elements[_elements.Length - 1].startPoint < 0)
					{
						_elements[_elements.Length - 1].startPoint = 0;
					}
					_elements[_elements.Length - 1].endPoint = connection.pointIndex;
					element.computer = connections[connectionIndex].computer;
					element.startPoint = connections[connectionIndex].pointIndex;
					if (direction == Spline.Direction.Backward)
					{
						element.endPoint = 0;
					}
					AddElement(element);
					return;
				}
			}
			UnityEngine.Debug.LogError("Connection not valid. Node must have computer " + _elements[_elements.Length - 1].computer.name + " in order to connect");
		}

		public void AddSpline(SplineComputer computer, int connectionIndex, int connectedIndex, Spline.Direction direction = Spline.Direction.Forward)
		{
			if (!(root == null))
			{
				if (connectedIndex < 0 || connectedIndex >= computer.pointCount)
				{
					throw new Exception("Invalid spline point index " + connectedIndex + ". Index must be in the range [" + 0 + "-" + computer.pointCount + "]");
				}
				if (_elements[_elements.Length - 1].startPoint < 0)
				{
					_elements[_elements.Length - 1].startPoint = 0;
				}
				_elements[_elements.Length - 1].endPoint = connectionIndex;
				Element element = new Element();
				element.computer = computer;
				element.startPoint = connectedIndex;
				if (direction == Spline.Direction.Backward)
				{
					element.endPoint = 0;
				}
				AddElement(element);
			}
		}

		public void Exit(int exitDepth)
		{
			int num = _elements.Length - exitDepth;
			if (num < 1)
			{
				num = 1;
			}
			Element[] array = new Element[num];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = _elements[i];
			}
			_elements = array;
			if (_elements[_elements.Length - 1].endPoint >= _elements[_elements.Length - 1].startPoint)
			{
				_elements[_elements.Length - 1].endPoint = -1;
			}
			else
			{
				_elements[_elements.Length - 1].endPoint = 0;
			}
		}

		public void Collapse()
		{
			SplineComputer computer = _elements[_elements.Length - 1].computer;
			int startPoint = _elements[_elements.Length - 1].startPoint;
			int endPoint = _elements[_elements.Length - 1].endPoint;
			_elements = new Element[1];
			_elements[0] = new Element();
			_elements[0].computer = computer;
			_elements[0].startPoint = startPoint;
			_elements[0].endPoint = endPoint;
		}

		public void Clear()
		{
			Element element = _elements[0];
			_elements = new Element[1];
			_elements[0] = element;
			_elements[0].endPoint = -1;
		}

		private void AddElement(Element element)
		{
			Element[] array = new Element[_elements.Length + 1];
			_elements.CopyTo(array, 0);
			array[array.Length - 1] = element;
			_elements = array;
		}
	}
}
