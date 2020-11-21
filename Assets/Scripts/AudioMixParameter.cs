using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixParameter : MonoBehaviour, IEventListener
{
	[Serializable]
	public class AudioMixCurve
	{
		public AnimationCurve parameterCurve;

		public string exposedMixerParameterName;
	}

	public enum AUDIOPARAMMODE
	{
		None,
		Slew
	}

	public AudioMixer audioMixer;

	public string m_audioEventName = string.Empty;

	public float m_defaultValue;

	public AudioMixCurve[] audioMixCurves;

	public AUDIOPARAMMODE m_interpolationMode;

	public float m_interpolationRate;

	private float m_inputFloatParameter;

	private float m_currentValue;

	private void Awake()
	{
		m_inputFloatParameter = m_defaultValue;
		m_currentValue = m_defaultValue;
	}

	private void OnEnable()
	{
		AudioController.Instance.RegisterListener(this, m_audioEventName);
	}

	private void OnDisable()
	{
		if (AudioController.Instance != null)
		{
			AudioController.Instance.UnregisterListener(this, m_audioEventName);
		}
	}

	bool IEventListener.HandleEvent(IEvent evt)
	{
		float num = m_inputFloatParameter = evt.GetAudioFloatParameter();
		return true;
	}

	private void Update()
	{
		switch (m_interpolationMode)
		{
		case AUDIOPARAMMODE.None:
			EvaluteCurves(m_inputFloatParameter);
			break;
		case AUDIOPARAMMODE.Slew:
			m_currentValue = Mathf.MoveTowards(m_currentValue, m_inputFloatParameter, m_interpolationRate * Time.deltaTime);
			EvaluteCurves(m_currentValue);
			break;
		}
	}

	private void EvaluteCurves(float floatParameter)
	{
		for (int i = 0; i < audioMixCurves.Length; i++)
		{
			audioMixer.SetFloat(audioMixCurves[i].exposedMixerParameterName, audioMixCurves[i].parameterCurve.Evaluate(floatParameter));
		}
	}
}
