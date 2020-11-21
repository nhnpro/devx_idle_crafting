using UnityEngine;

public class UISetActiveByPlatform : MonoBehaviour
{
	[SerializeField]
	private Platform m_platform;

	private Platform m_currentPlatform;

	private void Start()
	{
		m_currentPlatform = Platform.UNITY_ANDROID;
		if (m_currentPlatform == m_platform)
		{
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
