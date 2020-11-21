using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioGroupComponent : MonoBehaviour
{
	public bool m_overrideParentOutput;

	public AudioMixerGroup m_output;

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

	public bool m_overrideParentPositioning;

	public AudioObjectPositioning m_positioning;

	public Hashtable m_childComponents = new Hashtable();

	public ArrayList m_hashKeys = new ArrayList();

	private AudioGroupComponent m_parentGroupComponent;

	private bool m_initialised;

	private bool m_registeredWithParent;

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
		if (base.transform.parent != null)
		{
			m_parentGroupComponent = base.transform.parent.gameObject.GetComponent<AudioGroupComponent>();
		}
		if ((bool)m_parentGroupComponent && m_parentGroupComponent.RegisterComponent(base.gameObject))
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
		AudioRandomSimpleComponent component3 = _gameObject.GetComponent<AudioRandomSimpleComponent>();
		if ((bool)component3)
		{
			m_childComponents.Add(_gameObject, component3);
			m_hashKeys.Add(_gameObject);
			if (!component3.m_overrideParentOutput)
			{
				component3.m_output = m_output;
			}
			if (!component3.m_overrideParentPositioning)
			{
				component3.m_positioning = m_positioning;
			}
			return true;
		}
		AudioGroupComponent component4 = _gameObject.GetComponent<AudioGroupComponent>();
		if ((bool)component4)
		{
			m_childComponents.Add(_gameObject, component4);
			m_hashKeys.Add(_gameObject);
			if (!component4.m_overrideParentOutput)
			{
				component4.m_output = m_output;
			}
			if (!component4.m_overrideParentPositioning)
			{
				component4.m_positioning = m_positioning;
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
			if (obj is AudioRandomSimpleComponent)
			{
				AudioRandomSimpleComponent audioRandomSimpleComponent = obj as AudioRandomSimpleComponent;
				audioRandomSimpleComponent.m_parentVolume = m_realVolume;
				audioRandomSimpleComponent.m_parentPitch = m_realPitch;
			}
			if (obj is AudioGroupComponent)
			{
				AudioGroupComponent audioGroupComponent = obj as AudioGroupComponent;
				audioGroupComponent.m_parentVolume = m_realVolume;
				audioGroupComponent.m_parentPitch = m_realPitch;
			}
		}
	}
}
