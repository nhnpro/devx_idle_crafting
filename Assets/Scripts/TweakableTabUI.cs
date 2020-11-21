using UnityEngine;
using UnityEngine.UI;

public class TweakableTabUI : MonoBehaviour
{
	[SerializeField]
	private Image m_tab;

	[SerializeField]
	private Text m_text;

	public string TabName => m_text.text;

	public void OnSelected()
	{
		TweakableManagerUI tweakableManagerUI = UnityEngine.Object.FindObjectOfType(typeof(TweakableManagerUI)) as TweakableManagerUI;
		if (tweakableManagerUI != null)
		{
			tweakableManagerUI.ChangeTab(m_text.text);
		}
	}

	public void SetSelected(bool selected)
	{
		m_tab.color = ((!selected) ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f, 0.5f));
	}

	public void Init(string text)
	{
		m_text.text = text;
	}
}
