using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Letter Spacing", 14)]
	[RequireComponent(typeof(Text))]
	public class LetterSpacing : BaseMeshEffect
	{
		[SerializeField]
		private float m_spacing;

		private Text text;

		public float spacing
		{
			get
			{
				return m_spacing;
			}
			set
			{
				if (m_spacing != value)
				{
					m_spacing = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		protected override void Awake()
		{
			text = GetComponent<Text>();
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				vh.GetUIVertexStream(list);
				ModifyVertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
			}
		}

		public void ModifyVertices(List<UIVertex> verts)
		{
			if (!IsActive())
			{
				return;
			}
			string[] array = this.text.text.Split('\n');
			float num = spacing * (float)this.text.fontSize / 100f;
			float num2 = 0f;
			int num3 = 0;
			switch (this.text.alignment)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				num2 = 0f;
				break;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				num2 = 0.5f;
				break;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				num2 = 1f;
				break;
			}
			foreach (string text in array)
			{
				float num4 = (float)(text.Length - 1) * num * num2;
				for (int j = 0; j < text.Length; j++)
				{
					Vector3 vector = Vector3.right * (num * (float)j - num4);
					for (int k = 0; k < 6; k++)
					{
						int index = num3 * 6 + k;
						UIVertex value = verts[index];
						value.position += vector;
						verts[index] = value;
					}
					num3++;
				}
				num3++;
			}
		}
	}
}
