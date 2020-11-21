using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRandomComponent : MonoBehaviour, IPlayableAudioComponent
{
	public float m_volume = 1f;

	[HideInInspector]
	public float m_parentVolume = 1f;

	private float m_setVolume = 1f;

	private float m_realVolume = 1f;

	public float m_pitch = 1f;

	[HideInInspector]
	public float m_parentPitch = 1f;

	private float m_setPitch = 1f;

	private float m_realPitch = 1f;

	public bool m_overrideParentOutput;

	public AudioMixerGroup m_output;

	public bool m_overrideParentPositioning;

	public AudioObjectPositioning m_positioning;

	public bool m_loop;

	public int m_numberOfLoops = -1;

	private int m_timesPlayed;

	public float m_minDelayRandomisation;

	public float m_maxDelayRandomisation;

	public Hashtable m_childComponents = new Hashtable();

	public ArrayList m_hashKeys = new ArrayList();

	private AudioGroupComponent m_parentGroupComponent;

	private AudioRandomComponent m_parentRandomComponent;

	private bool m_initialised;

	private bool m_registeredWithParent;

	private bool m_looping;

	private void Init()
	{
		RegisterWithParent();
		m_initialised = true;
	}

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		if (!m_registeredWithParent && m_initialised)
		{
			RegisterWithParent();
		}
	}

	private void OnDisable()
	{
		if ((bool)m_parentGroupComponent && m_registeredWithParent && m_parentGroupComponent.UnRegisterComponent(base.gameObject))
		{
			m_registeredWithParent = false;
		}
	}

	private void RegisterWithParent()
	{
		m_parentGroupComponent = base.transform.parent.gameObject.GetComponent<AudioGroupComponent>();
		if ((bool)m_parentGroupComponent && m_parentGroupComponent.RegisterComponent(base.gameObject))
		{
			m_registeredWithParent = true;
		}
		m_parentRandomComponent = base.transform.parent.gameObject.GetComponent<AudioRandomComponent>();
		if ((bool)m_parentRandomComponent && m_parentRandomComponent.RegisterComponent(base.gameObject))
		{
			m_registeredWithParent = true;
		}
	}

	public bool RegisterComponent(GameObject _gameObject)
	{
		AudioComponent component = _gameObject.GetComponent<AudioComponent>();
		if ((bool)component)
		{
			m_childComponents.Add(_gameObject, component);
			m_hashKeys.Add(_gameObject);
			if (!component.m_overrideParentOutput)
			{
				component.m_output = m_output;
			}
			if (!component.m_overrideParentPositioning)
			{
				component.m_positioning = m_positioning;
			}
			return true;
		}
		AudioRandomComponent component2 = _gameObject.GetComponent<AudioRandomComponent>();
		if ((bool)component2)
		{
			m_childComponents.Add(_gameObject, component2);
			m_hashKeys.Add(_gameObject);
			if (!component2.m_overrideParentOutput)
			{
				component2.m_output = m_output;
			}
			if (!component2.m_overrideParentPositioning)
			{
				component2.m_positioning = m_positioning;
			}
			return true;
		}
		return false;
	}

	public bool UnRegisterComponent(GameObject _gameObject)
	{
		if (m_childComponents.ContainsKey(_gameObject))
		{
			m_childComponents.Remove(_gameObject);
			m_hashKeys.Remove(_gameObject);
			return true;
		}
		return false;
	}

	private IEnumerator Loop(GameObject _parentGameObject)
	{
		while (m_looping)
		{
			PlayComponent(_parentGameObject);
			yield return new WaitForSeconds(UnityEngine.Random.Range(m_minDelayRandomisation, m_maxDelayRandomisation));
		}
	}

	public void Play(GameObject _parentGameObject)
	{
		if ((m_loop && m_numberOfLoops == -1) || (m_loop && m_timesPlayed < m_numberOfLoops))
		{
			m_looping = true;
			StartCoroutine(Loop(_parentGameObject));
		}
		else
		{
			PlayComponent(_parentGameObject);
		}
	}

	private void PlayComponent(GameObject _parentGameObject)
	{
		if (m_hashKeys.Count > 0)
		{
			object key = m_hashKeys[UnityEngine.Random.Range(0, m_hashKeys.Count)];
			object obj = m_childComponents[key];
			if (obj is AudioComponent)
			{
				AudioComponent audioComponent = obj as AudioComponent;
				audioComponent.Play(_parentGameObject);
				m_timesPlayed++;
			}
			if (obj is AudioRandomComponent)
			{
				AudioRandomComponent audioRandomComponent = obj as AudioRandomComponent;
				audioRandomComponent.Play(_parentGameObject);
				m_timesPlayed++;
			}
		}
	}

	public void Pause(GameObject _parentGameObject)
	{
		m_looping = false;
		StopCoroutine(Loop(_parentGameObject));
		IDictionaryEnumerator enumerator = m_childComponents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
				if (dictionaryEntry.Value is AudioComponent)
				{
					AudioComponent audioComponent = dictionaryEntry.Value as AudioComponent;
					audioComponent.Pause(_parentGameObject);
				}
				if (dictionaryEntry.Value is AudioRandomComponent)
				{
					AudioRandomComponent audioRandomComponent = dictionaryEntry.Value as AudioRandomComponent;
					audioRandomComponent.Pause(_parentGameObject);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void PauseAll()
	{
		m_looping = false;
		StopAllCoroutines();
		IDictionaryEnumerator enumerator = m_childComponents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
				if (dictionaryEntry.Value is AudioComponent)
				{
					AudioComponent audioComponent = dictionaryEntry.Value as AudioComponent;
					audioComponent.PauseAll();
				}
				if (dictionaryEntry.Value is AudioRandomComponent)
				{
					AudioRandomComponent audioRandomComponent = dictionaryEntry.Value as AudioRandomComponent;
					audioRandomComponent.PauseAll();
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void Stop(GameObject _parentGameObject)
	{
		m_looping = false;
		StopCoroutine(Loop(_parentGameObject));
		IDictionaryEnumerator enumerator = m_childComponents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
				if (dictionaryEntry.Value is AudioComponent)
				{
					AudioComponent audioComponent = dictionaryEntry.Value as AudioComponent;
					audioComponent.Stop(_parentGameObject);
				}
				if (dictionaryEntry.Value is AudioRandomComponent)
				{
					AudioRandomComponent audioRandomComponent = dictionaryEntry.Value as AudioRandomComponent;
					audioRandomComponent.Stop(_parentGameObject);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void StopAll()
	{
		m_looping = false;
		StopAllCoroutines();
		IDictionaryEnumerator enumerator = m_childComponents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
				if (dictionaryEntry.Value is AudioComponent)
				{
					AudioComponent audioComponent = dictionaryEntry.Value as AudioComponent;
					audioComponent.StopAll();
				}
				if (dictionaryEntry.Value is AudioRandomComponent)
				{
					AudioRandomComponent audioRandomComponent = dictionaryEntry.Value as AudioRandomComponent;
					audioRandomComponent.StopAll();
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void SetVolume(float _volume)
	{
		m_setVolume = _volume;
	}

	public void SetPitch(float _pitch)
	{
		m_setPitch = _pitch;
	}

	private void Update()
	{
		m_realVolume = Mathf.Clamp(m_volume * m_parentVolume * m_setVolume, 0f, 1f);
		m_realPitch = Mathf.Clamp(m_pitch * m_parentPitch * m_setPitch, -3f, 3f);
		for (int i = 0; i < m_hashKeys.Count; i++)
		{
			object obj = m_childComponents[m_hashKeys[i]];
			if (obj is AudioComponent)
			{
				AudioComponent audioComponent = obj as AudioComponent;
				audioComponent.m_parentVolume = m_realVolume;
				audioComponent.m_parentPitch = m_realPitch;
			}
			if (obj is AudioRandomComponent)
			{
				AudioRandomComponent audioRandomComponent = obj as AudioRandomComponent;
				audioRandomComponent.m_parentVolume = m_realVolume;
				audioRandomComponent.m_parentPitch = m_realPitch;
			}
		}
	}
}
