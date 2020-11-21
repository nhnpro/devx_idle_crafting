using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dreamteck.Splines.IO
{
	public class CSV : SplineParser
	{
		public enum ColumnType
		{
			Position,
			Tangent,
			Tangent2,
			Normal,
			Size,
			Color
		}

		public List<ColumnType> columns = new List<ColumnType>();

		public CSV(SplineComputer computer)
		{
			Spline spline = new Spline(computer.type, computer.precision)
			{
				points = computer.GetPoints()
			};
			if (spline.type != Spline.Type.Bezier && spline.type != Spline.Type.Linear)
			{
				spline.ConvertToBezier();
			}
			if (computer.isClosed)
			{
				spline.Close();
			}
			buffer = new SplineDefinition(computer.name, spline);
			fileName = computer.name;
			columns.Add(ColumnType.Position);
			columns.Add(ColumnType.Tangent);
			columns.Add(ColumnType.Tangent2);
		}

		public CSV(string filePath, List<ColumnType> customColumns = null)
		{
			if (!File.Exists(filePath))
			{
				return;
			}
			string a = Path.GetExtension(filePath).ToLower();
			fileName = Path.GetFileNameWithoutExtension(filePath);
			if (a != ".csv")
			{
				UnityEngine.Debug.LogError("CSV Parsing ERROR: Wrong format. Please use SVG or XML");
				return;
			}
			string[] lines = File.ReadAllLines(filePath);
			if (customColumns == null)
			{
				columns.Add(ColumnType.Position);
				columns.Add(ColumnType.Tangent);
				columns.Add(ColumnType.Tangent2);
				columns.Add(ColumnType.Normal);
				columns.Add(ColumnType.Size);
				columns.Add(ColumnType.Color);
			}
			else
			{
				columns = new List<ColumnType>(customColumns);
			}
			buffer = new SplineDefinition(fileName, Spline.Type.Hermite);
			Read(lines);
		}

		private void Read(string[] lines)
		{
			int num = 0;
			using (List<ColumnType>.Enumerator enumerator = columns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case ColumnType.Position:
						num += 3;
						break;
					case ColumnType.Tangent:
						num += 3;
						break;
					case ColumnType.Tangent2:
						num += 3;
						break;
					case ColumnType.Normal:
						num += 3;
						break;
					case ColumnType.Size:
						num++;
						break;
					case ColumnType.Color:
						num += 4;
						break;
					}
				}
			}
			for (int i = 1; i < lines.Length; i++)
			{
				lines[i] = Regex.Replace(lines[i], "\\s+", string.Empty);
				string[] array = lines[i].Split(',');
				if (array.Length != num)
				{
					UnityEngine.Debug.LogError("Unexpected element count on row " + i + ". Expected " + num + " found " + array.Length + " Please make sure that all values exist and the column order is correct.");
					continue;
				}
				float[] array2 = new float[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					float.TryParse(array[j], out array2[j]);
				}
				int num2 = 0;
				using (List<ColumnType>.Enumerator enumerator2 = columns.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						switch (enumerator2.Current)
						{
						case ColumnType.Position:
							buffer.position = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
							break;
						case ColumnType.Tangent:
							buffer.tangent = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
							break;
						case ColumnType.Tangent2:
							buffer.tangent2 = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
							break;
						case ColumnType.Normal:
							buffer.normal = new Vector3(array2[num2++], array2[num2++], array2[num2++]);
							break;
						case ColumnType.Size:
							buffer.size = array2[num2++];
							break;
						case ColumnType.Color:
							buffer.color = new Color(array2[num2++], array2[num2++], array2[num2++], array2[num2++]);
							break;
						}
					}
				}
				buffer.CreateSmooth();
			}
		}

		public SplineComputer CreateSplineComputer(Vector3 position, Quaternion rotation)
		{
			return buffer.CreateSplineComputer(position, rotation);
		}

		public Spline CreateSpline()
		{
			return buffer.CreateSpline();
		}

		public void FlatX()
		{
			for (int i = 0; i < buffer.pointCount; i++)
			{
				SplinePoint value = buffer.points[i];
				value.position.x = 0f;
				value.tangent.x = 0f;
				value.tangent2.x = 0f;
				value.normal = Vector3.right;
				buffer.points[i] = value;
			}
		}

		public void FlatY()
		{
			for (int i = 0; i < buffer.pointCount; i++)
			{
				SplinePoint value = buffer.points[i];
				value.position.y = 0f;
				value.tangent.y = 0f;
				value.tangent2.y = 0f;
				value.normal = Vector3.up;
				buffer.points[i] = value;
			}
		}

		public void FlatZ()
		{
			for (int i = 0; i < buffer.pointCount; i++)
			{
				SplinePoint value = buffer.points[i];
				value.position.z = 0f;
				value.tangent.z = 0f;
				value.tangent2.z = 0f;
				value.normal = Vector3.back;
				buffer.points[i] = value;
			}
		}

		private void AddTitle(ref string[] content, string title)
		{
			string[] array;
			if (!string.IsNullOrEmpty(content[0]))
			{
				(array = content)[0] = array[0] + ",";
			}
			(array = content)[0] = array[0] + title;
		}

		private void AddVector3Title(ref string[] content, string prefix)
		{
			AddTitle(ref content, prefix + "X," + prefix + "Y," + prefix + "Z");
		}

		private void AddColorTitle(ref string[] content, string prefix)
		{
			AddTitle(ref content, prefix + "R," + prefix + "G," + prefix + "B" + prefix + "A");
		}

		private void AddVector3(ref string[] content, int index, Vector3 vector)
		{
			AddFloat(ref content, index, vector.x);
			AddFloat(ref content, index, vector.y);
			AddFloat(ref content, index, vector.z);
		}

		private void AddColor(ref string[] content, int index, Color color)
		{
			AddFloat(ref content, index, color.r);
			AddFloat(ref content, index, color.g);
			AddFloat(ref content, index, color.b);
			AddFloat(ref content, index, color.a);
		}

		private void AddFloat(ref string[] content, int index, float value)
		{
			string[] array;
			if (!string.IsNullOrEmpty(content[index]))
			{
				int num;
				(array = content)[num = index] = array[num] + ",";
			}
			int num2;
			(array = content)[num2 = index] = array[num2] + value.ToString();
		}

		public void Write(string filePath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filePath)))
			{
				throw new DirectoryNotFoundException("The file is being saved to a non-existing directory.");
			}
			List<SplinePoint> points = buffer.points;
			string[] content = new string[points.Count + 1];
			using (List<ColumnType>.Enumerator enumerator = columns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case ColumnType.Position:
						AddVector3Title(ref content, "Position");
						break;
					case ColumnType.Tangent:
						AddVector3Title(ref content, "Tangent");
						break;
					case ColumnType.Tangent2:
						AddVector3Title(ref content, "Tangent2");
						break;
					case ColumnType.Normal:
						AddVector3Title(ref content, "Normal");
						break;
					case ColumnType.Size:
						AddTitle(ref content, "Size");
						break;
					case ColumnType.Color:
						AddColorTitle(ref content, "Color");
						break;
					}
				}
			}
			foreach (ColumnType column in columns)
			{
				for (int i = 1; i <= points.Count; i++)
				{
					int index = i - 1;
					switch (column)
					{
					case ColumnType.Position:
					{
						int index7 = i;
						SplinePoint splinePoint6 = points[index];
						AddVector3(ref content, index7, splinePoint6.position);
						break;
					}
					case ColumnType.Tangent:
					{
						int index6 = i;
						SplinePoint splinePoint5 = points[index];
						AddVector3(ref content, index6, splinePoint5.tangent);
						break;
					}
					case ColumnType.Tangent2:
					{
						int index5 = i;
						SplinePoint splinePoint4 = points[index];
						AddVector3(ref content, index5, splinePoint4.tangent2);
						break;
					}
					case ColumnType.Normal:
					{
						int index4 = i;
						SplinePoint splinePoint3 = points[index];
						AddVector3(ref content, index4, splinePoint3.normal);
						break;
					}
					case ColumnType.Size:
					{
						int index3 = i;
						SplinePoint splinePoint2 = points[index];
						AddFloat(ref content, index3, splinePoint2.size);
						break;
					}
					case ColumnType.Color:
					{
						int index2 = i;
						SplinePoint splinePoint = points[index];
						AddColor(ref content, index2, splinePoint.color);
						break;
					}
					}
				}
			}
			File.WriteAllLines(filePath, content);
		}
	}
}
