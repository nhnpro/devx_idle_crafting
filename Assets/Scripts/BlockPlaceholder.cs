using UnityEngine;

[ExecuteInEditMode]
public class BlockPlaceholder : MonoBehaviour
{
	[SerializeField]
	private BlockSize m_size;

	[SerializeField]
	private BlockType m_type;

	[SerializeField]
	private BlockAlignment m_alignment;

	[SerializeField]
	private bool m_snap = true;

	[SerializeField]
	[HideInInspector]
	private bool m_error;

	public int Height
	{
		get
		{
			switch (m_size)
			{
			case BlockSize.Two:
				return 2;
			case BlockSize.Four:
				return 4;
			default:
				return 1;
			}
		}
	}

	public int Width => Height;

	public BlockType Type => m_type;

	public BlockAlignment Alignment => m_alignment;
}
