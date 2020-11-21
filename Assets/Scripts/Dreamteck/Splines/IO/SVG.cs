using Dreamteck.Splines.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace Dreamteck.Splines.IO
{
	public class SVG : SplineParser
	{
		public enum Axis
		{
			X,
			Y,
			Z
		}

		internal class PathSegment
		{
			internal enum Type
			{
				Cubic,
				CubicShort,
				Quadratic,
				QuadraticShort
			}

			internal Vector3 startTangent = Vector3.zero;

			internal Vector3 endTangent = Vector3.zero;

			internal Vector3 endPoint = Vector3.zero;

			internal PathSegment(Vector2 s, Vector2 e, Vector2 c)
			{
				startTangent = s;
				endTangent = e;
				endPoint = c;
			}

			internal PathSegment()
			{
			}
		}

		public enum Element
		{
			All,
			Path,
			Polygon,
			Ellipse,
			Rectangle,
			Line
		}

		private List<SplineDefinition> paths = new List<SplineDefinition>();

		private List<SplineDefinition> polygons = new List<SplineDefinition>();

		private List<SplineDefinition> ellipses = new List<SplineDefinition>();

		private List<SplineDefinition> rectangles = new List<SplineDefinition>();

		private List<SplineDefinition> lines = new List<SplineDefinition>();

		private List<Transformation> transformBuffer = new List<Transformation>();

		public SVG(string filePath)
		{
			if (File.Exists(filePath))
			{
				string a = Path.GetExtension(filePath).ToLower();
				fileName = Path.GetFileNameWithoutExtension(filePath);
				if (a != ".svg" && a != ".xml")
				{
					UnityEngine.Debug.LogError("SVG Parsing ERROR: Wrong format. Please use SVG or XML");
					return;
				}
				XmlDocument xmlDocument = new XmlDocument
				{
					XmlResolver = null
				};
				try
				{
					xmlDocument.Load(filePath);
				}
				catch (XmlException ex)
				{
					UnityEngine.Debug.LogError(ex.Message);
					return;
				}
				Read(xmlDocument);
			}
		}

		public SVG(List<SplineComputer> computers)
		{
			paths = new List<SplineDefinition>(computers.Count);
			for (int i = 0; i < computers.Count; i++)
			{
				if (!(computers[i] == null))
				{
					Spline spline = new Spline(computers[i].type, computers[i].precision)
					{
						points = computers[i].GetPoints()
					};
					if (spline.type != Spline.Type.Bezier && spline.type != Spline.Type.Linear)
					{
						spline.ConvertToBezier();
					}
					if (computers[i].isClosed)
					{
						spline.Close();
					}
					paths.Add(new SplineDefinition(computers[i].name, spline));
				}
			}
		}

		public void Write(string filePath, Axis ax = Axis.Z)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("svg");
			foreach (SplineDefinition path in paths)
			{
				string name = "path";
				string name2 = "d";
				if (path.type == Spline.Type.Linear)
				{
					name2 = "points";
					name = ((!path.closed) ? "polyline" : "polygon");
				}
				XmlElement xmlElement2 = xmlDocument.CreateElement(name);
				XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("id");
				xmlAttribute.Value = path.name;
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute(name2);
				if (path.type == Spline.Type.Linear)
				{
					xmlAttribute.Value = EncodePolygon(path, ax);
				}
				else
				{
					xmlAttribute.Value = EncodePath(path, ax);
				}
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute("stroke");
				xmlAttribute.Value = "black";
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute("stroke-width");
				xmlAttribute.Value = "3";
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlAttribute = xmlDocument.CreateAttribute("fill");
				xmlAttribute.Value = "none";
				xmlElement2.Attributes.Append(xmlAttribute);
				xmlElement.AppendChild(xmlElement2);
			}
			XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("version");
			xmlAttribute2.Value = "1.1";
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlAttribute2 = xmlDocument.CreateAttribute("xmlns");
			xmlAttribute2.Value = "http://www.w3.org/2000/svg";
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlDocument.AppendChild(xmlElement);
			xmlDocument.Save(filePath);
		}

		private Vector2 MapPoint(Vector3 original, Axis ax)
		{
			switch (ax)
			{
			case Axis.X:
				return new Vector2(original.z, 0f - original.y);
			case Axis.Y:
				return new Vector2(original.x, 0f - original.z);
			case Axis.Z:
				return new Vector2(original.x, 0f - original.y);
			default:
				return original;
			}
		}

		private void Read(XmlDocument doc)
		{
			transformBuffer.Clear();
			Traverse(doc.ChildNodes);
		}

		private void Traverse(XmlNodeList nodes)
		{
			IEnumerator enumerator = nodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode xmlNode = (XmlNode)enumerator.Current;
					int num = 0;
					switch (xmlNode.Name)
					{
					case "g":
						num = ParseTransformation(xmlNode);
						break;
					case "path":
						num = ReadPath(xmlNode);
						break;
					case "polygon":
						num = ReadPolygon(xmlNode, closed: true);
						break;
					case "polyline":
						num = ReadPolygon(xmlNode, closed: false);
						break;
					case "ellipse":
						num = ReadEllipse(xmlNode);
						break;
					case "circle":
						num = ReadEllipse(xmlNode);
						break;
					case "line":
						num = ReadLine(xmlNode);
						break;
					case "rect":
						num = ReadRectangle(xmlNode);
						break;
					}
					Traverse(xmlNode.ChildNodes);
					if (num > 0)
					{
						transformBuffer.RemoveRange(transformBuffer.Count - num, num);
					}
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

		public List<SplineComputer> CreateSplineComputers(Vector3 position, Quaternion rotation, Element elements = Element.All)
		{
			List<SplineComputer> list = new List<SplineComputer>();
			if (elements == Element.All || elements == Element.Path)
			{
				foreach (SplineDefinition path in paths)
				{
					list.Add(path.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == Element.All || elements == Element.Polygon)
			{
				foreach (SplineDefinition polygon in polygons)
				{
					list.Add(polygon.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == Element.All || elements == Element.Ellipse)
			{
				foreach (SplineDefinition ellipsis in ellipses)
				{
					list.Add(ellipsis.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == Element.All || elements == Element.Rectangle)
			{
				foreach (SplineDefinition rectangle in rectangles)
				{
					list.Add(rectangle.CreateSplineComputer(position, rotation));
				}
			}
			if (elements == Element.All || elements == Element.Line)
			{
				foreach (SplineDefinition line in lines)
				{
					list.Add(line.CreateSplineComputer(position, rotation));
				}
				return list;
			}
			return list;
		}

		public List<Spline> CreateSplines(Element elements = Element.All)
		{
			List<Spline> list = new List<Spline>();
			if (elements == Element.All || elements == Element.Path)
			{
				foreach (SplineDefinition path in paths)
				{
					list.Add(path.CreateSpline());
				}
			}
			if (elements == Element.All || elements == Element.Polygon)
			{
				foreach (SplineDefinition polygon in polygons)
				{
					list.Add(polygon.CreateSpline());
				}
			}
			if (elements == Element.All || elements == Element.Ellipse)
			{
				foreach (SplineDefinition ellipsis in ellipses)
				{
					list.Add(ellipsis.CreateSpline());
				}
			}
			if (elements == Element.All || elements == Element.Rectangle)
			{
				foreach (SplineDefinition rectangle in rectangles)
				{
					list.Add(rectangle.CreateSpline());
				}
			}
			if (elements == Element.All || elements == Element.Line)
			{
				foreach (SplineDefinition line in lines)
				{
					list.Add(line.CreateSpline());
				}
				return list;
			}
			return list;
		}

		private int ReadRectangle(XmlNode rectNode)
		{
			float result = 0f;
			float result2 = 0f;
			float result3 = 0f;
			float result4 = 0f;
			float result5 = -1f;
			float result6 = -1f;
			string attributeContent = GetAttributeContent(rectNode, "x");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result);
			attributeContent = GetAttributeContent(rectNode, "y");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result2);
			attributeContent = GetAttributeContent(rectNode, "width");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result3);
			attributeContent = GetAttributeContent(rectNode, "height");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result4);
			attributeContent = GetAttributeContent(rectNode, "rx");
			if (attributeContent != "ERROR")
			{
				float.TryParse(attributeContent, out result5);
			}
			attributeContent = GetAttributeContent(rectNode, "ry");
			if (attributeContent != "ERROR")
			{
				float.TryParse(attributeContent, out result6);
			}
			else
			{
				result6 = result5;
			}
			string text = GetAttributeContent(rectNode, "id");
			if (result5 == -1f && result6 == -1f)
			{
				Rectangle rectangle = new Rectangle();
				rectangle.offset = new Vector2(result + result3 / 2f, 0f - result2 - result4 / 2f);
				rectangle.axis = SplinePrimitive.Axis.nZ;
				rectangle.size = new Vector2(result3, result4);
				if (text == "ERROR")
				{
					text = fileName + "_rectangle" + (rectangles.Count + 1);
				}
				buffer = new SplineDefinition(text, rectangle.GetSpline());
			}
			else
			{
				RoundedRectangle roundedRectangle = new RoundedRectangle();
				roundedRectangle.offset = new Vector2(result + result3 / 2f, 0f - result2 - result4 / 2f);
				roundedRectangle.axis = SplinePrimitive.Axis.nZ;
				roundedRectangle.size = new Vector2(result3, result4);
				roundedRectangle.xRadius = result5;
				roundedRectangle.yRadius = result6;
				if (text == "ERROR")
				{
					text = fileName + "_roundedRectangle" + (rectangles.Count + 1);
				}
				buffer = new SplineDefinition(text, roundedRectangle.GetSpline());
			}
			int result7 = ParseTransformation(rectNode);
			WriteBufferTo(rectangles);
			return result7;
		}

		private int ReadLine(XmlNode lineNode)
		{
			float result = 0f;
			float result2 = 0f;
			float result3 = 0f;
			float result4 = 0f;
			string attributeContent = GetAttributeContent(lineNode, "x1");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result);
			attributeContent = GetAttributeContent(lineNode, "y1");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result2);
			attributeContent = GetAttributeContent(lineNode, "x2");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result3);
			attributeContent = GetAttributeContent(lineNode, "y2");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result4);
			string text = GetAttributeContent(lineNode, "id");
			if (text == "ERROR")
			{
				text = fileName + "_line" + (ellipses.Count + 1);
			}
			buffer = new SplineDefinition(text, Spline.Type.Linear);
			buffer.position = new Vector2(result, 0f - result2);
			buffer.CreateLinear();
			buffer.position = new Vector2(result3, 0f - result4);
			buffer.CreateLinear();
			int result5 = ParseTransformation(lineNode);
			WriteBufferTo(lines);
			return result5;
		}

		private int ReadEllipse(XmlNode ellipseNode)
		{
			float result = 0f;
			float result2 = 0f;
			float result3 = 0f;
			float result4 = 0f;
			string attributeContent = GetAttributeContent(ellipseNode, "cx");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result);
			attributeContent = GetAttributeContent(ellipseNode, "cy");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			float.TryParse(attributeContent, out result2);
			attributeContent = GetAttributeContent(ellipseNode, "r");
			string text = "circle";
			if (attributeContent == "ERROR")
			{
				text = "ellipse";
				attributeContent = GetAttributeContent(ellipseNode, "rx");
				if (attributeContent == "ERROR")
				{
					return 0;
				}
				float.TryParse(attributeContent, out result3);
				attributeContent = GetAttributeContent(ellipseNode, "ry");
				if (attributeContent == "ERROR")
				{
					return 0;
				}
			}
			else
			{
				float.TryParse(attributeContent, out result3);
				result4 = result3;
			}
			float.TryParse(attributeContent, out result4);
			Ellipse ellipse = new Ellipse();
			ellipse.offset = new Vector2(result, 0f - result2);
			ellipse.axis = SplinePrimitive.Axis.nZ;
			ellipse.xRadius = result3;
			ellipse.yRadius = result4;
			string text2 = GetAttributeContent(ellipseNode, "id");
			if (text2 == "ERROR")
			{
				text2 = fileName + "_" + text + (ellipses.Count + 1);
			}
			buffer = new SplineDefinition(text2, ellipse.GetSpline());
			int result5 = ParseTransformation(ellipseNode);
			WriteBufferTo(ellipses);
			return result5;
		}

		private int ReadPolygon(XmlNode polyNode, bool closed)
		{
			string attributeContent = GetAttributeContent(polyNode, "points");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			List<float> list = ParseFloatArray(attributeContent);
			if (list.Count % 2 != 0)
			{
				UnityEngine.Debug.LogWarning("There is an error with one of the polygon shapes.");
				return 0;
			}
			string text = GetAttributeContent(polyNode, "id");
			if (text == "ERROR")
			{
				text = fileName + ((!closed) ? "_polyline" : "_polygon ") + (polygons.Count + 1);
			}
			buffer = new SplineDefinition(text, Spline.Type.Linear);
			int num = list.Count / 2;
			for (int i = 0; i < num; i++)
			{
				buffer.position = new Vector2(list[2 * i], 0f - list[1 + 2 * i]);
				buffer.CreateLinear();
			}
			if (closed)
			{
				buffer.CreateClosingPoint();
				buffer.closed = true;
			}
			int result = ParseTransformation(polyNode);
			WriteBufferTo(polygons);
			return result;
		}

		private int ParseTransformation(XmlNode node)
		{
			string attributeContent = GetAttributeContent(node, "transform");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			List<Transformation> list = ParseTransformations(attributeContent);
			transformBuffer.AddRange(list);
			return list.Count;
		}

		private List<Transformation> ParseTransformations(string transformContent)
		{
			List<Transformation> list = new List<Transformation>();
			MatchCollection matchCollection = Regex.Matches(transformContent.ToLower(), "(?<function>translate|rotate|scale|skewx|skewy|matrix)\\s*\\((\\s*(?<param>-?\\s*\\d+(\\.\\d+)?)\\s*\\,*\\s*)+\\)");
			IEnumerator enumerator = matchCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Match match = (Match)enumerator.Current;
					if (match.Groups["function"].Success)
					{
						CaptureCollection captures = match.Groups["param"].Captures;
						switch (match.Groups["function"].Value)
						{
						case "translate":
							if (captures.Count >= 2)
							{
								list.Add(new Translate(new Vector2(float.Parse(captures[0].Value), float.Parse(captures[1].Value))));
							}
							break;
						case "rotate":
							if (captures.Count >= 1)
							{
								list.Add(new Rotate(float.Parse(captures[0].Value)));
							}
							break;
						case "scale":
							if (captures.Count >= 2)
							{
								list.Add(new Scale(new Vector2(float.Parse(captures[0].Value), float.Parse(captures[1].Value))));
							}
							break;
						case "skewx":
							if (captures.Count >= 1)
							{
								list.Add(new SkewX(float.Parse(captures[0].Value)));
							}
							break;
						case "skewy":
							if (captures.Count >= 1)
							{
								list.Add(new SkewY(float.Parse(captures[0].Value)));
							}
							break;
						case "matrix":
							if (captures.Count >= 6)
							{
								list.Add(new MatrixTransform(float.Parse(captures[0].Value), float.Parse(captures[1].Value), float.Parse(captures[2].Value), float.Parse(captures[3].Value), float.Parse(captures[4].Value), float.Parse(captures[5].Value)));
							}
							break;
						}
					}
				}
				return list;
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

		private int ReadPath(XmlNode pathNode)
		{
			string attributeContent = GetAttributeContent(pathNode, "d");
			if (attributeContent == "ERROR")
			{
				return 0;
			}
			string text = GetAttributeContent(pathNode, "id");
			if (text == "ERROR")
			{
				text = fileName + "_path " + (paths.Count + 1);
			}
			IEnumerable<string> enumerable = from t in Regex.Split(attributeContent, "(?=[A-Za-z])")
				where !string.IsNullOrEmpty(t)
				select t;
			foreach (string item in enumerable)
			{
				switch (item.Substring(0, 1).Single())
				{
				case 'M':
					PathStart(text, item, relative: false);
					break;
				case 'm':
					PathStart(text, item, relative: true);
					break;
				case 'Z':
					PathClose();
					break;
				case 'z':
					PathClose();
					break;
				case 'L':
					PathLineTo(item, relative: false);
					break;
				case 'l':
					PathLineTo(item, relative: true);
					break;
				case 'H':
					PathHorizontalLineTo(item, relative: false);
					break;
				case 'h':
					PathHorizontalLineTo(item, relative: true);
					break;
				case 'V':
					PathVerticalLineTo(item, relative: false);
					break;
				case 'v':
					PathVerticalLineTo(item, relative: true);
					break;
				case 'C':
					PathCurveTo(item, PathSegment.Type.Cubic, relative: false);
					break;
				case 'c':
					PathCurveTo(item, PathSegment.Type.Cubic, relative: true);
					break;
				case 'S':
					PathCurveTo(item, PathSegment.Type.CubicShort, relative: false);
					break;
				case 's':
					PathCurveTo(item, PathSegment.Type.CubicShort, relative: true);
					break;
				case 'Q':
					PathCurveTo(item, PathSegment.Type.Quadratic, relative: false);
					break;
				case 'q':
					PathCurveTo(item, PathSegment.Type.Quadratic, relative: true);
					break;
				case 'T':
					PathCurveTo(item, PathSegment.Type.QuadraticShort, relative: false);
					break;
				case 't':
					PathCurveTo(item, PathSegment.Type.QuadraticShort, relative: true);
					break;
				}
			}
			int result = ParseTransformation(pathNode);
			if (buffer != null)
			{
				WriteBufferTo(paths);
			}
			return result;
		}

		private void PathStart(string name, string coords, bool relative)
		{
			if (buffer != null)
			{
				WriteBufferTo(paths);
			}
			buffer = new SplineDefinition(name, Spline.Type.Bezier);
			Vector2[] array = ParseVector2(coords);
			Vector2[] array2 = array;
			foreach (Vector3 vector in array2)
			{
				if (relative)
				{
					buffer.position += vector;
				}
				else
				{
					buffer.position = vector;
				}
				buffer.CreateLinear();
			}
		}

		private void PathClose()
		{
			buffer.closed = true;
		}

		private void PathLineTo(string coords, bool relative)
		{
			Vector2[] array = ParseVector2(coords);
			Vector2[] array2 = array;
			foreach (Vector3 vector in array2)
			{
				if (relative)
				{
					buffer.position += vector;
				}
				else
				{
					buffer.position = vector;
				}
				buffer.CreateLinear();
			}
		}

		private void PathHorizontalLineTo(string coords, bool relative)
		{
			float[] array = ParseFloat(coords);
			float[] array2 = array;
			foreach (float num in array2)
			{
				if (relative)
				{
					buffer.position.x += num;
				}
				else
				{
					buffer.position.x = num;
				}
				buffer.CreateLinear();
			}
		}

		private void PathVerticalLineTo(string coords, bool relative)
		{
			float[] array = ParseFloat(coords);
			float[] array2 = array;
			foreach (float num in array2)
			{
				if (relative)
				{
					buffer.position.y -= num;
				}
				else
				{
					buffer.position.y = 0f - num;
				}
				buffer.CreateLinear();
			}
		}

		private void PathCurveTo(string coords, PathSegment.Type type, bool relative)
		{
			PathSegment[] array = ParsePathSegment(coords, type);
			for (int i = 0; i < array.Length; i++)
			{
				SplinePoint lastPoint = buffer.GetLastPoint();
				lastPoint.type = SplinePoint.Type.Broken;
				Vector3 position = lastPoint.position;
				Vector3 endPoint = array[i].endPoint;
				Vector3 vector = array[i].startTangent;
				Vector3 vector2 = array[i].endTangent;
				switch (type)
				{
				case PathSegment.Type.CubicShort:
					vector = position - lastPoint.tangent;
					break;
				case PathSegment.Type.Quadratic:
					buffer.tangent = array[i].startTangent;
					vector = position + 2f / 3f * (buffer.tangent - position);
					vector2 = endPoint + 2f / 3f * (buffer.tangent - endPoint);
					break;
				case PathSegment.Type.QuadraticShort:
				{
					Vector3 a = position + (position - buffer.tangent);
					vector = position + 2f / 3f * (a - position);
					vector2 = endPoint + 2f / 3f * (a - endPoint);
					break;
				}
				}
				if (type == PathSegment.Type.CubicShort || type == PathSegment.Type.QuadraticShort)
				{
					lastPoint.type = SplinePoint.Type.SmoothMirrored;
				}
				else if (relative)
				{
					lastPoint.SetTangent2Position(position + vector);
				}
				else
				{
					lastPoint.SetTangent2Position(vector);
				}
				buffer.SetLastPoint(lastPoint);
				if (relative)
				{
					buffer.position += endPoint;
					buffer.tangent = position + vector2;
				}
				else
				{
					buffer.position = endPoint;
					buffer.tangent = vector2;
				}
				buffer.CreateBroken();
			}
		}

		private void WriteBufferTo(List<SplineDefinition> list)
		{
			buffer.Transform(transformBuffer);
			list.Add(buffer);
			buffer = null;
		}

		private PathSegment[] ParsePathSegment(string coord, PathSegment.Type type)
		{
			List<float> list = ParseFloatArray(coord.Substring(1));
			int num = 0;
			switch (type)
			{
			case PathSegment.Type.Cubic:
				num = list.Count / 6;
				break;
			case PathSegment.Type.Quadratic:
				num = list.Count / 4;
				break;
			case PathSegment.Type.CubicShort:
				num = list.Count / 4;
				break;
			case PathSegment.Type.QuadraticShort:
				num = list.Count / 2;
				break;
			}
			if (num == 0)
			{
				return new PathSegment[1]
				{
					new PathSegment()
				};
			}
			PathSegment[] array = new PathSegment[num];
			for (int i = 0; i < num; i++)
			{
				switch (type)
				{
				case PathSegment.Type.Cubic:
					array[i] = new PathSegment(new Vector2(list[6 * i], 0f - list[1 + 6 * i]), new Vector2(list[2 + 6 * i], 0f - list[3 + 6 * i]), new Vector2(list[4 + 6 * i], 0f - list[5 + 6 * i]));
					break;
				case PathSegment.Type.Quadratic:
					array[i] = new PathSegment(new Vector2(list[4 * i], 0f - list[1 + 4 * i]), Vector2.zero, new Vector2(list[2 + 4 * i], 0f - list[3 + 4 * i]));
					break;
				case PathSegment.Type.CubicShort:
					array[i] = new PathSegment(Vector2.zero, new Vector2(list[4 * i], 0f - list[1 + 4 * i]), new Vector2(list[2 + 4 * i], 0f - list[3 + 4 * i]));
					break;
				case PathSegment.Type.QuadraticShort:
					array[i] = new PathSegment(Vector2.zero, Vector2.zero, new Vector2(list[4 * i], 0f - list[1 + 4 * i]));
					break;
				}
			}
			return array;
		}

		private string EncodePath(SplineDefinition definition, Axis ax)
		{
			string text = "M";
			for (int i = 0; i < definition.pointCount; i++)
			{
				SplinePoint splinePoint = definition.points[i];
				Vector3 vector = MapPoint(splinePoint.tangent, ax);
				Vector3 vector2 = MapPoint(splinePoint.position, ax);
				string text2;
				if (i == 0)
				{
					text2 = text;
					text = text2 + vector2.x + "," + vector2.y;
					continue;
				}
				SplinePoint splinePoint2 = definition.points[i - 1];
				Vector3 vector3 = MapPoint(splinePoint2.tangent2, ax);
				text2 = text;
				text = text2 + "C" + vector3.x + "," + vector3.y + "," + vector.x + "," + vector.y + "," + vector2.x + "," + vector2.y;
			}
			if (definition.closed)
			{
				text += "z";
			}
			return text;
		}

		private string EncodePolygon(SplineDefinition definition, Axis ax)
		{
			string text = string.Empty;
			for (int i = 0; i < definition.pointCount; i++)
			{
				SplinePoint splinePoint = definition.points[i];
				Vector3 vector = MapPoint(splinePoint.position, ax);
				if (text != string.Empty)
				{
					text += ",";
				}
				string text2 = text;
				text = text2 + vector.x + "," + vector.y;
			}
			return text;
		}

		private string GetAttributeContent(XmlNode node, string attributeName)
		{
			for (int i = 0; i < node.Attributes.Count; i++)
			{
				if (node.Attributes[i].Name == attributeName)
				{
					return node.Attributes[i].InnerText;
				}
			}
			return "ERROR";
		}
	}
}
