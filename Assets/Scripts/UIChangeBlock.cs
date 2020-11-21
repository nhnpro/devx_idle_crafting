using UnityEngine;

public class UIChangeBlock : MonoBehaviour
{
	[SerializeField]
	private BlockType m_blockType;

	[SerializeField]
	private int m_blockSize = 1;

	public void OnChangeBlock()
	{
		LevelEditorControls.Instance.ChangeBlockType(m_blockType);
	}

	public void OnChangeSize()
	{
		LevelEditorControls.Instance.ChangeBlockSize(m_blockSize);
	}

	public void OnChangeBlockAndSize()
	{
		LevelEditorControls.Instance.ChangeBlockType(m_blockType);
		LevelEditorControls.Instance.ChangeBlockSize(m_blockSize);
	}
}
