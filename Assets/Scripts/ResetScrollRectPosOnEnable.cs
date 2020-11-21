using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ResetScrollRectPosOnEnable : MonoBehaviour
{
	[SerializeField]
	[Range(0f, 1f)]
	private float m_pos;

	protected void OnEnable()
	{
		GetComponent<ScrollRect>().normalizedPosition = new Vector2(m_pos, m_pos);
	}
}
