using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioComponent : MonoBehaviour, IPlayableAudioComponent
{
	public enum STEALINGBEHAVIOUR
	{
		StealOldest,
		StealNewest,
		StealQuietest,
		JustFail,
		JustFailIfQuietest
	}

	public AudioClip m_audioClip;

	private AudioObject m_nextAudioObjectToPlay;

	private bool m_paused;

	public int m_maxInstances = 1;

	private List<AudioObject> m_instances = new List<AudioObject>();

	public STEALINGBEHAVIOUR m_stealingBehaviour = STEALINGBEHAVIOUR.JustFail;

	public int m_priority = 128;

	public bool m_overrideParentOutput;

	public AudioMixerGroup m_output;

	public float m_volume = 1f;

	[HideInInspector]
	public float m_parentVolume = 1f;

	public bool m_randomiseVolume;

	public float m_minVolume = 1f;

	public float m_maxVolume = 1f;

	public float m_pitch = 1f;

	[HideInInspector]
	public float m_parentPitch = 1f;

	public bool m_randomisePitch;

	public float m_minPitch = 1f;

	public float m_maxPitch = 1f;

	public float m_fadeInTime;

	public AnimationCurve m_fadeInCurve;

	public float m_fadeOutTime;

	public AnimationCurve m_fadeOutCurve;

	public AudioObjectSettings m_settings = new AudioObjectSettings();

	public bool m_overrideParentPositioning;

	public AudioObjectPositioning m_positioning;

	private Object[] m_audioListeners;

	private AudioListener m_audioListener;

	private AudioGroupComponent m_parentGroupComponent;

	private AudioRandomComponent m_parentRandomComponent;

	private bool m_registeredWithParent;

	private bool m_initialised;

	private void Init()
	{
		RegisterWithParent();
		for (int i = 1; i <= m_maxInstances; i++)
		{
			m_instances.Add(new AudioObject());
		}
		int num = 1;
		foreach (AudioObject instance in m_instances)
		{
			instance.gameObject = new GameObject("[AudioSource " + num++ + "] ");
			instance.name = instance.gameObject.name;
			instance.gameObject.AddComponent<AudioSource>();
			instance.audioSource = instance.gameObject.GetComponent<AudioSource>();
			instance.audioSource.playOnAwake = false;
			instance.audioSource.volume = 1f;
			instance.audioSource.pitch = 1f;
			instance.audioSource.outputAudioMixerGroup = m_output;
			instance.gameObject.transform.parent = base.transform;
			instance.audioState = AudioObject.AUDIOSTATE.Waiting;
			instance.targetVolume = 1f;
			instance.setVolume = 1f;
			instance.realVolume = 1f;
			instance.curveFadeModifier = 1f;
			instance.targetPitch = 1f;
			instance.setPitch = 1f;
			instance.realPitch = 1f;
			instance.gameObject.SetActive(value: false);
			UpdateParameters(instance);
		}
		if (m_instances.Capacity > 0)
		{
			m_nextAudioObjectToPlay = m_instances[0];
		}
		GetAudioListener();
		if (m_settings.playOnAwake)
		{
			Play(null);
		}
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

	private void UnRegisterWithParent()
	{
		m_parentGroupComponent = base.transform.parent.gameObject.GetComponent<AudioGroupComponent>();
		if ((bool)m_parentGroupComponent && m_parentGroupComponent.UnRegisterComponent(base.gameObject))
		{
			m_parentGroupComponent = null;
			m_registeredWithParent = false;
		}
		m_parentRandomComponent = base.transform.parent.gameObject.GetComponent<AudioRandomComponent>();
		if ((bool)m_parentRandomComponent && m_parentRandomComponent.UnRegisterComponent(base.gameObject))
		{
			m_parentRandomComponent = null;
			m_registeredWithParent = false;
		}
	}

	private void OnDisable()
	{
		UnRegisterWithParent();
	}

	private void OnDestroy()
	{
		UnRegisterWithParent();
		m_instances.Clear();
	}

	private void Update()
	{
		for (int i = 0; i < m_instances.Count; i++)
		{
			AudioObject audioObject = m_instances[i];
			if (audioObject.audioState == AudioObject.AUDIOSTATE.Stopped)
			{
				continue;
			}
			UpdateVolume(audioObject);
			UpdatePosition(audioObject);
			UpdatePitch(audioObject);
			if (!audioObject.audioSource.isPlaying && audioObject.audioState != AudioObject.AUDIOSTATE.Paused && !audioObject.audioSource.loop)
			{
				audioObject.audioState = AudioObject.AUDIOSTATE.Stopped;
				audioObject.gameObject.SetActive(value: false);
				if (m_nextAudioObjectToPlay == null)
				{
					m_nextAudioObjectToPlay = audioObject;
				}
			}
		}
	}

	public void SetVolume(float _volume)
	{
		foreach (AudioObject instance in m_instances)
		{
			if (instance.parentGameObject == null)
			{
				instance.setVolume = _volume;
			}
		}
	}

	public void SetVolume(float _volume, GameObject _parentGameObject)
	{
		foreach (AudioObject instance in m_instances)
		{
			if (instance.parentGameObject == _parentGameObject)
			{
				instance.setVolume = _volume;
			}
		}
	}

	public void SetPitch(float _pitch)
	{
		foreach (AudioObject instance in m_instances)
		{
			if (instance.parentGameObject == null)
			{
				instance.setPitch = _pitch;
			}
		}
	}

	public void SetPitch(float _pitch, GameObject _parentGameObject)
	{
		foreach (AudioObject instance in m_instances)
		{
			if (instance.parentGameObject == _parentGameObject)
			{
				instance.setPitch = _pitch;
			}
		}
	}

	private void UpdatePitch(AudioObject _audioObject)
	{
		_audioObject.realPitch = Mathf.Clamp(_audioObject.targetPitch * m_parentPitch * _audioObject.setPitch, -3f, 3f);
		_audioObject.audioSource.pitch = _audioObject.realPitch;
	}

	private void UpdateVolume(AudioObject _audioObject)
	{
		if (!(_audioObject.audioSource != null))
		{
			return;
		}
		switch (_audioObject.audioState)
		{
		case AudioObject.AUDIOSTATE.Playing:
			_audioObject.realVolume = Mathf.Clamp(_audioObject.targetVolume * m_parentVolume * _audioObject.setVolume, 0f, 1f);
			_audioObject.audioSource.volume = _audioObject.realVolume;
			break;
		case AudioObject.AUDIOSTATE.FadeIn:
			if (_audioObject.curveFadeModifier < 1f)
			{
				_audioObject.curveFadeModifier = m_fadeInCurve.Evaluate(_audioObject.currentTimeFading);
				_audioObject.realVolume = Mathf.Clamp(_audioObject.targetVolume * m_parentVolume * _audioObject.setVolume * _audioObject.curveFadeModifier, 0f, 1f);
				_audioObject.audioSource.volume = _audioObject.realVolume;
				_audioObject.currentTimeFading += Time.deltaTime;
			}
			else
			{
				_audioObject.audioState = AudioObject.AUDIOSTATE.Playing;
				_audioObject.currentTimeFading = 0f;
			}
			break;
		case AudioObject.AUDIOSTATE.FadeOut:
			if (_audioObject.curveFadeModifier > 0f)
			{
				_audioObject.curveFadeModifier = m_fadeOutCurve.Evaluate(_audioObject.currentTimeFading);
				_audioObject.realVolume = Mathf.Clamp(_audioObject.targetVolume * m_parentVolume * _audioObject.setVolume * _audioObject.curveFadeModifier, 0f, 1f);
				_audioObject.audioSource.volume = _audioObject.realVolume;
				_audioObject.currentTimeFading += Time.deltaTime;
			}
			else
			{
				_audioObject.audioState = AudioObject.AUDIOSTATE.Stopped;
				_audioObject.currentTimeFading = 0f;
				_audioObject.realVolume = 0f;
				_audioObject.audioSource.volume = _audioObject.realVolume;
				_audioObject.audioSource.Stop();
			}
			break;
		}
	}

	public void UpdatePosition(AudioObject _audioObject)
	{
		if (_audioObject.parentGameObject != null)
		{
			_audioObject.gameObject.transform.position = _audioObject.parentGameObject.transform.position;
			_audioObject.audioSource.panStereo = m_positioning.panStereo;
		}
		else if (!m_positioning.randomisePanStereo)
		{
			_audioObject.audioSource.panStereo = m_positioning.panStereo;
		}
	}

	public void UpdateParameters(AudioObject _audioObject)
	{
		_audioObject.audioSource.priority = m_priority;
		_audioObject.audioSource.ignoreListenerVolume = m_settings.ignoreListenerVolume;
		_audioObject.audioSource.ignoreListenerPause = m_settings.ignoreListenerPause;
		_audioObject.audioSource.bypassEffects = m_settings.bypassEffects;
		_audioObject.audioSource.bypassListenerEffects = m_settings.bypassListenerEffects;
		_audioObject.audioSource.bypassReverbZones = m_settings.bypassReverbZones;
		_audioObject.audioSource.loop = m_settings.loop;
		_audioObject.audioSource.mute = m_settings.mute;
		_audioObject.audioSource.dopplerLevel = m_positioning.dopplerLevel;
		_audioObject.audioSource.spatialBlend = m_positioning.spatialBlend;
		_audioObject.audioSource.spread = m_positioning.spread;
		_audioObject.audioSource.minDistance = m_positioning.minDistance;
		_audioObject.audioSource.maxDistance = m_positioning.maxDistance;
		switch (m_positioning.volumeRolloff)
		{
		case AudioObjectPositioning.VOLUMEROLLOFF.LinearRolloff:
			_audioObject.audioSource.rolloffMode = AudioRolloffMode.Linear;
			break;
		case AudioObjectPositioning.VOLUMEROLLOFF.LogarithmicRolloff:
			_audioObject.audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
			break;
		case AudioObjectPositioning.VOLUMEROLLOFF.Custom:
			_audioObject.audioSource.rolloffMode = AudioRolloffMode.Custom;
			_audioObject.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, m_positioning.customVolumeRolloff);
			break;
		}
		if (m_positioning.useReverbMixCustomCurve)
		{
			_audioObject.audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, m_positioning.customReverbZoneMixCurve);
		}
		if (m_positioning.useSpatialBlendCurve)
		{
			_audioObject.audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, m_positioning.customSpreadCurve);
		}
		if (m_positioning.useSpreadCustomCurve)
		{
			_audioObject.audioSource.SetCustomCurve(AudioSourceCurveType.Spread, m_positioning.customSpreadCurve);
		}
	}

	public void GetAudioListener()
	{
		m_audioListeners = UnityEngine.Object.FindObjectsOfType(typeof(AudioListener));
		Object[] audioListeners = m_audioListeners;
		int num = 0;
		AudioListener audioListener;
		while (true)
		{
			if (num < audioListeners.Length)
			{
				audioListener = (AudioListener)audioListeners[num];
				if (audioListener.enabled)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		m_audioListener = audioListener;
	}

	public void Play(GameObject _parentGameObject)
	{
		if (m_nextAudioObjectToPlay == null && m_instances.Capacity > 0)
		{
			switch (m_stealingBehaviour)
			{
			case STEALINGBEHAVIOUR.StealOldest:
				m_instances = (from audioObject in m_instances
					orderby audioObject.initTime
					select audioObject).ToList();
				m_nextAudioObjectToPlay = m_instances[0];
				m_nextAudioObjectToPlay.audioSource.Stop();
				AudioController.Instance.Log("StealOldest " + base.name);
				break;
			case STEALINGBEHAVIOUR.StealNewest:
				m_instances = (from audioObject in m_instances
					orderby audioObject.initTime descending
					select audioObject).ToList();
				m_nextAudioObjectToPlay = m_instances[0];
				m_nextAudioObjectToPlay.audioSource.Stop();
				AudioController.Instance.Log("StealNewest " + base.name);
				break;
			case STEALINGBEHAVIOUR.StealQuietest:
				if (m_audioListener != null)
				{
					foreach (AudioObject instance in m_instances)
					{
						instance.listenerDistance = Mathf.Sqrt((m_audioListener.gameObject.transform.position - instance.gameObject.transform.position).sqrMagnitude);
					}
					m_instances = (from audioObject in m_instances
						orderby audioObject.listenerDistance descending
						select audioObject).ToList();
					m_nextAudioObjectToPlay = m_instances[0];
					m_nextAudioObjectToPlay.audioSource.Stop();
					AudioController.Instance.Log("StealQuietest " + base.name);
				}
				break;
			case STEALINGBEHAVIOUR.JustFail:
				m_nextAudioObjectToPlay = null;
				AudioController.Instance.Log("JustFail " + base.name);
				break;
			case STEALINGBEHAVIOUR.JustFailIfQuietest:
				if (m_audioListener != null && _parentGameObject != null)
				{
					float num = Mathf.Sqrt((m_audioListener.gameObject.transform.position - _parentGameObject.transform.position).sqrMagnitude);
					foreach (AudioObject instance2 in m_instances)
					{
						instance2.listenerDistance = Mathf.Sqrt((m_audioListener.gameObject.transform.position - instance2.gameObject.transform.position).sqrMagnitude);
					}
					m_instances = (from audioObject in m_instances
						orderby audioObject.listenerDistance descending
						select audioObject).ToList();
					if (num < m_instances[0].listenerDistance)
					{
						m_nextAudioObjectToPlay = m_instances[0];
						m_nextAudioObjectToPlay.audioSource.Stop();
						AudioController.Instance.Log("JustFailIfQuietest " + base.name);
					}
				}
				break;
			}
		}
		if (m_nextAudioObjectToPlay != null)
		{
			if (!(m_audioClip != null) || !(m_nextAudioObjectToPlay.gameObject != null))
			{
				return;
			}
			m_nextAudioObjectToPlay.gameObject.SetActive(value: true);
			m_nextAudioObjectToPlay.initTime = AudioSettings.dspTime;
			m_nextAudioObjectToPlay.parentGameObject = _parentGameObject;
			m_nextAudioObjectToPlay.audioSource.clip = m_audioClip;
			if (m_settings.randomiseStartSeekTime)
			{
				m_nextAudioObjectToPlay.audioSource.time = UnityEngine.Random.Range(0f, m_nextAudioObjectToPlay.audioSource.clip.length);
			}
			if (m_randomiseVolume)
			{
				m_nextAudioObjectToPlay.targetVolume = Mathf.Clamp(UnityEngine.Random.Range(m_minVolume, m_maxVolume), 0f, 1f);
			}
			else
			{
				m_nextAudioObjectToPlay.targetVolume = m_volume;
			}
			if (m_randomisePitch)
			{
				m_nextAudioObjectToPlay.targetPitch = Mathf.Clamp(UnityEngine.Random.Range(m_minPitch, m_maxPitch), -3f, 3f);
			}
			else
			{
				m_nextAudioObjectToPlay.targetPitch = m_pitch;
			}
			m_nextAudioObjectToPlay.realPitch = m_nextAudioObjectToPlay.targetPitch * m_parentPitch * m_nextAudioObjectToPlay.setPitch;
			if (m_positioning.randomisePanStereo)
			{
				float panStereo = UnityEngine.Random.Range(Mathf.Clamp(m_positioning.minRandomPan, -1f, 1f), Mathf.Clamp(m_positioning.maxRandomPan, -1f, 1f));
				m_nextAudioObjectToPlay.audioSource.panStereo = panStereo;
			}
			if (m_fadeInTime > 0f)
			{
				m_nextAudioObjectToPlay.audioSource.volume = 0f;
				m_nextAudioObjectToPlay.realVolume = 0f;
				m_nextAudioObjectToPlay.curveFadeModifier = 0f;
				m_nextAudioObjectToPlay.audioState = AudioObject.AUDIOSTATE.FadeIn;
				if (m_nextAudioObjectToPlay.audioSource.clip != null)
				{
					m_nextAudioObjectToPlay.audioSource.Play();
				}
			}
			else
			{
				m_nextAudioObjectToPlay.realVolume = m_nextAudioObjectToPlay.targetVolume * m_parentVolume * m_nextAudioObjectToPlay.setVolume;
				m_nextAudioObjectToPlay.audioSource.volume = m_nextAudioObjectToPlay.realVolume;
				m_nextAudioObjectToPlay.audioState = AudioObject.AUDIOSTATE.Playing;
				if (m_nextAudioObjectToPlay.audioSource.clip != null)
				{
					m_nextAudioObjectToPlay.audioSource.Play();
				}
			}
			m_settings.timesTriggered += 1f;
			m_nextAudioObjectToPlay = null;
		}
		else
		{
			AudioController.Instance.Log("Failed to Find AudioObject to PlaySound - " + base.name);
		}
	}

	public void Pause(GameObject _parentGameObject)
	{
		foreach (AudioObject instance in m_instances)
		{
			if (_parentGameObject != null)
			{
				if (instance.parentGameObject == _parentGameObject)
				{
					instance.audioState = AudioObject.AUDIOSTATE.Paused;
					instance.audioSource.Pause();
				}
			}
			else if (instance.parentGameObject == null)
			{
				instance.audioState = AudioObject.AUDIOSTATE.Paused;
				instance.audioSource.Pause();
			}
		}
	}

	public void PauseAll()
	{
		foreach (AudioObject instance in m_instances)
		{
			if (!m_paused)
			{
				if (instance.audioSource.isPlaying)
				{
					instance.windowFocusPause = true;
					instance.audioSource.Pause();
					instance.audioState = AudioObject.AUDIOSTATE.Paused;
					m_paused = true;
				}
			}
			else if (instance.windowFocusPause)
			{
				instance.gameObject.SetActive(value: true);
				instance.audioSource.Play();
				instance.audioState = AudioObject.AUDIOSTATE.Playing;
			}
		}
	}

	public void Stop(GameObject _parentGameObject)
	{
		foreach (AudioObject instance in m_instances)
		{
			if (_parentGameObject != null)
			{
				if (instance.parentGameObject == _parentGameObject && instance.audioSource.isPlaying)
				{
					if (m_fadeOutTime > 0f)
					{
						instance.curveFadeModifier = 1f;
						instance.currentTimeFading = 0f;
						instance.audioState = AudioObject.AUDIOSTATE.FadeOut;
					}
					else
					{
						instance.audioSource.Stop();
					}
				}
			}
			else if (instance.parentGameObject == null && instance.audioSource.isPlaying)
			{
				if (m_fadeOutTime > 0f)
				{
					instance.currentTimeFading = 0f;
					instance.audioState = AudioObject.AUDIOSTATE.FadeOut;
				}
				else
				{
					instance.audioSource.Stop();
				}
			}
		}
	}

	public void StopAll()
	{
		foreach (AudioObject instance in m_instances)
		{
			instance.audioSource.Stop();
		}
	}
}
