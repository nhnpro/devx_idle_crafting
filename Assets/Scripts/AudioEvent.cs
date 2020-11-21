using UnityEngine;

public class AudioEvent : IEvent
{
	private string m_eventName;

	public AUDIOEVENTACTION m_audioEventAction;

	private GameObject m_parentGameObject;

	public float m_floatParameter = 1f;

	public AudioEvent(string _eventName, AUDIOEVENTACTION _audioEventAction)
	{
		m_eventName = _eventName;
		m_audioEventAction = _audioEventAction;
	}

	public AudioEvent(string _eventName, AUDIOEVENTACTION _audioEventAction, GameObject _parentGameObject)
	{
		m_eventName = _eventName;
		m_audioEventAction = _audioEventAction;
		m_parentGameObject = _parentGameObject;
	}

	public AudioEvent(string _eventName, float _floatParameter, GameObject _parentGameObject)
	{
		m_eventName = _eventName;
		m_floatParameter = _floatParameter;
		m_parentGameObject = _parentGameObject;
		m_audioEventAction = AUDIOEVENTACTION.FloatParameter;
	}

	string IEvent.GetName()
	{
		return m_eventName;
	}

	int IEvent.GetAudioEventAction()
	{
		return (int)m_audioEventAction;
	}

	GameObject IEvent.GetParentGameObject()
	{
		return m_parentGameObject;
	}

	string IEvent.GetAudioEventActionString()
	{
		return m_audioEventAction.ToString();
	}

	float IEvent.GetAudioFloatParameter()
	{
		return m_floatParameter;
	}
}
