using UnityEngine;

public class AudioEventTriggerOnDisable : MonoBehaviour
{
	public string m_audioEventName = string.Empty;

	public AUDIOEVENTACTION m_audioEventAction = AUDIOEVENTACTION.Play;

	public bool m_ignoreGameObject;

	private GameObject m_parentGameObject;

	private bool m_applicationQuitting;

	private void OnDisable()
	{
		if (!m_applicationQuitting && AudioController.Instance != null)
		{
			if (m_ignoreGameObject)
			{
				AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventName, m_audioEventAction));
				return;
			}
			m_parentGameObject = base.transform.gameObject;
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventName, m_audioEventAction, m_parentGameObject));
		}
	}

	private void OnApplicationQuit()
	{
		m_applicationQuitting = true;
	}
}
