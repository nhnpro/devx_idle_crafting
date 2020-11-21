using UnityEngine;

public enum AUDIOEVENTTRIGGER
{
	OnStart,
	OnAwake,
	OnEnable,
	OnDisable,
	OnDestroy
}
public class AudioEventTrigger : MonoBehaviour
{
	public string m_audioEventName = string.Empty;

	public AUDIOEVENTTRIGGER m_audioEventTrigger = AUDIOEVENTTRIGGER.OnEnable;

	public AUDIOEVENTACTION m_audioEventAction = AUDIOEVENTACTION.Play;

	public bool m_ignoreGameObject;

	private GameObject m_parentGameObject;

	private bool m_applicationQuitting;

	private void Awake()
	{
		if (m_audioEventTrigger == AUDIOEVENTTRIGGER.OnAwake)
		{
			TriggerEvent();
		}
	}

	private void Start()
	{
		if (m_audioEventTrigger == AUDIOEVENTTRIGGER.OnStart)
		{
			TriggerEvent();
		}
	}

	private void OnEnable()
	{
		if (m_audioEventTrigger == AUDIOEVENTTRIGGER.OnEnable)
		{
			TriggerEvent();
		}
	}

	private void OnDisable()
	{
		if (!m_applicationQuitting && m_audioEventTrigger == AUDIOEVENTTRIGGER.OnDisable && AudioController.Instance != null)
		{
			TriggerEvent();
		}
	}

	private void OnDestroy()
	{
		if (!m_applicationQuitting && m_audioEventTrigger == AUDIOEVENTTRIGGER.OnDestroy && AudioController.Instance != null)
		{
			TriggerEvent();
		}
	}

	private void TriggerEvent()
	{
		if (m_ignoreGameObject)
		{
			AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventName, m_audioEventAction));
			return;
		}
		m_parentGameObject = base.transform.gameObject;
		AudioController.Instance.QueueEvent(new AudioEvent(m_audioEventName, m_audioEventAction, m_parentGameObject));
	}

	private void OnApplicationQuit()
	{
		m_applicationQuitting = true;
	}
}
