using UnityEngine;
using UnityEngine.UI;

public class DebugException : MonoBehaviour
{
	private static string FirstException;

	[SerializeField]
	private Canvas m_canvas;

	[SerializeField]
	private Text m_text;

	protected void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}
}
