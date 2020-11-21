using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	[HideInInspector]
	public bool m_IsInitialised;

	private static Hashtable m_listenerTable = new Hashtable();

	private static Queue m_eventQueue = new Queue();

	public bool m_limitQueueProcesing;

	public float m_queueProcessTime;

	public bool m_debugEventSystem = true;

	private static AudioController instance;

	public static AudioController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<AudioController>();
				Object.DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}
	}

	private void Update()
	{
		ProcessAudioEventQueue();
	}

	public bool RegisterListener(IEventListener listener, string eventName)
	{
		if (listener == null || eventName == null)
		{
			Instance.LogWarning("RegisterListener failed, no listener or event name.");
			return false;
		}
		if (!m_listenerTable.ContainsKey(eventName))
		{
			m_listenerTable.Add(eventName, new ArrayList());
		}
		ArrayList arrayList = m_listenerTable[eventName] as ArrayList;
		if (arrayList.Contains(listener))
		{
			Instance.LogWarning("RegisterListener failed " + listener.GetType().ToString() + " is already in list for the event: " + eventName);
			return false;
		}
		Instance.Log("Register Listener: " + listener + " for the event: " + eventName);
		arrayList.Add(listener);
		return true;
	}

	public bool UnregisterListener(IEventListener listener, string eventName)
	{
		if (!m_listenerTable.ContainsKey(eventName))
		{
			return false;
		}
		ArrayList arrayList = m_listenerTable[eventName] as ArrayList;
		if (!arrayList.Contains(listener))
		{
			return false;
		}
		Instance.Log("UnRegister Listener: " + listener + " for event: " + eventName);
		arrayList.Remove(listener);
		return true;
	}

	public bool TriggerEvent(IEvent evt)
	{
		string name = evt.GetName();
		string text = "[TIMESTAMP] // " + AudioSettings.dspTime + " //  [EVENTNAME]: " + evt.GetName() + "   [EVENTACTION]: " + evt.GetAudioEventActionString() + "   [PARENTOBJECT]: " + evt.GetParentGameObject();
		if (evt.GetAudioEventAction() == 7)
		{
			text = text + "   [VALUE]: " + evt.GetAudioFloatParameter();
		}
		Instance.Log(text);
		if (!m_listenerTable.ContainsKey(name))
		{
			Instance.LogWarning("No EventListeners: " + evt.GetName());
			return false;
		}
		ArrayList arrayList = m_listenerTable[name] as ArrayList;
		object[] array = arrayList.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			IEventListener eventListener = (IEventListener)array[i];
			eventListener.HandleEvent(evt);
		}
		return true;
	}

	public bool QueueEvent(IEvent evt)
	{
		if (!m_listenerTable.ContainsKey(evt.GetName()))
		{
			Instance.LogWarning("No EventListeners: " + evt.GetName());
			return false;
		}
		m_eventQueue.Enqueue(evt);
		return true;
	}

	private void ProcessAudioEventQueue()
	{
		float num = 0f;
		while (m_eventQueue.Count > 0 && (!m_limitQueueProcesing || !(num > m_queueProcessTime)))
		{
			IEvent @event = m_eventQueue.Dequeue() as IEvent;
			if (!TriggerEvent(@event))
			{
				Instance.LogWarning("Error Processing: " + @event.GetName());
				if (m_limitQueueProcesing)
				{
					num += Time.deltaTime;
				}
			}
		}
	}

	public void Log(string log)
	{
		if (!Instance.m_debugEventSystem)
		{
		}
	}

	public void LogWarning(string log)
	{
		UnityEngine.Debug.LogWarning("<color=yellow>[AUDIO] - </color>" + log);
	}

	public void LogError(string log)
	{
		UnityEngine.Debug.LogError("<color=red>[AUDIO] - </color>" + log);
	}
}
