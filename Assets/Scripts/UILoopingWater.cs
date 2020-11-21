using UnityEngine;
using UnityEngine.UI;

public class UILoopingWater : MonoBehaviour
{
	[SerializeField]
	private int m_loopSize;

	[SerializeField]
	private GameObject m_parallax;

	private RectTransform m_rect;

	private RectTransform m_parallaxRect;

	private CanvasScaler m_canvasScaler;

	protected void Start()
	{
		m_rect = GetComponent<RectTransform>();
		m_parallaxRect = m_parallax.GetComponent<RectTransform>();
		m_canvasScaler = BindingManager.Instance.UI.GetComponent<CanvasScaler>();
	}

	protected void Update()
	{
		Vector3 vector = BindingManager.Instance.UICamera.WorldToScreenPoint(m_rect.position);
		Vector2 referenceResolution = m_canvasScaler.referenceResolution;
		float num = referenceResolution.x / (float)Screen.width;
		Vector2 referenceResolution2 = m_canvasScaler.referenceResolution;
		float num2 = referenceResolution2.y / (float)Screen.height;
		vector = new Vector2(vector.x * num, vector.y * num2);
		if (vector.y < (float)(-m_loopSize))
		{
			m_rect.anchoredPosition += new Vector2(0f, m_loopSize * 3);
		}
		if (vector.y > (float)(m_loopSize * 2))
		{
			m_rect.anchoredPosition -= new Vector2(0f, m_loopSize * 3);
		}
		vector = BindingManager.Instance.UICamera.WorldToScreenPoint(m_rect.position);
		vector = new Vector2(0f, vector.y * num2);
		m_parallaxRect.anchoredPosition = vector;
	}
}
