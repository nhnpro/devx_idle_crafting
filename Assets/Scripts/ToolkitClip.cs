using UnityEngine;
using UnityEngine.Serialization;

public class ToolkitClip : MonoBehaviour
{
	[FormerlySerializedAs("name")]
	[SerializeField]
	public string m_audioEventName;

	[FormerlySerializedAs("TargetObject")]
	[SerializeField]
	private GameObject m_targetObject;

	private AudioEvent m_audioEvent;

	public void Play()
	{
		if (m_audioEvent == null)
		{
			m_audioEvent = new AudioEvent(m_audioEventName, AUDIOEVENTACTION.Play, m_targetObject);
		}
		AudioController.Instance.QueueEvent(m_audioEvent);
	}
}
