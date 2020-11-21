using UnityEngine;
using UnityEngine.UI;

public class AutoPlayToggle : MonoBehaviour
{
	[SerializeField]
	private float m_delay = 0.5f;

	[SerializeField]
	private float m_closeDelay = 1f;

	[SerializeField]
	private Toggle m_theToggle;
}
