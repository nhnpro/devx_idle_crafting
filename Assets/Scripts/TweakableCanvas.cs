using UnityEngine;
using UnityEngine.UI;

public class TweakableCanvas : MonoBehaviour
{
	[SerializeField]
	private TweakableManagerUI m_manager;

	[SerializeField]
	private Button m_openButton;

	public TweakableManagerUI Manager => m_manager;

	protected void Update()
	{
		if (Debug.isDebugBuild)
		{
			m_manager.UpdateKeyShortCuts();
		}
	}

	public void OnOpen()
	{
		m_manager.gameObject.SetActive(value: true);
		m_openButton.gameObject.SetActive(value: false);
	}

	public void OnClose()
	{
		m_manager.gameObject.SetActive(value: false);
		m_openButton.gameObject.SetActive(value: true);
	}
}
